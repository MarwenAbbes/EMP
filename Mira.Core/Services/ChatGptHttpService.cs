using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mira.Core.Services;

/// <summary>
/// ChatGPT service using direct HTTP calls for better control and transparency
/// </summary>
public class ChatGptHttpService
{
    #region Configuration Constants

    // OpenAI Responses API Configuration
    private const string OpenAiApiUrl = "https://api.openai.com/v1/responses";
    private const string DefaultModel = "gpt-4.1"; // Recommended model for this use case
    private const int MaxOutputTokens = 12000; // Enough for 13 detailed tables
    private const float DefaultTemperature = 0.1f; // Deterministic, low-variance output
    private const int TimeoutMinutes = 10; // Allow long multi-image requests

    #endregion

    #region Private Fields

    private readonly ILoggerService _logger;
    private readonly HttpClient _httpClient;
    private string? _apiKey;
    private string? _outputDirectory;

    #endregion

    #region Properties

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    #endregion

    #region Constructor

    public ChatGptHttpService(ILoggerService logger)
    {
        _logger = logger;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(TimeoutMinutes)
        };
        _logger.LogInfo($"ChatGPT HTTP service initialized (Model: {DefaultModel}, Timeout: {TimeoutMinutes}min)");
    }

    #endregion

    #region Public Methods

    public void SetApiKey(string apiKey)
    {
        _logger.LogDebug("Setting API key for HTTP service");
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Authorization =
     new AuthenticationHeaderValue("Bearer", apiKey);
        _logger.LogInfo($"API key configured for HTTP service (Model: {DefaultModel})");
    }

    public void SetOutputDirectory(string outputDirectory)
    {
        _logger.LogInfo($"Setting output directory for HTTP service: {outputDirectory}");
        _outputDirectory = outputDirectory;
    }

    /// <summary>
    /// Compares two PDFs by converting them to images and sending via HTTP
    /// </summary>
    public async Task<string> ComparePdfDocumentsAsync(
   string clientPdfPath,
        string supplierPdfPath,
    IProgress<string>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInfo($"=== HTTP COMPARISON: {clientPdfPath} vs {supplierPdfPath} ===");
        progressCallback?.Report("Starting HTTP-based PDF comparison...");

        ValidateConfiguration();
        ValidateFiles(clientPdfPath, supplierPdfPath);

        try
        {
            // Convert PDFs to images
            progressCallback?.Report("Converting PDFs to images...");
            var pdfImageService = new PdfImageService(_logger);

            var clientImages = pdfImageService.ConvertPdfToImages(clientPdfPath);
            var supplierImages = pdfImageService.ConvertPdfToImages(supplierPdfPath);

            _logger.LogInfo($"Client: {clientImages.Count} pages, Supplier: {supplierImages.Count} pages");

            // Use the main comparison method
            return await ComparePdfDocumentsViaHttpAsync(clientImages, supplierImages, progressCallback, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($">>> HTTP COMPARISON FAILED: {ex.Message} <<<", ex);
            progressCallback?.Report($"Comparison failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Compares pre-converted images via HTTP (main comparison method)
    /// </summary>
    public async Task<string> ComparePdfDocumentsViaHttpAsync(
 List<byte[]> clientImages,
        List<byte[]> supplierImages,
        IProgress<string>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInfo($"=== HTTP IMAGE COMPARISON: {clientImages.Count} client + {supplierImages.Count} supplier pages ===");

        ValidateConfiguration();

        try
        {
            // Build and serialize request
            progressCallback?.Report("Building HTTP request with all images...");
            var requestPayload = BuildComparisonRequest(clientImages, supplierImages);
            var jsonPayload = SerializeRequest(requestPayload);

            LogRequestMetrics(jsonPayload, clientImages, supplierImages);
            LogPromptToFile(clientImages, supplierImages);

            // Send HTTP request
            progressCallback?.Report($"Sending HTTP request with {clientImages.Count + supplierImages.Count} images...");
            var response = await SendHttpRequestAsync(jsonPayload, cancellationToken);

            // Parse and validate response
            progressCallback?.Report("Parsing response from ChatGPT...");
            var result = await ParseResponseAsync(response, cancellationToken);

            progressCallback?.Report("Comparison completed successfully!");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($">>> HTTP IMAGE COMPARISON FAILED: {ex.Message} <<<", ex);
            progressCallback?.Report($"Comparison failed: {ex.Message}");
            throw;
        }
    }

    #endregion

    #region Private Helper Methods - Validation

    private void ValidateConfiguration()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("API key not configured");
        }
    }

    private void ValidateFiles(string clientPdfPath, string supplierPdfPath)
    {
        if (!File.Exists(clientPdfPath) || !File.Exists(supplierPdfPath))
        {
            throw new FileNotFoundException("One or both PDF files not found");
        }
    }

    #endregion

    #region Private Helper Methods - Request Building

    private OpenAiChatRequest BuildComparisonRequest(List<byte[]> clientImages, List<byte[]> supplierImages)
    {
        string systemPrompt = GetSystemPrompt();
        string userPrompt = GetUserPrompt();

        // Build input array for Responses API
        var inputItems = new List<object>();

        // Add system instructions as the first item
        inputItems.Add(new
        {
            role = "system",
            content = new[]
 {
       new { type = "input_text", text = systemPrompt }
            }
        });

        // Build user message content
        var userContentList = new List<object>
        {
          new { type = "input_text", text = userPrompt },
         new { type = "input_text", text = "\n\n=== DOCUMENT CLIENT (RÉFÉRENCE MAÎTRE) ===\n" }
        };

        // Add client images
        AddImagesToInputContent(userContentList, clientImages, "CLIENT");
        userContentList.Add(new { type = "input_text", text = "\n=== FIN DOCUMENT CLIENT ===\n" });
        userContentList.Add(new { type = "input_text", text = "\n=== DOCUMENT FOURNISSEUR ===\n" });

        // Add supplier images
        AddImagesToInputContent(userContentList, supplierImages, "FOURNISSEUR");
        userContentList.Add(new { type = "input_text", text = "\n=== FIN DOCUMENT FOURNISSEUR ===\n" });

        // Add user message
        inputItems.Add(new
        {
            role = "user",
            content = userContentList.ToArray()
        });

        return new OpenAiChatRequest
        {
            Model = DefaultModel,
            Input = inputItems.ToArray(),
            MaxTokens = MaxOutputTokens,
            Temperature = DefaultTemperature
        };
    }

    private void AddImagesToInputContent(List<object> content, List<byte[]> images, string documentType)
    {
        for (int i = 0; i < images.Count; i++)
        {
            content.Add(new
            {
                type = "input_text",
                text = $"\n--- {documentType} PAGE {i + 1}/{images.Count} ---\n"
            });

            content.Add(new
            {
                type = "input_image",
                image_url = $"data:image/png;base64,{Convert.ToBase64String(images[i])}"
            });
        }
    }

    private string SerializeRequest(OpenAiChatRequest request)
    {
        return JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    }

    #endregion

    #region Private Helper Methods - HTTP Communication

    private async Task<HttpResponseMessage> SendHttpRequestAsync(string jsonPayload, CancellationToken cancellationToken)
    {
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        _logger.LogInfo(">>> SENDING HTTP POST TO OPENAI API <<<");
        var response = await _httpClient.PostAsync(OpenAiApiUrl, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError($"HTTP Error {response.StatusCode}: {errorContent}");
            throw new HttpRequestException($"OpenAI API returned {response.StatusCode}: {errorContent}");
        }

        return response;
    }

    private async Task<string> ParseResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var apiResponse = JsonSerializer.Deserialize<OpenAiChatResponse>(responseContent);

        if (apiResponse?.Output == null || apiResponse.Output.Count == 0)
        {
            throw new Exception("No response from OpenAI API");
        }

        // Find the assistant message in output
        var assistantOutput = apiResponse.Output.FirstOrDefault(o => o.Role == "assistant");
        if (assistantOutput == null || assistantOutput.Content == null || assistantOutput.Content.Count == 0)
        {
            throw new Exception("No assistant message in response");
        }

        // Extract text from output_text content
        var textContent = assistantOutput.Content.FirstOrDefault(c => c.Type == "output_text");
        if (textContent == null)
        {
            throw new Exception("No text content in response");
        }

        var result = textContent.Text;
        _logger.LogInfo($">>> HTTP COMPARISON SUCCESS: {result.Length} characters <<<");

        // Log token usage
        if (apiResponse.Usage != null)
        {
            _logger.LogInfo($"Token usage - Input: {apiResponse.Usage.PromptTokens}, " +
             $"Output: {apiResponse.Usage.CompletionTokens}, " +
                          $"Total: {apiResponse.Usage.TotalTokens}");
        }

        return result;
    }

    #endregion

    #region Private Helper Methods - Logging

    private void LogRequestMetrics(string jsonPayload, List<byte[]> clientImages, List<byte[]> supplierImages)
    {
        _logger.LogInfo($"HTTP Payload size: {jsonPayload.Length / 1024.0:F2} KB");
        _logger.LogInfo($"Total images in payload: {clientImages.Count + supplierImages.Count}");
        _logger.LogInfo($"Using model: {DefaultModel}");
    }

    private void LogPromptToFile(List<byte[]> clientImages, List<byte[]> supplierImages)
    {
        _logger.LogInfo(">>> LOGGING COMPLETE PROMPT SENT TO CHATGPT (HTTP) <<<");

        var promptLog = new StringBuilder();
        promptLog.AppendLine("================================================================================");
        promptLog.AppendLine($"COMPLETE PROMPT SENT TO CHATGPT API (HTTP METHOD)");
        promptLog.AppendLine($"Model: {DefaultModel} | Max Tokens: {MaxOutputTokens} | Temperature: {DefaultTemperature}");
        promptLog.AppendLine("================================================================================");
        promptLog.AppendLine();
        promptLog.AppendLine("=== SYSTEM MESSAGE (Expert Role) ===");
        promptLog.AppendLine(GetSystemPrompt());
        promptLog.AppendLine();
        promptLog.AppendLine("=== USER MESSAGE ===");
        promptLog.AppendLine(GetUserPrompt());
        promptLog.AppendLine();
        promptLog.AppendLine("\n\n=== DOCUMENT CLIENT (RÉFÉRENCE MAÎTRE) ===\n");

        LogImagesMetadata(promptLog, clientImages, "CLIENT");

        promptLog.AppendLine("\n=== FIN DOCUMENT CLIENT ===\n");
        promptLog.AppendLine("\n=== DOCUMENT FOURNISSEUR ===\n");

        LogImagesMetadata(promptLog, supplierImages, "FOURNISSEUR");

        promptLog.AppendLine("\n=== FIN DOCUMENT FOURNISSEUR ===\n");
        promptLog.AppendLine();
        promptLog.AppendLine("================================================================================");
        promptLog.AppendLine($"Total images sent: {clientImages.Count + supplierImages.Count}");
        promptLog.AppendLine($"Client images: {clientImages.Count}");
        promptLog.AppendLine($"Supplier images: {supplierImages.Count}");
        promptLog.AppendLine($"Total payload size: {(clientImages.Sum(i => i.Length) + supplierImages.Sum(i => i.Length)) / 1024.0:F2} KB");
        promptLog.AppendLine("================================================================================");

        _logger.LogChatGptResponse("PROMPT_SENT_HTTP", promptLog.ToString());
        _logger.LogInfo(">>> PROMPT LOGGED TO CHATGPT LOG FILE <<<");
    }

    private void LogImagesMetadata(StringBuilder log, List<byte[]> images, string documentType)
    {
        for (int i = 0; i < images.Count; i++)
        {
            log.AppendLine($"\n--- {documentType} PAGE {i + 1}/{images.Count} ---");
            log.AppendLine($"[IMAGE: {documentType} image {i + 1} - {images[i].Length / 1024.0:F2} KB - Base64 encoded]");
        }
    }

    #endregion

    #region Private Helper Methods - Prompts

    private string GetSystemPrompt()
    {
        return @"Tu es un expert senior en qualité et ingénierie technique spécialisé dans les revues de conformité technique indépendantes.

Tu possèdes une expertise approfondie dans l'analyse comparative de documents techniques industriels, incluant les plans mécaniques, spécifications dimensionnelles, exigences matériaux, et normes de qualité.

Tu travailles avec rigueur et précision, en te basant uniquement sur les informations présentes dans les documents fournis.";
    }

    private string GetUserPrompt()
    {
        return @"CONTEXTE :

Deux documents techniques sont fournis :

1) Document CLIENT (référence maître, exigences contractuelles)
2) Document FOURNISSEUR (document dérivé, exécution ou fabrication)

OBJECTIF :

Réaliser une revue complète de conformité du document fournisseur par rapport au document client.

RÈGLES GÉNÉRALES :

- Le document client est la référence maître absolue
- Évaluer les exigences techniques, dimensionnelles, fonctionnelles, de sécurité et qualité
- Utiliser une terminologie technique professionnelle applicable à tout secteur industriel
- Te baser UNIQUEMENT sur le contenu des documents fournis
- Les images fournies sont des scans haute résolution des documents techniques originaux

NIVEAU DE DÉTAIL ATTENDU (TRÈS IMPORTANT) :

- Aller dans le DÉTAIL POUR CHAQUE PIÈCE / REPÈRE des plans
- Pour chaque pièce, détailler au minimum :
 - Nom / repère / identifiant de la pièce
 - Toutes les cotes importantes visibles (dimensions principales, épaisseurs, diamètres, entraxes, rayons, etc.)
 - Toutes les tolérances associées (dimensionnelles, géométriques, état de surface)
 - Les matériaux et traitements associés à la pièce
 - Les fonctions principales de la pièce (portante, guidage, étanchéité, liaison, sécurité, etc.)
 - Les interfaces de la pièce avec les autres pièces (ajustements, jeux, serrages, type d’assemblage)
 - Les notes spécifiques ou remarques techniques associées à la pièce
- Comparer pièce par pièce : pour chaque repère, indiquer clairement les différences entre Client et Fournisseur
- Si une pièce existe dans un document et pas dans l’autre, le signaler explicitement comme non conforme
- Si des informations sont manquantes pour une pièce (matière, traitement, cote critique, tolérance), les signaler explicitement

FORMAT DE SORTIE STRICT (OBLIGATOIRE) :

- Sortie sous forme de TABLEAUX UNIQUEMENT
- AUCUN texte explicatif, résumé ou conclusion
- AUCUNE liste à puces
- AUCUN emoji
- Chaque tableau doit comporter EXACTEMENT3 colonnes : | Client | Fournisseur | Statut |
- Le champ ""Statut"" doit contenir uniquement : ""Conforme"" ou ""Non conforme""

STRUCTURE DE SORTIE OBLIGATOIRE (UN TABLEAU PAR SECTION) :

1) Références générales et périmètre
2) Nomenclature / liste des pièces (inclure un niveau de détail par pièce : repère, désignation, quantité, version, indice de révision)
3) Matériaux
4) Traitements thermiques et traitements de surface
5) Tolérances dimensionnelles et états de surface
6) Exigences fonctionnelles et mécaniques
7) Exigences d'essais et de validation
8) Sécurité, avertissements et marquages réglementaires
9) Identification, marquage et traçabilité
10) Conformité réglementaire et documentation obligatoire
11) Conditionnement, manutention et protection
12) Documentation qualité et livrables
13) Vérification dimensionnelle de chaque repère
 - vérification de toutes les cotes par pièce
 - cotes critiques et fonctionnelles
 - interfaces et ajustements
 - méthodes de mesure
 - rapports de contrôle dimensionnel
 - conformité dimensionnelle avant essais ou mise en service

RÈGLES D'ÉVALUATION :

- Si l'exigence fournisseur correspond totalement à l'exigence client → ""Conforme""
- Si une information est manquante, ambiguë ou différente → ""Non conforme""
- En cas de différence d'unités (métrique/impérial), indiquer ""Conforme"" uniquement si l'équivalence technique est démontrée
- Reprendre la terminologie exacte des documents lorsque possible

EXIGENCES FINALES :

- Tableaux uniquement
- Mise en forme claire et cohérente
- Aucun texte avant ou après les tableaux";
    }

    #endregion
}
#region OpenAI API Models

public class OpenAiChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "gpt-4.1";

    [JsonPropertyName("input")]
    public object[]? Input { get; set; }

    [JsonPropertyName("max_output_tokens")]
    public int MaxTokens { get; set; } = 12000;

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.1f;
}

public class OpenAiChatResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("output")]
    public List<ResponseOutput> Output { get; set; } = new();

    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }
}

public class ResponseOutput
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public List<ResponseContent> Content { get; set; } = new();
}

public class ResponseContent
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("annotations")]
    public List<object> Annotations { get; set; } = new();
}

public class Usage
{
    [JsonPropertyName("input_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}

#endregion

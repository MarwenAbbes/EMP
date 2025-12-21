using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;

namespace Mira.Core.Services;

public interface IChatGptService
{
    /// <summary>
    /// Sends a message to ChatGPT and returns the response
    /// </summary>
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes a PDF file by uploading it to OpenAI
    /// </summary>
    Task<string> AnalyzePdfFileAsync(string pdfFilePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares two PDF documents (client vs supplier) and generates a conformity report
    /// </summary>
    /// <param name="clientPdfPath">Path to the client PDF file</param>
    /// <param name="supplierPdfPath">Path to the supplier PDF file</param>
    /// <param name="progressCallback">Optional callback to report progress</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<string> ComparePdfDocumentsAsync(string clientPdfPath, string supplierPdfPath, 
      IProgress<string>? progressCallback = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the connection to the ChatGPT API
    /// </summary>
    Task<bool> TestConnectionAsync();

    /// <summary>
    /// Sets the API key for ChatGPT
    /// </summary>
    void SetApiKey(string apiKey);

    /// <summary>
    /// Gets whether an API key is configured
    /// </summary>
    bool IsConfigured { get; }
    
    /// <summary>
    /// Sets the output directory where images should be saved
    /// </summary>
    void SetOutputDirectory(string outputDirectory);
}

public class ChatGptService : IChatGptService
{
    private string? _apiKey;
    private OpenAIClient? _openAiClient;
    private ChatClient? _chatClient;
    private OpenAIFileClient? _fileClient;
    private readonly ILoggerService _logger;
    private readonly IPdfImageService _pdfImageService;
    private readonly ChatGptHttpService _httpService;
    private string? _outputDirectory;
    private const string DefaultModel = "gpt-4o"; // Upgraded to latest GPT-4o

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    public ChatGptService(ILoggerService logger)
    {
        _logger = logger;
        _pdfImageService = new PdfImageService(logger);
        _httpService = new ChatGptHttpService(logger);
        _logger.LogInfo("ChatGPT service initialized WITH PDF IMAGE SERVICE and HTTP SERVICE");
    }

    public void SetApiKey(string apiKey)
    {
        _logger.LogDebug("Setting API key");
        _apiKey = apiKey;
        _openAiClient = new OpenAIClient(apiKey);
        _chatClient = _openAiClient.GetChatClient(DefaultModel);
        _fileClient = _openAiClient.GetOpenAIFileClient();
  
        // Also set API key for HTTP service
        _httpService.SetApiKey(apiKey);
        
        _logger.LogInfo($"API key configured. Model: {DefaultModel}");
    }
    
    public void SetOutputDirectory(string outputDirectory)
    {
        _logger.LogInfo($"Setting output directory for images: {outputDirectory}");
     _outputDirectory = outputDirectory;
    
        // Ensure the images subdirectory exists
        if (!string.IsNullOrEmpty(_outputDirectory))
  {
    string imagesDir = Path.Combine(_outputDirectory, "Images");
  if (!Directory.Exists(imagesDir))
    {
     Directory.CreateDirectory(imagesDir);
     _logger.LogInfo($"Created images directory: {imagesDir}");
    }
        }
  
        // Set output directory for HTTP service as well
        _httpService.SetOutputDirectory(outputDirectory);
    }

    /// <summary>
    /// Saves images to disk in the configured output directory
    /// </summary>
    private void SaveImages(List<byte[]> images, string prefix)
    {
        if (string.IsNullOrEmpty(_outputDirectory))
        {
            _logger.LogWarning("Output directory not set, skipping image save");
            return;
        }

        string imagesDir = Path.Combine(_outputDirectory, "Images");
        _logger.LogInfo($"Saving {images.Count} images with prefix '{prefix}' to: {imagesDir}");

        for (int i = 0; i < images.Count; i++)
        {
            string fileName = $"{prefix}_page_{i + 1:D3}.png";
            string filePath = Path.Combine(imagesDir, fileName);
            
            try
            {
                File.WriteAllBytes(filePath, images[i]);
                _logger.LogDebug($"Saved image: {fileName} ({images[i].Length / 1024.0:F2} KB)");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save image {fileName}", ex);
            }
        }
        
        _logger.LogInfo($"Successfully saved {images.Count} images");
    }

    public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"Sending message (length: {message.Length} chars)");
        
        if (!IsConfigured || _chatClient == null)
        {
            _logger.LogError("Attempted to send message without configured API key");
            throw new InvalidOperationException("ChatGPT API key is not configured. Please set your API key first.");
        }

        try
        {
            var messages = new List<ChatMessage>
            {
                new UserChatMessage(message)
            };

            _logger.LogInfo("Calling ChatGPT API");
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

            string response = completion.Content[0].Text;
            _logger.LogInfo($"Received response (length: {response.Length} chars)");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to communicate with ChatGPT API", ex);
            throw new Exception($"Error communicating with ChatGPT: {ex.Message}", ex);
        }
    }

    public async Task<string> AnalyzePdfFileAsync(string pdfFilePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInfo($"=== ANALYZING PDF WITH IMAGES: {pdfFilePath} ===");
        
        if (!IsConfigured || _chatClient == null)
        {
            _logger.LogError("Attempted to analyze PDF without configured API key");
            throw new InvalidOperationException("ChatGPT API key is not configured. Please set your API key first.");
        }

        if (!File.Exists(pdfFilePath))
        {
            _logger.LogError($"PDF file not found: {pdfFilePath}");
            throw new FileNotFoundException($"PDF file not found: {pdfFilePath}");
        }

        try
        {
    // CONVERT PDF TO IMAGES
       _logger.LogInfo(">>> CONVERTING PDF TO IMAGES <<<");
            var imageByteArrays = _pdfImageService.ConvertPdfToImages(pdfFilePath);
_logger.LogInfo($">>> CONVERTED TO {imageByteArrays.Count} IMAGES <<<");

            // Save ALL images to disk BEFORE sending
    string pdfFileName = Path.GetFileNameWithoutExtension(pdfFilePath);
   SaveImages(imageByteArrays, pdfFileName);
            _logger.LogInfo(">>> ALL IMAGES SAVED TO DISK <<<");

 // Take first 5 pages to stay within token limits
    var pagesToAnalyze = imageByteArrays.Take(5).ToList();
            
 // BUILD MESSAGE WITH TEXT + IMAGES
      var contentParts = new List<ChatMessageContentPart>
            {
            ChatMessageContentPart.CreateTextPart(
     "Analyze this technical PDF document. Provide a summary in EXACTLY 2 sentences. " +
     "Focus on the main technical content, key details, diagrams, images, and important information.")
            };

 // ADD IMAGES
  foreach (var imageBytes in pagesToAnalyze)
            {
    BinaryData imageData = BinaryData.FromBytes(imageBytes);
       contentParts.Add(ChatMessageContentPart.CreateImagePart(imageData, "image/png"));
     _logger.LogInfo($">>> ADDED IMAGE: {imageBytes.Length / 1024.0:F2} KB <<<");
         }

        var messages = new List<ChatMessage>
            {
        new UserChatMessage(contentParts)
            };

            _logger.LogInfo($">>> SENDING {pagesToAnalyze.Count} IMAGES TO CHATGPT <<<");
          ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

            string response = completion.Content[0].Text;
            _logger.LogInfo($">>> PDF ANALYSIS SUCCESS: {response} <<<");
            _logger.LogDebug($"Response: {response}");

       return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($">>> PDF ANALYSIS FAILED: {ex.Message} <<<", ex);
            throw new Exception($"Error analyzing PDF with ChatGPT: {ex.Message}", ex);
  }
    }

    public async Task<string> ComparePdfDocumentsAsync(string clientPdfPath, string supplierPdfPath, 
  IProgress<string>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInfo($"=== COMPARING TWO PDF DOCUMENTS (HTTP METHOD) ===");
        _logger.LogInfo($"Client PDF: {clientPdfPath}");
        _logger.LogInfo($"Supplier PDF: {supplierPdfPath}");
     
        progressCallback?.Report("Starting HTTP-based PDF comparison...");
     
        if (!IsConfigured)
        {
_logger.LogError("Attempted to compare PDFs without configured API key");
        throw new InvalidOperationException("ChatGPT API key is not configured. Please set your API key first.");
        }

        if (!File.Exists(clientPdfPath))
      {
   _logger.LogError($"Client PDF file not found: {clientPdfPath}");
            throw new FileNotFoundException($"Client PDF file not found: {clientPdfPath}");
        }

        if (!File.Exists(supplierPdfPath))
   {
            _logger.LogError($"Supplier PDF file not found: {supplierPdfPath}");
     throw new FileNotFoundException($"Supplier PDF file not found: {supplierPdfPath}");
        }

   try
 {
    // CONVERT BOTH PDFs TO IMAGES
            progressCallback?.Report("Converting Client PDF to images...");
  _logger.LogInfo(">>> CONVERTING CLIENT PDF TO IMAGES <<<");
            var clientImages = _pdfImageService.ConvertPdfToImages(clientPdfPath);
     _logger.LogInfo($">>> CLIENT CONVERTED TO {clientImages.Count} IMAGES <<<");

      progressCallback?.Report("Converting Supplier PDF to images...");
          _logger.LogInfo(">>> CONVERTING SUPPLIER PDF TO IMAGES <<<");
            var supplierImages = _pdfImageService.ConvertPdfToImages(supplierPdfPath);
       _logger.LogInfo($">>> SUPPLIER CONVERTED TO {supplierImages.Count} IMAGES <<<");

            // Save ALL images to disk BEFORE sending to API
   progressCallback?.Report("Saving images to disk...");
 string clientFileName = Path.GetFileNameWithoutExtension(clientPdfPath);
  string supplierFileName = Path.GetFileNameWithoutExtension(supplierPdfPath);
     _logger.LogInfo(">>> SAVING ALL IMAGES TO DISK BEFORE API CALL <<<");
       SaveImages(clientImages, $"Client_{clientFileName}");
            SaveImages(supplierImages, $"Supplier_{supplierFileName}");
            _logger.LogInfo(">>> ALL IMAGES SAVED TO DISK <<<");

            _logger.LogInfo($">>> USING HTTP SERVICE FOR COMPARISON <<<");
            _logger.LogInfo($">>> SENDING ALL {clientImages.Count} CLIENT PAGES <<<");
            _logger.LogInfo($">>> SENDING ALL {supplierImages.Count} SUPPLIER PAGES <<<");
    
            // Use HTTP service for the actual comparison
    string response = await _httpService.ComparePdfDocumentsViaHttpAsync(
         clientImages, 
       supplierImages, 
                progressCallback, 
      cancellationToken);

       _logger.LogInfo($">>> HTTP COMPARISON SUCCESS <<<");
      _logger.LogInfo($"Response received: {response.Length} characters");
     
            progressCallback?.Report("Comparison completed successfully!");

 return response;
        }
        catch (OperationCanceledException)
      {
  _logger.LogError(">>> PDF COMPARISON TIMEOUT - Operation exceeded 10 minutes <<<");
   progressCallback?.Report("Comparison timed out after 10 minutes");
      throw new TimeoutException("The comparison operation timed out after 10 minutes. The documents may be too large or complex.");
        }
 catch (Exception ex)
        {
   _logger.LogError($">>> PDF COMPARISON FAILED: {ex.Message} <<<", ex);
     progressCallback?.Report($"Comparison failed: {ex.Message}");
            throw new Exception($"Error comparing PDFs with ChatGPT: {ex.Message}", ex);
        }
    }
    public async Task<bool> TestConnectionAsync()
    {
        _logger.LogInfo("Testing connection to ChatGPT API");
        
        if (!IsConfigured)
        {
            _logger.LogWarning("Connection test skipped - API key not configured");
            return false;
        }

        try
        {
            var response = await SendMessageAsync("Hello, this is a test. Please respond with 'OK'.");
            bool success = !string.IsNullOrEmpty(response);
            
            if (success)
            {
                _logger.LogInfo("Connection test successful");
            }
            else
            {
                _logger.LogWarning("Connection test returned empty response");
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError("Connection test failed", ex);
            return false;
        }
    }
}

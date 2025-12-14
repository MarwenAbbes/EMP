using OpenAI;
using OpenAI.Chat;
using OpenAI.Files;

namespace Mira.Core.Services;

public interface IChatGptService
{
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task<string> AnalyzePdfFileAsync(string pdfFilePath, CancellationToken cancellationToken = default);
    Task<bool> TestConnectionAsync();
    void SetApiKey(string apiKey);
    bool IsConfigured { get; }
}

public class ChatGptService : IChatGptService
{
    private string? _apiKey;
    private OpenAIClient? _openAiClient;
    private ChatClient? _chatClient;
    private OpenAIFileClient? _fileClient;
    private readonly ILoggerService _logger;
    private readonly IPdfImageService _pdfImageService;
    private const string DefaultModel = "gpt-4o-mini";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    public ChatGptService(ILoggerService logger)
    {
        _logger = logger;
        _pdfImageService = new PdfImageService(logger);
        _logger.LogInfo("ChatGPT service initialized WITH PDF IMAGE SERVICE");
    }

    public void SetApiKey(string apiKey)
    {
        _logger.LogDebug("Setting API key");
        _apiKey = apiKey;
        _openAiClient = new OpenAIClient(apiKey);
        _chatClient = _openAiClient.GetChatClient(DefaultModel);
        _fileClient = _openAiClient.GetOpenAIFileClient();
        _logger.LogInfo($"API key configured. Model: {DefaultModel}");
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

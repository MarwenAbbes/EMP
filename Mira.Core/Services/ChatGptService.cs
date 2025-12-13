using OpenAI;
using OpenAI.Chat;

namespace Mira.Core.Services;

public interface IChatGptService
{
    /// <summary>
    /// Sends a message to ChatGPT and returns the response
    /// </summary>
    /// <param name="message">The user message to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ChatGPT response</returns>
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tests the connection to the ChatGPT API
    /// </summary>
    /// <returns>True if connection is successful</returns>
    Task<bool> TestConnectionAsync();

    /// <summary>
    /// Sets the API key for ChatGPT
    /// </summary>
    void SetApiKey(string apiKey);

    /// <summary>
    /// Gets whether an API key is configured
    /// </summary>
    bool IsConfigured { get; }
}

public class ChatGptService : IChatGptService
{
    private string? _apiKey;
    private ChatClient? _chatClient;
    private const string DefaultModel = "gpt-4o-mini";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        _chatClient = new ChatClient(DefaultModel, apiKey);
    }

    public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured || _chatClient == null)
        {
            throw new InvalidOperationException("ChatGPT API key is not configured. Please set your API key first.");
        }

        try
        {
            var messages = new List<ChatMessage>
            {
                new UserChatMessage(message)
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error communicating with ChatGPT: {ex.Message}", ex);
        }
    }

    public async Task<bool> TestConnectionAsync()
    {
        if (!IsConfigured)
        {
            return false;
        }

        try
        {
            var response = await SendMessageAsync("Hello, this is a test. Please respond with 'OK'.");
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }
}


using Serilog;
using Serilog.Events;

namespace Mira.Core.Services;

public enum LogSeverity
{
    Debug,      // Detailed information for debugging
    Info,       // General information about application flow
    Warning,    // Potentially harmful situations
    Error,      // Error events that might still allow the app to continue
    Critical    // Critical failures that require immediate attention
}

public interface ILoggerService
{
    /// <summary>
    /// Logs a message with specified severity
    /// </summary>
    void Log(LogSeverity severity, string message, Exception? exception = null);

    /// <summary>
    /// Logs debug information
    /// </summary>
    void LogDebug(string message);

    /// <summary>
    /// Logs informational message
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    /// Logs warning message
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    /// Logs error message
    /// </summary>
    void LogError(string message, Exception? exception = null);

    /// <summary>
    /// Logs critical failure
    /// </summary>
    void LogCritical(string message, Exception? exception = null);

    /// <summary>
    /// Logs a ChatGPT response to a dedicated file
    /// </summary>
    void LogChatGptResponse(string fileName, string response);

    /// <summary>
    /// Gets the log file path
    /// </summary>
    string GetLogFilePath();
}

public class SerilogLoggerService : ILoggerService
{
    private readonly ILogger _mainLogger;
    private readonly ILogger _chatGptLogger;
    private readonly string _logDirectory;
    private readonly string _chatGptLogFileName;

    public SerilogLoggerService()
    {
        _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        _chatGptLogFileName = $"ChatGPT_{DateTime.Now:yyyyMMdd}.log";

        // Ensure log directory exists
        Directory.CreateDirectory(_logDirectory);

        // Configure main application logger
        _mainLogger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: Path.Combine(_logDirectory, $"Application_.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u8}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30,
                fileSizeLimitBytes: 10485760, // 10MB
                rollOnFileSizeLimit: true,
                shared: true) // Enable shared file access
            .CreateLogger();

        // Configure ChatGPT logger (separate file)
        _chatGptLogger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                path: Path.Combine(_logDirectory, _chatGptLogFileName),
                outputTemplate: "{Message:lj}{NewLine}",
                shared: true) // Enable shared file access
            .CreateLogger();

        LogInfo("Serilog logging system initialized");
    }

    public void Log(LogSeverity severity, string message, Exception? exception = null)
    {
        var logEvent = ConvertToSerilogLevel(severity);

        if (exception != null)
        {
            _mainLogger.Write(logEvent, exception, message);
        }
        else
        {
            _mainLogger.Write(logEvent, message);
        }
    }

    public void LogDebug(string message)
        => _mainLogger.Debug(message);

    public void LogInfo(string message)
        => _mainLogger.Information(message);

    public void LogWarning(string message)
        => _mainLogger.Warning(message);

    public void LogError(string message, Exception? exception = null)
    {
        if (exception != null)
            _mainLogger.Error(exception, message);
        else
            _mainLogger.Error(message);
    }

    public void LogCritical(string message, Exception? exception = null)
    {
        if (exception != null)
            _mainLogger.Fatal(exception, message);
        else
            _mainLogger.Fatal(message);
    }

    public void LogChatGptResponse(string fileName, string response)
    {
        try
        {
            string logEntry = $@"
================================================================================
Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
File: {fileName}
ChatGPT Response:
{response}
================================================================================
";

            _chatGptLogger.Information(logEntry);
            LogInfo($"Response logged for file: {fileName}");
        }
        catch (Exception ex)
        {
            LogError("Failed to log ChatGPT response", ex);
        }
    }

    public string GetLogFilePath()
    {
        return Path.Combine(_logDirectory, _chatGptLogFileName);
    }

    private LogEventLevel ConvertToSerilogLevel(LogSeverity severity)
    {
        return severity switch
        {
            LogSeverity.Debug => LogEventLevel.Debug,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }

    // Ensure logs are flushed on disposal
    ~SerilogLoggerService()
    {
        Serilog.Log.CloseAndFlush();
    }
}

// Alias for easier migration
public class LoggerService : SerilogLoggerService
{
}

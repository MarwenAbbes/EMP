using Serilog;

namespace Mira.UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Configure Serilog as early as possible
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Application_.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u8}] {Message:lj}{NewLine}{Exception}",
                shared: true) // Enable shared file access
            .CreateLogger();

        try
        {
            Log.Information("========== Application Starting ==========");
            Log.Information("Mira Technical Review Application");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            
            Log.Information("Running main form");
            Application.Run(new FHome());
            
            Log.Information("Application closing normally");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.Information("========== Application Shutdown ==========");
            Log.CloseAndFlush();
        }
    }
}
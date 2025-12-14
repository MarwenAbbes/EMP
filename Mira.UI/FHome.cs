using Mira.Core;
using Mira.Core.DTO;
using Mira.Core.Services;
using System.Text.Json;

namespace Mira.UI;

public partial class FHome : Form
{
    private ComparisonDto comparisonDto = null;
    private IFileImportService _fileImportService;
    private IChatGptService _chatGptService;
    private ILoggerService _loggerService;
    private const string SecretsFileName = "appsettings.local.json";

    // Dictionary to map ReportType to UI components and properties
    private readonly Dictionary<Enums.ReportType, (Label statusLabel, string propertyName)> _reportTypeMapping = new()
    {
        { Enums.ReportType.Client, (null, nameof(ComparisonDto.ClientPlantPath)) },
        { Enums.ReportType.EMP, (null, nameof(ComparisonDto.EmpPlanPath)) }
    };

    public FHome()
    {
        InitializeComponent();
        
        // Initialize logger first
        _loggerService = new LoggerService();
        _loggerService.LogInfo("========== Application Starting ==========");
        _loggerService.LogInfo("Initializing main form");
        
        // Initialize services with logger
        _fileImportService = new FileImportService(_loggerService);
        _chatGptService = new ChatGptService(_loggerService);
        
        _loggerService.LogDebug("All services initialized");
        
        InitializeReportTypeMapping();
        InitializeUi();
        LoadChatGptApiKey();
        
        _loggerService.LogInfo("Main form initialization completed");
    }

    /// <summary>
    /// Maps report types to their corresponding UI labels
    /// </summary>
    private void InitializeReportTypeMapping()
    {
        _loggerService.LogDebug("Initializing report type mapping");
        _reportTypeMapping[Enums.ReportType.Client] = (clientPlanStatusValueLabel, nameof(ComparisonDto.ClientPlantPath));
        _reportTypeMapping[Enums.ReportType.EMP] = (empPlanStatusValueLabel, nameof(ComparisonDto.EmpPlanPath));
    }

    private void InitializeUi()
    {
        _loggerService.LogDebug("Initializing UI components");
        saveComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        saveAsComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        deleteComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        reviewToolStripMenuItem.Enabled = comparisonDto != null;
        exportToolStripMenuItem.Enabled = comparisonDto != null;
        comparisonContainerGroupBox.Visible = comparisonDto != null;
        comparisonContainerGroupBox.Text = comparisonDto != null ? comparisonDto.Id: string.Empty ;
        UpdateAllStatusLabels();
        
        _loggerService.LogInfo($"UI initialized. Comparison loaded: {comparisonDto != null}");
    }

    /// <summary>
    /// Loads the ChatGPT API key from secrets file and tests connection
    /// </summary>
    private async void LoadChatGptApiKey()
    {
        _loggerService.LogInfo("Loading ChatGPT API key from secrets");
        
        try
        {
            string secretsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SecretsFileName);
            _loggerService.LogDebug($"Secrets path: {secretsPath}");
            
            if (File.Exists(secretsPath))
            {
                _loggerService.LogInfo("Secrets file found, reading API key");
                string json = File.ReadAllText(secretsPath);
                using JsonDocument doc = JsonDocument.Parse(json);
                
                if (doc.RootElement.TryGetProperty("OpenAI", out JsonElement openAiElement) &&
                    openAiElement.TryGetProperty("ApiKey", out JsonElement apiKeyElement))
                {
                    string? apiKey = apiKeyElement.GetString();
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        _loggerService.LogInfo("API key found in secrets file");
                        statusStripLabel.Text = "ChatGPT: Connecting...";
                        statusStripLabel.ForeColor = Color.Orange;
                        
                        _chatGptService.SetApiKey(apiKey);
                        bool isConnected = await _chatGptService.TestConnectionAsync();

                        if (isConnected)
                        {
                            statusStripLabel.Text = "ChatGPT: Connected (GPT-4o mini) ✓";
                            statusStripLabel.ForeColor = Color.Green;
                            _loggerService.LogInfo("ChatGPT connection test successful");
                        }
                        else
                        {
                            statusStripLabel.Text = "ChatGPT: Connection failed ✗";
                            statusStripLabel.ForeColor = Color.Red;
                            _loggerService.LogWarning("ChatGPT connection test failed");
                        }
                        return;
                    }
                }
            }
            
            _loggerService.LogWarning("No API key found in secrets file");
            statusStripLabel.Text = "ChatGPT: Not configured";
            statusStripLabel.ForeColor = Color.Gray;
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Failed to load ChatGPT API key", ex);
            statusStripLabel.Text = $"ChatGPT: Error - {ex.Message}";
            statusStripLabel.ForeColor = Color.Red;
        }
    }

    /// <summary>
    /// Updates all plan status labels based on the current comparison state
    /// </summary>
    private void UpdateAllStatusLabels()
    {
        _loggerService.LogDebug("Updating all status labels");
        if (comparisonDto != null)
        {
            UpdateStatusLabel(Enums.ReportType.Client, comparisonDto.ClientPlanLoaded);
            UpdateStatusLabel(Enums.ReportType.EMP, comparisonDto.EmpPlanLoaded);
        }
    }

    /// <summary>
    /// Updates the status label and cursor for a specific report type
    /// </summary>
    private void UpdateStatusLabel(Enums.ReportType reportType, bool isLoaded)
    {
        _loggerService.LogDebug($"Updating status label for {reportType}: {(isLoaded ? "Loaded" : "Not Loaded")}");
        
        if (_reportTypeMapping.TryGetValue(reportType, out var mapping))
        {
            var (statusLabel, _) = mapping;
            statusLabel.Text = isLoaded ? "Loaded" : "Not Loaded";
            statusLabel.ForeColor = isLoaded ? Color.Green : Color.Red;
            statusLabel.Cursor = isLoaded ? Cursors.Hand : Cursors.Default;
        }
    }

    /// <summary>
    /// Handles import operations for both Client and EMP plans
    /// </summary>
    private async void HandleImportFile(Enums.ReportType reportType)
    {
        _loggerService.LogInfo($"Starting import for {reportType} plan");
        
        if (comparisonDto == null)
        {
            _loggerService.LogWarning("Import attempted without active comparison");
            return;
        }

        string? importedFileName = _fileImportService.ImportFile(reportType, comparisonDto.BaseReportDirectory);

        if (importedFileName != null)
        {
            _loggerService.LogInfo($"File imported: {importedFileName}");
            
            // Update the comparison DTO with the imported file information
            if (reportType == Enums.ReportType.Client)
            {
                _loggerService.LogInfo("Processing Client plan");
                comparisonDto.ClientPlantPath = importedFileName;
                comparisonDto.ClientPlanLoaded = true;
            }
            else if (reportType == Enums.ReportType.EMP)
            {
                _loggerService.LogInfo("Processing EMP plan");
                comparisonDto.EmpPlanPath = importedFileName;
                comparisonDto.EmpPlanLoaded = true;
            }

            // Update UI
            UpdateStatusLabel(reportType, true);

            _loggerService.LogInfo($"{reportType} plan import completed successfully");
           
        }
        else
        {
            _loggerService.LogInfo("Import cancelled or failed");
        }
    }

    /// <summary>
    /// Analyzes the client plan PDF with ChatGPT by uploading the PDF file directly
    /// </summary>
    private async Task AnalyzeClientPlanWithChatGpt(string fileName)
    {
        _loggerService.LogInfo($"Starting ChatGPT PDF analysis for: {fileName}");
        
        if (!_chatGptService.IsConfigured || comparisonDto == null)
        {
            _loggerService.LogWarning("ChatGPT not configured or no active comparison");
            return;
        }

        try
        {
            statusStripLabel.Text = "ChatGPT: Uploading PDF file...";
            statusStripLabel.ForeColor = Color.Orange;

            // Get full file path
            string filePath = Path.Combine(comparisonDto.BaseReportDirectory, fileName);
            _loggerService.LogDebug($"Full file path: {filePath}");

            statusStripLabel.Text = "ChatGPT: Analyzing PDF...";

            // Analyze PDF file directly (uploads to OpenAI Files API)
            _loggerService.LogInfo("Uploading and analyzing PDF with ChatGPT");
            string chatGptResponse = await _chatGptService.AnalyzePdfFileAsync(filePath);

            // Log the response
            _loggerService.LogInfo("Logging ChatGPT response");
            _loggerService.LogChatGptResponse(fileName, chatGptResponse);

            // Update status
            statusStripLabel.Text = $"ChatGPT: Analysis complete - Logged to {Path.GetFileName(_loggerService.GetLogFilePath())}";
            statusStripLabel.ForeColor = Color.Green;

            _loggerService.LogInfo("ChatGPT PDF analysis completed successfully");

            // Show response in message box
            MessageBox.Show(
                $"ChatGPT Analysis:\n\n{chatGptResponse}\n\nResponse logged to:\n{_loggerService.GetLogFilePath()}",
                "Client Plan Analysis",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        catch (Exception ex)
        {
            _loggerService.LogError("ChatGPT PDF analysis failed", ex);
            statusStripLabel.Text = $"ChatGPT: Analysis failed - {ex.Message}";
            statusStripLabel.ForeColor = Color.Red;

            MessageBox.Show(
                $"Failed to analyze client plan:\n{ex.Message}",
                "Analysis Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void importClientPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked import Client plan menu item");
        HandleImportFile(Enums.ReportType.Client);
    }

    private void importEmpPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked import EMP plan menu item");
        HandleImportFile(Enums.ReportType.EMP);
    }

    /// <summary>
    /// Opens the specified plan file
    /// </summary>
    private void OpenPlanFile(string fileName)
    {
        _loggerService.LogInfo($"User attempting to open plan file: {fileName}");
        
        if (string.IsNullOrEmpty(fileName) || comparisonDto == null)
        {
            _loggerService.LogWarning("Cannot open file - invalid filename or no active comparison");
            return;
        }

        string filePath = Path.Combine(comparisonDto.BaseReportDirectory, fileName);
        _loggerService.LogDebug($"Full file path: {filePath}");

        if (File.Exists(filePath))
        {
            _loggerService.LogInfo("Opening file with default application");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        else
        {
            _loggerService.LogError($"File not found: {filePath}");
            MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void clientPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked client plan status label");
        if (comparisonDto?.ClientPlanLoaded == true)
        {
            OpenPlanFile(comparisonDto.ClientPlantPath);
        }
        else
        {
            _loggerService.LogDebug("Client plan not loaded");
        }
    }

    private void empPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked EMP plan status label");
        if (comparisonDto?.EmpPlanLoaded == true)
        {
            OpenPlanFile(comparisonDto.EmpPlanPath);
        }
        else
        {
            _loggerService.LogDebug("EMP plan not loaded");
        }
    }

    /// <summary>
    /// Handles creation of a new comparison
    /// </summary>
    private void newComparisonToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked new comparison menu item");
        comparisonDto = new ComparisonDto();
        _loggerService.LogInfo($"New comparison created: {comparisonDto.Id}");
        InitializeUi();
    }

    /// <summary>
    /// Handles compare button click - analyzes both plans with ChatGPT
    /// </summary>
    private async void compareButton_Click(object sender, EventArgs e)
    {
        _loggerService.LogInfo("User clicked Compare button");

        if (comparisonDto == null)
        {
            _loggerService.LogWarning("Compare attempted without active comparison");
            MessageBox.Show(
                "Please create a new comparison first.",
                "No Comparison",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (!comparisonDto.ClientPlanLoaded)
        {
            _loggerService.LogWarning("Compare attempted without Client plan loaded");
            MessageBox.Show(
                "Please import the Client plan first.",
                "Client Plan Missing",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (!_chatGptService.IsConfigured)
        {
            _loggerService.LogWarning("Compare attempted without ChatGPT configured");
            MessageBox.Show(
                "ChatGPT is not configured. Please check your API key.",
                "ChatGPT Not Configured",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        // Disable button during processing
        compareButton.Enabled = false;
        compareButton.Text = "Analyzing...";

        try
        {
            _loggerService.LogInfo("Starting comparison analysis");

            // Analyze Client Plan
            await AnalyzeClientPlanWithChatGpt(comparisonDto.ClientPlantPath);

            // If EMP plan is also loaded, analyze it
            if (comparisonDto.EmpPlanLoaded)
            {
                _loggerService.LogInfo("Analyzing EMP plan as well");
                await AnalyzeEmpPlanWithChatGpt(comparisonDto.EmpPlanPath);
            }

            _loggerService.LogInfo("Comparison analysis completed");
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Comparison analysis failed", ex);
            MessageBox.Show(
                $"Comparison failed:\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            // Re-enable button
            compareButton.Enabled = true;
            compareButton.Text = "Comparer";
        }
    }

    /// <summary>
    /// Analyzes the EMP plan PDF with ChatGPT by uploading the PDF file directly
    /// </summary>
    private async Task AnalyzeEmpPlanWithChatGpt(string fileName)
    {
        _loggerService.LogInfo($"Starting ChatGPT PDF analysis for EMP plan: {fileName}");

        if (!_chatGptService.IsConfigured || comparisonDto == null)
        {
            _loggerService.LogWarning("ChatGPT not configured or no active comparison");
            return;
        }

        try
        {
            statusStripLabel.Text = "ChatGPT: Uploading EMP PDF...";
            statusStripLabel.ForeColor = Color.Orange;

            // Get full file path
            string filePath = Path.Combine(comparisonDto.BaseReportDirectory, fileName);
            _loggerService.LogDebug($"Full file path: {filePath}");

            statusStripLabel.Text = "ChatGPT: Analyzing EMP PDF...";

            // Analyze PDF file directly (uploads to OpenAI Files API)
            _loggerService.LogInfo("Uploading and analyzing PDF with ChatGPT");
            string chatGptResponse = await _chatGptService.AnalyzePdfFileAsync(filePath);

            // Log the response
            _loggerService.LogInfo("Logging ChatGPT response");
            _loggerService.LogChatGptResponse($"EMP_{fileName}", chatGptResponse);

            _loggerService.LogInfo("EMP plan ChatGPT PDF analysis completed successfully");
        }
        catch (Exception ex)
        {
            _loggerService.LogError("EMP plan ChatGPT PDF analysis failed", ex);
            throw;
        }
    }
}

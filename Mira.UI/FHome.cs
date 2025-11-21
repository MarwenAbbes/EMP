using Mira.Core;
using Mira.Core.DTO;
using Mira.Core.Services;

namespace Mira.UI;

public partial class FHome : Form
{
    private ComparisonDto comparisonDto = null;
    private IFileImportService _fileImportService;

    // Dictionary to map ReportType to UI components and properties
    private readonly Dictionary<Enums.ReportType, (Label statusLabel, string propertyName)> _reportTypeMapping = new()
    {
        { Enums.ReportType.Client, (null, nameof(ComparisonDto.ClientPlantPath)) },
        { Enums.ReportType.EMP, (null, nameof(ComparisonDto.EmpPlanPath)) }
    };

    public FHome()
    {
        InitializeComponent();
        _fileImportService = new FileImportService();
        InitializeReportTypeMapping();
        InitializeUi();
    }

    /// <summary>
    /// Maps report types to their corresponding UI labels
    /// </summary>
    private void InitializeReportTypeMapping()
    {
        _reportTypeMapping[Enums.ReportType.Client] = (clientPlanStatusValueLabel, nameof(ComparisonDto.ClientPlantPath));
        _reportTypeMapping[Enums.ReportType.EMP] = (empPlanStatusValueLabel, nameof(ComparisonDto.EmpPlanPath));
    }

    private void InitializeUi()
    {
        saveComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        saveAsComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        deleteComparisonToolStripMenuItem.Enabled = comparisonDto != null;
        reviewToolStripMenuItem.Enabled = comparisonDto != null;
        exportToolStripMenuItem.Enabled = comparisonDto != null;
        comparisonContainerGroupBox.Visible = comparisonDto != null;
        comparisonContainerGroupBox.Text = comparisonDto != null ? comparisonDto.Id: string.Empty ;
        UpdateAllStatusLabels();   
    }

    /// <summary>
    /// Updates all plan status labels based on the current comparison state
    /// </summary>
    private void UpdateAllStatusLabels()
    {
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
    private void HandleImportFile(Enums.ReportType reportType)
    {
        if (comparisonDto == null)
        {
            return;
        }

        string? importedFileName = _fileImportService.ImportFile(reportType, comparisonDto.BaseReportDirectory);

        if (importedFileName != null)
        {
            // Update the comparison DTO with the imported file information
            if (reportType == Enums.ReportType.Client)
            {
                comparisonDto.ClientPlantPath = importedFileName;
                comparisonDto.ClientPlanLoaded = true;
            }
            else if (reportType == Enums.ReportType.EMP)
            {
                comparisonDto.EmpPlanPath = importedFileName;
                comparisonDto.EmpPlanLoaded = true;
            }

            // Update UI
            UpdateStatusLabel(reportType, true);

            // Show success message
            MessageBox.Show(
                string.Format(Constants.REPORT_IMPORT_SUCCESS_MESSAGE, reportType),
                Constants.REPORT_IMPORT_SUCCESS_TITLE,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }

    private void importClientPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        HandleImportFile(Enums.ReportType.Client);
    }

    private void importEmpPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        HandleImportFile(Enums.ReportType.EMP);
    }

    /// <summary>
    /// Opens the specified plan file
    /// </summary>
    private void OpenPlanFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || comparisonDto == null)
        {
            return;
        }

        string filePath = Path.Combine(comparisonDto.BaseReportDirectory, fileName);

        if (File.Exists(filePath))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
        else
        {
            MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void clientPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        if (comparisonDto?.ClientPlanLoaded == true)
        {
            OpenPlanFile(comparisonDto.ClientPlantPath);
        }
    }

    private void empPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        if (comparisonDto?.EmpPlanLoaded == true)
        {
            OpenPlanFile(comparisonDto.EmpPlanPath);
        }
    }

    /// <summary>
    /// Handles creation of a new comparison
    /// </summary>
    private void newComparisonToolStripMenuItem_Click(object sender, EventArgs e)
    {
        comparisonDto = new ComparisonDto();
        InitializeUi();
    }
}
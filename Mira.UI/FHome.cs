using Mira.Core;

namespace Mira.UI;

public partial class FHome : Form
{
    private bool ClientreportLoaded = false;
    private bool EMPreportLoaded = false;
    private string ClientReportGeneratedName = string.Empty;
    private string EMPReportGeneratedName = string.Empty;

    public FHome()
    {
        InitializeComponent();
        CheckDirectories();
        InitializeStatusLabelCursors();
    }

    private void CheckDirectories()
    {
        //check if he directory Data is present else create it, then checks for directory Reports inside Data
        if (!Directory.Exists(Paths.DataDirectory))
        {
            Directory.CreateDirectory(Paths.DataDirectory);
        }
        if (!Directory.Exists(Paths.ReportsDirectory))
        {
            Directory.CreateDirectory(Paths.ReportsDirectory);
        }

    }

    private void InitializeStatusLabelCursors()
    {
        // Set default cursors for status labels based on initial load state
        UpdateStatusLabelCursor(clientPlanStatusValueLabel, ClientreportLoaded);
        UpdateStatusLabelCursor(empPlanStatusValueLabel, EMPreportLoaded);
    }

    private void UpdateStatusLabelCursor(Label statusLabel, bool isLoaded)
    {
        // Change cursor to hand if loaded, otherwise use default arrow cursor
        statusLabel.Cursor = isLoaded ? Cursors.Hand : Cursors.Default;
    }

    private void ImportFile(Enums.ReportType reportType, ref bool reportLoaded, ref string generatedName)
    {

        //Copy file to Reports directory with a unique name based on the report type and current timestamp
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Title = "Select Report File";
            openFileDialog.Filter = "All Files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(sourceFilePath);
                string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string destFileName = $"{reportType}_Report_{timeStamp}{fileExtension}";
                string destFilePath = Path.Combine(Paths.ReportsDirectory, destFileName);
                File.Copy(sourceFilePath, destFilePath);
                generatedName = destFileName;
                reportLoaded = true;
                MessageBox.Show($"{reportType} report imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void importClientPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ImportFile(Enums.ReportType.Client, ref ClientreportLoaded, ref ClientReportGeneratedName);
        clientPlanStatusValueLabel.Text = ClientreportLoaded ? "Loaded" : "Not Loaded";
        clientPlanStatusValueLabel.ForeColor = ClientreportLoaded ? Color.Green : Color.Red;
        UpdateStatusLabelCursor(clientPlanStatusValueLabel, ClientreportLoaded);
    }

    private void importEmpPlanToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ImportFile(Enums.ReportType.EMP, ref EMPreportLoaded, ref EMPReportGeneratedName);
        empPlanStatusValueLabel.Text = EMPreportLoaded ? "Loaded" : "Not Loaded";
        empPlanStatusValueLabel.ForeColor = EMPreportLoaded ? Color.Green : Color.Red;
        UpdateStatusLabelCursor(empPlanStatusValueLabel, EMPreportLoaded);
    }

    private void clientPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        if (ClientreportLoaded)
        {
            // Open the client report file
            string filePath = Path.Combine(Paths.ReportsDirectory, ClientReportGeneratedName);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true
            });

        }
    }

    private void empPlanStatusValueLabel_Click(object sender, EventArgs e)
    {
        if (EMPreportLoaded)
        {
            string filePath = Path.Combine(Paths.ReportsDirectory, EMPReportGeneratedName);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = filePath,
                UseShellExecute = true
            });
        }
    }
}
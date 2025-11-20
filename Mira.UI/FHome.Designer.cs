namespace Mira.UI;

partial class FHome
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        mainMenuStrip = new MenuStrip();
        comparisonToolStripMenuItem = new ToolStripMenuItem();
        newComparisonToolStripMenuItem = new ToolStripMenuItem();
        openComparisonToolStripMenuItem = new ToolStripMenuItem();
        saveComparisonToolStripMenuItem = new ToolStripMenuItem();
        saveAsComparisonToolStripMenuItem = new ToolStripMenuItem();
        deleteComparisonToolStripMenuItem = new ToolStripMenuItem();
        exitApplicationToolStripMenuItem = new ToolStripMenuItem();
        reviewToolStripMenuItem = new ToolStripMenuItem();
        importToolStripMenuItem = new ToolStripMenuItem();
        importClientPlanToolStripMenuItem = new ToolStripMenuItem();
        importEmpPlanToolStripMenuItem = new ToolStripMenuItem();
        exportToolStripMenuItem = new ToolStripMenuItem();
        exportExcelFormatToolStripMenuItem = new ToolStripMenuItem();
        exportWordFormatToolStripMenuItem = new ToolStripMenuItem();
        exportCsvFormatToolStripMenuItem = new ToolStripMenuItem();
        generalInfoGroupBox = new GroupBox();
        comparisonDateTextBox = new TextBox();
        comparisonDateLabel = new Label();
        responsiblePersonTextBox = new TextBox();
        responsiblePersonLabel = new Label();
        empPlanRefTextBox = new TextBox();
        empPlanRefLabel = new Label();
        clientPlanRefTextBox = new TextBox();
        clientPlanRefLabel = new Label();
        projectNameTextBox = new TextBox();
        projectNameLabel = new Label();
        comparisonContainerGroupBox = new GroupBox();
        compareButton = new Button();
        planStatusGroupBox = new GroupBox();
        clientPlanStatusValueLabel = new Label();
        clientPlanStatusLabel = new Label();
        empPlanStatusValueLabel = new Label();
        empPlanStatusLabel = new Label();
        comparisonDataGridView = new DataGridView();
        comparisonResultsGroupBox = new GroupBox();
        mainMenuStrip.SuspendLayout();
        generalInfoGroupBox.SuspendLayout();
        comparisonContainerGroupBox.SuspendLayout();
        planStatusGroupBox.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)comparisonDataGridView).BeginInit();
        comparisonResultsGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // mainMenuStrip
        // 
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { comparisonToolStripMenuItem, reviewToolStripMenuItem, exportToolStripMenuItem });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(800, 24);
        mainMenuStrip.TabIndex = 0;
        mainMenuStrip.Text = "mainMenuStrip";
        // 
        // comparisonToolStripMenuItem
        // 
        comparisonToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newComparisonToolStripMenuItem, openComparisonToolStripMenuItem, saveComparisonToolStripMenuItem, saveAsComparisonToolStripMenuItem, deleteComparisonToolStripMenuItem, exitApplicationToolStripMenuItem });
        comparisonToolStripMenuItem.Name = "comparisonToolStripMenuItem";
        comparisonToolStripMenuItem.Size = new Size(84, 20);
        comparisonToolStripMenuItem.Text = "Comparison";
        // 
        // newComparisonToolStripMenuItem
        // 
        newComparisonToolStripMenuItem.Name = "newComparisonToolStripMenuItem";
        newComparisonToolStripMenuItem.Size = new Size(158, 22);
        newComparisonToolStripMenuItem.Text = "Nouveau";
        // 
        // openComparisonToolStripMenuItem
        // 
        openComparisonToolStripMenuItem.Name = "openComparisonToolStripMenuItem";
        openComparisonToolStripMenuItem.Size = new Size(158, 22);
        openComparisonToolStripMenuItem.Text = "Open";
        // 
        // saveComparisonToolStripMenuItem
        // 
        saveComparisonToolStripMenuItem.Name = "saveComparisonToolStripMenuItem";
        saveComparisonToolStripMenuItem.Size = new Size(158, 22);
        saveComparisonToolStripMenuItem.Text = "Enregistrer";
        // 
        // saveAsComparisonToolStripMenuItem
        // 
        saveAsComparisonToolStripMenuItem.Name = "saveAsComparisonToolStripMenuItem";
        saveAsComparisonToolStripMenuItem.Size = new Size(158, 22);
        saveAsComparisonToolStripMenuItem.Text = "Enregistrer Sous";
        // 
        // deleteComparisonToolStripMenuItem
        // 
        deleteComparisonToolStripMenuItem.Name = "deleteComparisonToolStripMenuItem";
        deleteComparisonToolStripMenuItem.Size = new Size(158, 22);
        deleteComparisonToolStripMenuItem.Text = "Supprimer";
        // 
        // exitApplicationToolStripMenuItem
        // 
        exitApplicationToolStripMenuItem.Name = "exitApplicationToolStripMenuItem";
        exitApplicationToolStripMenuItem.Size = new Size(158, 22);
        exitApplicationToolStripMenuItem.Text = "Quitter";
        // 
        // reviewToolStripMenuItem
        // 
        reviewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importToolStripMenuItem });
        reviewToolStripMenuItem.Name = "reviewToolStripMenuItem";
        reviewToolStripMenuItem.Size = new Size(51, 20);
        reviewToolStripMenuItem.Text = "Revue";
        // 
        // importToolStripMenuItem
        // 
        importToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { importClientPlanToolStripMenuItem, importEmpPlanToolStripMenuItem });
        importToolStripMenuItem.Name = "importToolStripMenuItem";
        importToolStripMenuItem.Size = new Size(120, 22);
        importToolStripMenuItem.Text = "Importer";
        // 
        // importClientPlanToolStripMenuItem
        // 
        importClientPlanToolStripMenuItem.Name = "importClientPlanToolStripMenuItem";
        importClientPlanToolStripMenuItem.Size = new Size(131, 22);
        importClientPlanToolStripMenuItem.Text = "Plan Client";
        // 
        // importEmpPlanToolStripMenuItem
        // 
        importEmpPlanToolStripMenuItem.Name = "importEmpPlanToolStripMenuItem";
        importEmpPlanToolStripMenuItem.Size = new Size(131, 22);
        importEmpPlanToolStripMenuItem.Text = "Plan EMP";
        // 
        // exportToolStripMenuItem
        // 
        exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportExcelFormatToolStripMenuItem, exportWordFormatToolStripMenuItem, exportCsvFormatToolStripMenuItem });
        exportToolStripMenuItem.Name = "exportToolStripMenuItem";
        exportToolStripMenuItem.Size = new Size(59, 20);
        exportToolStripMenuItem.Text = "Exporte";
        // 
        // exportExcelFormatToolStripMenuItem
        // 
        exportExcelFormatToolStripMenuItem.Name = "exportExcelFormatToolStripMenuItem";
        exportExcelFormatToolStripMenuItem.Size = new Size(144, 22);
        exportExcelFormatToolStripMenuItem.Text = "Format Excel";
        // 
        // exportWordFormatToolStripMenuItem
        // 
        exportWordFormatToolStripMenuItem.Name = "exportWordFormatToolStripMenuItem";
        exportWordFormatToolStripMenuItem.Size = new Size(144, 22);
        exportWordFormatToolStripMenuItem.Text = "Format Word";
        // 
        // exportCsvFormatToolStripMenuItem
        // 
        exportCsvFormatToolStripMenuItem.Name = "exportCsvFormatToolStripMenuItem";
        exportCsvFormatToolStripMenuItem.Size = new Size(144, 22);
        exportCsvFormatToolStripMenuItem.Text = "Format Csv";
        // 
        // generalInfoGroupBox
        // 
        generalInfoGroupBox.Controls.Add(compareButton);
        generalInfoGroupBox.Controls.Add(comparisonDateTextBox);
        generalInfoGroupBox.Controls.Add(comparisonDateLabel);
        generalInfoGroupBox.Controls.Add(responsiblePersonTextBox);
        generalInfoGroupBox.Controls.Add(responsiblePersonLabel);
        generalInfoGroupBox.Controls.Add(empPlanRefTextBox);
        generalInfoGroupBox.Controls.Add(empPlanRefLabel);
        generalInfoGroupBox.Controls.Add(clientPlanRefTextBox);
        generalInfoGroupBox.Controls.Add(clientPlanRefLabel);
        generalInfoGroupBox.Controls.Add(projectNameTextBox);
        generalInfoGroupBox.Controls.Add(projectNameLabel);
        generalInfoGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        generalInfoGroupBox.Location = new Point(6, 35);
        generalInfoGroupBox.Name = "generalInfoGroupBox";
        generalInfoGroupBox.Size = new Size(764, 95);
        generalInfoGroupBox.TabIndex = 1;
        generalInfoGroupBox.TabStop = false;
        generalInfoGroupBox.Text = "Information General";
        // 
        // comparisonDateTextBox
        // 
        comparisonDateTextBox.CharacterCasing = CharacterCasing.Upper;
        comparisonDateTextBox.Font = new Font("Segoe UI", 9F);
        comparisonDateTextBox.Location = new Point(616, 24);
        comparisonDateTextBox.Name = "comparisonDateTextBox";
        comparisonDateTextBox.Size = new Size(142, 23);
        comparisonDateTextBox.TabIndex = 1;
        comparisonDateTextBox.TextAlign = HorizontalAlignment.Center;
        // 
        // comparisonDateLabel
        // 
        comparisonDateLabel.AutoSize = true;
        comparisonDateLabel.Location = new Point(573, 27);
        comparisonDateLabel.Name = "comparisonDateLabel";
        comparisonDateLabel.Size = new Size(40, 15);
        comparisonDateLabel.TabIndex = 0;
        comparisonDateLabel.Text = "Date :";
        // 
        // responsiblePersonTextBox
        // 
        responsiblePersonTextBox.CharacterCasing = CharacterCasing.Upper;
        responsiblePersonTextBox.Font = new Font("Segoe UI", 9F);
        responsiblePersonTextBox.Location = new Point(389, 24);
        responsiblePersonTextBox.Name = "responsiblePersonTextBox";
        responsiblePersonTextBox.Size = new Size(163, 23);
        responsiblePersonTextBox.TabIndex = 1;
        responsiblePersonTextBox.TextAlign = HorizontalAlignment.Center;
        // 
        // responsiblePersonLabel
        // 
        responsiblePersonLabel.AutoSize = true;
        responsiblePersonLabel.Location = new Point(304, 27);
        responsiblePersonLabel.Name = "responsiblePersonLabel";
        responsiblePersonLabel.Size = new Size(82, 15);
        responsiblePersonLabel.TabIndex = 0;
        responsiblePersonLabel.Text = "Responsable :";
        // 
        // empPlanRefTextBox
        // 
        empPlanRefTextBox.CharacterCasing = CharacterCasing.Upper;
        empPlanRefTextBox.Font = new Font("Segoe UI", 9F);
        empPlanRefTextBox.Location = new Point(137, 62);
        empPlanRefTextBox.Name = "empPlanRefTextBox";
        empPlanRefTextBox.Size = new Size(153, 23);
        empPlanRefTextBox.TabIndex = 1;
        empPlanRefTextBox.TextAlign = HorizontalAlignment.Center;
        // 
        // empPlanRefLabel
        // 
        empPlanRefLabel.AutoSize = true;
        empPlanRefLabel.Location = new Point(6, 65);
        empPlanRefLabel.Name = "empPlanRefLabel";
        empPlanRefLabel.Size = new Size(125, 15);
        empPlanRefLabel.TabIndex = 0;
        empPlanRefLabel.Text = "Reférence Plan EMP :";
        // 
        // clientPlanRefTextBox
        // 
        clientPlanRefTextBox.CharacterCasing = CharacterCasing.Upper;
        clientPlanRefTextBox.Font = new Font("Segoe UI", 9F);
        clientPlanRefTextBox.Location = new Point(435, 62);
        clientPlanRefTextBox.Name = "clientPlanRefTextBox";
        clientPlanRefTextBox.Size = new Size(153, 23);
        clientPlanRefTextBox.TabIndex = 1;
        clientPlanRefTextBox.TextAlign = HorizontalAlignment.Center;
        // 
        // clientPlanRefLabel
        // 
        clientPlanRefLabel.AutoSize = true;
        clientPlanRefLabel.Location = new Point(304, 65);
        clientPlanRefLabel.Name = "clientPlanRefLabel";
        clientPlanRefLabel.Size = new Size(133, 15);
        clientPlanRefLabel.TabIndex = 0;
        clientPlanRefLabel.Text = "Reférence Plan Client :";
        // 
        // projectNameTextBox
        // 
        projectNameTextBox.CharacterCasing = CharacterCasing.Upper;
        projectNameTextBox.Font = new Font("Segoe UI", 9F);
        projectNameTextBox.Location = new Point(56, 24);
        projectNameTextBox.Name = "projectNameTextBox";
        projectNameTextBox.Size = new Size(234, 23);
        projectNameTextBox.TabIndex = 1;
        projectNameTextBox.TextAlign = HorizontalAlignment.Center;
        // 
        // projectNameLabel
        // 
        projectNameLabel.AutoSize = true;
        projectNameLabel.Location = new Point(6, 27);
        projectNameLabel.Name = "projectNameLabel";
        projectNameLabel.Size = new Size(47, 15);
        projectNameLabel.TabIndex = 0;
        projectNameLabel.Text = "Projet :";
        // 
        // comparisonContainerGroupBox
        // 
        comparisonContainerGroupBox.Controls.Add(comparisonResultsGroupBox);
        comparisonContainerGroupBox.Controls.Add(planStatusGroupBox);
        comparisonContainerGroupBox.Controls.Add(generalInfoGroupBox);
        comparisonContainerGroupBox.Location = new Point(12, 41);
        comparisonContainerGroupBox.Name = "comparisonContainerGroupBox";
        comparisonContainerGroupBox.Size = new Size(776, 397);
        comparisonContainerGroupBox.TabIndex = 2;
        comparisonContainerGroupBox.TabStop = false;
        comparisonContainerGroupBox.Text = "Comparison COMP0001";
        // 
        // compareButton
        // 
        compareButton.Location = new Point(616, 62);
        compareButton.Name = "compareButton";
        compareButton.Size = new Size(142, 23);
        compareButton.TabIndex = 2;
        compareButton.Text = "Comparer";
        compareButton.UseVisualStyleBackColor = true;
        // 
        // planStatusGroupBox
        // 
        planStatusGroupBox.Controls.Add(empPlanStatusValueLabel);
        planStatusGroupBox.Controls.Add(empPlanStatusLabel);
        planStatusGroupBox.Controls.Add(clientPlanStatusValueLabel);
        planStatusGroupBox.Controls.Add(clientPlanStatusLabel);
        planStatusGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        planStatusGroupBox.Location = new Point(6, 136);
        planStatusGroupBox.Name = "planStatusGroupBox";
        planStatusGroupBox.Size = new Size(764, 44);
        planStatusGroupBox.TabIndex = 2;
        planStatusGroupBox.TabStop = false;
        planStatusGroupBox.Text = "Statut de plans";
        // 
        // clientPlanStatusLabel
        // 
        clientPlanStatusLabel.AutoSize = true;
        clientPlanStatusLabel.Location = new Point(15, 19);
        clientPlanStatusLabel.Name = "clientPlanStatusLabel";
        clientPlanStatusLabel.Size = new Size(94, 15);
        clientPlanStatusLabel.TabIndex = 0;
        clientPlanStatusLabel.Text = "Plan Client est : ";
        // 
        // clientPlanStatusValueLabel
        // 
        clientPlanStatusValueLabel.AutoSize = true;
        clientPlanStatusValueLabel.ForeColor = Color.Red;
        clientPlanStatusValueLabel.Location = new Point(103, 19);
        clientPlanStatusValueLabel.Name = "clientPlanStatusValueLabel";
        clientPlanStatusValueLabel.Size = new Size(72, 15);
        clientPlanStatusValueLabel.TabIndex = 0;
        clientPlanStatusValueLabel.Text = "Introuvable";
        // 
        // empPlanStatusLabel
        // 
        empPlanStatusLabel.AutoSize = true;
        empPlanStatusLabel.Location = new Point(194, 19);
        empPlanStatusLabel.Name = "empPlanStatusLabel";
        empPlanStatusLabel.Size = new Size(86, 15);
        empPlanStatusLabel.TabIndex = 0;
        empPlanStatusLabel.Text = "Plan EMP est : ";
        // 
        // empPlanStatusValueLabel
        // 
        empPlanStatusValueLabel.AutoSize = true;
        empPlanStatusValueLabel.ForeColor = Color.Red;
        empPlanStatusValueLabel.Location = new Point(282, 19);
        empPlanStatusValueLabel.Name = "empPlanStatusValueLabel";
        empPlanStatusValueLabel.Size = new Size(72, 15);
        empPlanStatusValueLabel.TabIndex = 0;
        empPlanStatusValueLabel.Text = "Introuvable";
        // 
        // comparisonDataGridView
        // 
        comparisonDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        comparisonDataGridView.Dock = DockStyle.Fill;
        comparisonDataGridView.Location = new Point(3, 19);
        comparisonDataGridView.Name = "comparisonDataGridView";
        comparisonDataGridView.Size = new Size(758, 183);
        comparisonDataGridView.TabIndex = 3;
        // 
        // comparisonResultsGroupBox
        // 
        comparisonResultsGroupBox.Controls.Add(comparisonDataGridView);
        comparisonResultsGroupBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        comparisonResultsGroupBox.Location = new Point(6, 186);
        comparisonResultsGroupBox.Name = "comparisonResultsGroupBox";
        comparisonResultsGroupBox.Size = new Size(764, 205);
        comparisonResultsGroupBox.TabIndex = 4;
        comparisonResultsGroupBox.TabStop = false;
        comparisonResultsGroupBox.Text = "Resultat";
        // 
        // FHome
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(comparisonContainerGroupBox);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        Name = "FHome";
        Text = "Mira- Revue Technique des Plans";
        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        generalInfoGroupBox.ResumeLayout(false);
        generalInfoGroupBox.PerformLayout();
        comparisonContainerGroupBox.ResumeLayout(false);
        planStatusGroupBox.ResumeLayout(false);
        planStatusGroupBox.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)comparisonDataGridView).EndInit();
        comparisonResultsGroupBox.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem comparisonToolStripMenuItem;
    private ToolStripMenuItem newComparisonToolStripMenuItem;
    private ToolStripMenuItem openComparisonToolStripMenuItem;
    private ToolStripMenuItem saveComparisonToolStripMenuItem;
    private ToolStripMenuItem saveAsComparisonToolStripMenuItem;
    private ToolStripMenuItem deleteComparisonToolStripMenuItem;
    private ToolStripMenuItem exitApplicationToolStripMenuItem;
    private ToolStripMenuItem reviewToolStripMenuItem;
    private ToolStripMenuItem importToolStripMenuItem;
    private ToolStripMenuItem importClientPlanToolStripMenuItem;
    private ToolStripMenuItem importEmpPlanToolStripMenuItem;
    private ToolStripMenuItem exportToolStripMenuItem;
    private ToolStripMenuItem exportExcelFormatToolStripMenuItem;
    private ToolStripMenuItem exportWordFormatToolStripMenuItem;
    private ToolStripMenuItem exportCsvFormatToolStripMenuItem;
    private GroupBox generalInfoGroupBox;
    private TextBox responsiblePersonTextBox;
    private Label responsiblePersonLabel;
    private TextBox empPlanRefTextBox;
    private Label empPlanRefLabel;
    private TextBox clientPlanRefTextBox;
    private Label clientPlanRefLabel;
    private TextBox projectNameTextBox;
    private Label projectNameLabel;
    private TextBox comparisonDateTextBox;
    private Label comparisonDateLabel;
    private GroupBox comparisonContainerGroupBox;
    private Button compareButton;
    private GroupBox comparisonResultsGroupBox;
    private DataGridView comparisonDataGridView;
    private GroupBox planStatusGroupBox;
    private Label empPlanStatusValueLabel;
    private Label empPlanStatusLabel;
    private Label clientPlanStatusValueLabel;
    private Label clientPlanStatusLabel;
}
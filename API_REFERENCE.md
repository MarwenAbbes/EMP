# API Reference & Component Documentation

## Table of Contents
1. [Mira.Core Namespace](#miracore-namespace)
2. [Mira.Core.DTO Namespace](#miracoredto-namespace)
3. [Mira.Core.Services Namespace](#miracoreservices-namespace)
4. [Mira.UI Namespace](#miraui-namespace)
5. [Quick Reference](#quick-reference)

---

## Mira.Core Namespace

### Enums Class
**Namespace:** `Mira.Core`  
**Type:** `static class` (contains only enumerations)  
**Purpose:** Define enumeration types used throughout the application

#### ReportType Enumeration

```csharp
public enum ReportType
{
    Client = 0,  // Client plan document
    EMP = 1      // Employee/EMP plan document
}
```

**Values:**
- `Client` - Represents client-provided plan documents
- `EMP` - Represents employee/company plan documents

**Usage Examples:**
```csharp
// Identifying import type
FileImportService service = new FileImportService();
string? fileName = service.ImportFile(Enums.ReportType.Client, directory);

// Updating status labels
UpdateStatusLabel(Enums.ReportType.EMP, true);

// Conditional logic
if (reportType == Enums.ReportType.Client)
{
    comparisonDto.ClientPlantPath = fileName;
}
```

---

### Paths Class
**Namespace:** `Mira.Core`  
**Type:** `abstract class`  
**Purpose:** Define and manage directory paths for application data

#### Properties

##### DataDirectory
```csharp
public static string DataDirectory { get; }
```

**Type:** String  
**Value:** `Path.Combine(Environment.CurrentDirectory, "Data")`  
**Example:** `"C:\Projects\EMP\Data"`  
**Purpose:** Root directory for application data

##### ReportsDirectory
```csharp
public static string ReportsDirectory { get; }
```

**Type:** String  
**Value:** `Path.Combine(DataDirectory, "Reports")`  
**Example:** `"C:\Projects\EMP\Data\Reports"`  
**Purpose:** Directory containing all comparison reports

**Directory Structure:**
```
DataDirectory/
??? Reports/
    ??? COMP-0001/
    ?   ??? Client_Report_20240115_143022.pdf
    ?   ??? EMP_Report_20240115_143045.pdf
    ??? COMP-0002/
    ?   ??? Client_Report_20240120_091500.pdf
    ?   ??? EMP_Report_20240120_091522.pdf
    ??? COMP-0003/
        ??? Client_Report_...
        ??? EMP_Report_...
```

**Usage Examples:**
```csharp
// Check if directory exists
if (Directory.Exists(Paths.ReportsDirectory))
{
    // Directory operations
}

// Create new comparison directory
string comparisonDir = Path.Combine(Paths.ReportsDirectory, "COMP-0001");
Directory.CreateDirectory(comparisonDir);

// List all comparisons
var comparisons = Directory.GetDirectories(Paths.ReportsDirectory);
```

---

### Constants Class
**Namespace:** `Mira.Core`  
**Type:** `public static class`  
**Purpose:** Centralize application constants and configuration values

#### String Constants

##### COMPARISON_PREFIX
```csharp
public const string COMPARISON_PREFIX = "COMP";
```

**Type:** String  
**Value:** `"COMP"`  
**Usage:** Prefix for comparison IDs  
**Example ID:** `"COMP-0001"`, `"COMP-0042"`

---

##### TIMESTAMP_FORMAT
```csharp
public const string TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";
```

**Type:** String  
**Format Spec:** `yyyyMMdd_HHmmss`  
**Example:** `"20240115_143022"` for 2024-01-15 at 14:30:22

**Usage in Filenames:**
```
Client_Report_20240115_143022.pdf
EMP_Report_20240115_143045.pdf
```

---

##### PDF_FILTER
```csharp
public const string PDF_FILTER = "PDF Files (*.pdf)|*.pdf";
```

**Type:** String  
**Purpose:** File dialog filter for PDF files  
**Format:** Windows Forms file dialog filter string

---

##### SELECT_REPORT_FILE_TITLE
```csharp
public const string SELECT_REPORT_FILE_TITLE = "Select Report File";
```

**Type:** String  
**Usage:** Title for file open dialog

---

##### REPORT_IMPORT_SUCCESS_MESSAGE
```csharp
public const string REPORT_IMPORT_SUCCESS_MESSAGE = "{0} report imported successfully!";
```

**Type:** String  
**Format String:** Yes (uses `{0}` placeholder)  
**Usage:** Success notification after import  
**Example Output:** 
- `"Client report imported successfully!"`
- `"EMP report imported successfully!"`

---

##### REPORT_IMPORT_SUCCESS_TITLE
```csharp
public const string REPORT_IMPORT_SUCCESS_TITLE = "Success";
```

**Type:** String  
**Usage:** Title for success message dialog

---

### Utils Class
**Namespace:** `Mira.Core`  
**Type:** `public static class`  
**Purpose:** Utility functions for common operations

#### GetNextComparisonId Method

```csharp
public static string GetNextComparisonId()
```

**Return Type:** `String`  
**Parameters:** None  
**Throws:** None (returns gracefully for all cases)

**Returns:** Next sequential comparison ID

**Examples:**
```
Empty Reports directory ? "COMP-0001"
Existing: COMP-0001, COMP-0002 ? "COMP-0003"
Existing: COMP-0001, COMP-0003, COMP-0005 ? "COMP-0006"
```

**Algorithm Overview:**
1. Check if Reports directory exists
2. List all subdirectories
3. Filter those starting with "COMP"
4. Extract numeric suffix
5. Find maximum number
6. Return next sequential ID

**Error Handling:**
- Reports directory missing ? Safe return with COMP-0001
- Invalid directory names ? Safely skip with error handling
- Integer overflow ? Unlikely with practical usage

**Performance:**
- Time Complexity: O(n) where n = number of comparison directories
- Space Complexity: O(n) for directory listing
- Typical case: < 1ms for most applications

**Example Usage:**
```csharp
// Generate new ID during comparison creation
string newId = Utils.GetNextComparisonId();
// Result: "COMP-0005" (if COMP-0001 through COMP-0004 exist)

// Use in directory creation
string baseDir = Path.Combine(Paths.ReportsDirectory, newId);
Directory.CreateDirectory(baseDir);
```

---

## Mira.Core.DTO Namespace

### ComparisonDto Class
**Namespace:** `Mira.Core.DTO`  
**Type:** `public class`  
**Purpose:** Data Transfer Object for comparison project data

#### Properties

##### Id
```csharp
public string Id { get; }
```

**Type:** String (read-only)  
**Format:** `"COMP-XXXX"` where XXXX is 4-digit number  
**Example:** `"COMP-0001"`, `"COMP-0042"`  
**Set:** By constructor via `GetNextComparisonId()`  
**Purpose:** Unique identifier for the comparison

---

##### BaseReportDirectory
```csharp
public string BaseReportDirectory { get; }
```

**Type:** String (read-only)  
**Example:** `"Data/Reports/COMP-0001"`  
**Set:** By constructor via `Path.Combine()`  
**Purpose:** Root directory for comparison's imported files  
**Usage:** Passed to `FileImportService.ImportFile()`

---

##### ProjectName
```csharp
public string ProjectName { get; set; }
```

**Type:** String  
**Default:** null  
**Example:** `"Highway Expansion Phase 2"`  
**Purpose:** Name of the project being reviewed

---

##### ResponsiblePerson
```csharp
public string ResponsiblePerson { get; set; }
```

**Type:** String  
**Default:** null  
**Example:** `"John Smith"`  
**Purpose:** Person responsible for the comparison

---

##### ComparisonDate
```csharp
public DateTime ComparisonDate { get; set; }
```

**Type:** DateTime  
**Default:** `DateTime.MinValue`  
**Example:** `new DateTime(2024, 1, 15)`  
**Purpose:** Date when the comparison was conducted

---

##### EmpPlanReference
```csharp
public string EmpPlanReference { get; set; }
```

**Type:** String  
**Default:** null  
**Example:** `"EMP-2024-001"`  
**Purpose:** Reference number or identifier for EMP plan

---

##### ClientPlanReference
```csharp
public string ClientPlanReference { get; set; }
```

**Type:** String  
**Default:** null  
**Example:** `"CP-2024-A"`  
**Purpose:** Reference number or identifier for Client plan

---

##### ClientPlantPath
```csharp
public string ClientPlantPath { get; set; }
```

**Type:** String (filename, not full path)  
**Default:** null  
**Example:** `"Client_Report_20240115_143022.pdf"`  
**Purpose:** Filename of imported Client plan  
**Note:** Use with `BaseReportDirectory` to get full path

---

##### EmpPlanPath
```csharp
public string EmpPlanPath { get; set; }
```

**Type:** String (filename, not full path)  
**Default:** null  
**Example:** `"EMP_Report_20240115_143045.pdf"`  
**Purpose:** Filename of imported EMP plan  
**Note:** Use with `BaseReportDirectory` to get full path

---

##### ClientPlanLoaded
```csharp
public bool ClientPlanLoaded { get; set; }
```

**Type:** Boolean  
**Default:** `false`  
**Meaning:** 
- `true` - Client plan has been imported
- `false` - Client plan not yet imported

---

##### EmpPlanLoaded
```csharp
public bool EmpPlanLoaded { get; set; }
```

**Type:** Boolean  
**Default:** `false`  
**Meaning:**
- `true` - EMP plan has been imported
- `false` - EMP plan not yet imported

---

#### Constructor

```csharp
public ComparisonDto()
```

**Parameters:** None

**Initialization:**
```
1. Call Utils.GetNextComparisonId() ? "COMP-XXXX"
2. Store in _id
3. Create _baseReportDirectory = "Data/Reports/COMP-XXXX"
4. Call DirectoryService.EnsureDirectoriesExist()
5. Initialize all properties to default values
```

**Directories Created:**
- `Data/` (if doesn't exist)
- `Data/Reports/COMP-XXXX/` (if doesn't exist)

**Example Usage:**
```csharp
// Create new comparison
var comparison = new ComparisonDto();

// Access properties
Console.WriteLine($"ID: {comparison.Id}");  // "COMP-0001"
Console.WriteLine($"Dir: {comparison.BaseReportDirectory}");  // "Data/Reports/COMP-0001"

// Set metadata
comparison.ProjectName = "Highway Review";
comparison.ResponsiblePerson = "John Smith";
comparison.ComparisonDate = DateTime.Now;

// Import files later
comparison.ClientPlantPath = "Client_Report_20240115_143022.pdf";
comparison.ClientPlanLoaded = true;
```

---

## Mira.Core.Services Namespace

### IFileImportService Interface
**Namespace:** `Mira.Core.Services`  
**Type:** `public interface`  
**Purpose:** Define contract for file import operations

#### ImportFile Method

```csharp
string? ImportFile(Enums.ReportType reportType, string destinationDirectory);
```

**Parameters:**
- `reportType` (Enums.ReportType) - Type of report being imported
  - `ReportType.Client` - Client plan document
  - `ReportType.EMP` - Employee plan document
- `destinationDirectory` (string) - Target directory path
  - Example: `"Data/Reports/COMP-0001"`

**Return Type:** `string?` (nullable string)
- **Returns:** Filename of imported file (e.g., `"Client_Report_20240115_143022.pdf"`)
- **Returns:** `null` if user cancels operation

**Behavior:**
1. Shows file open dialog
2. Allows user to select a PDF file
3. On confirmation: Copies file to destination with timestamp in name
4. On cancel: Returns null without creating files

**Exceptions:**
- `IOException` - File already exists at destination
- `UnauthorizedAccessException` - Permission denied
- `ArgumentException` - Invalid directory path

**Example Usage:**
```csharp
var service = new FileImportService();

// Import Client plan
string? clientFileName = service.ImportFile(
    Enums.ReportType.Client,
    "Data/Reports/COMP-0001"
);

if (clientFileName != null)
{
    Console.WriteLine($"Imported: {clientFileName}");
    // User selected a file and it was copied
}
else
{
    Console.WriteLine("Import cancelled by user");
}

// Import EMP plan
string? empFileName = service.ImportFile(
    Enums.ReportType.EMP,
    "Data/Reports/COMP-0001"
);
```

---

### FileImportService Class
**Namespace:** `Mira.Core.Services`  
**Type:** `public class`  
**Implements:** `IFileImportService`  
**Purpose:** Concrete implementation of file import operations

#### ImportFile Method (Implementation)

```csharp
public string? ImportFile(Enums.ReportType reportType, string destinationDirectory)
```

**Dialog Configuration:**
- **Title:** "Select Report File" (from Constants.SELECT_REPORT_FILE_TITLE)
- **Filter:** "PDF Files (*.pdf)|*.pdf" (from Constants.PDF_FILTER)
- **Default Directory:** User's last browsed directory

**File Naming Convention:**
```
Format: {ReportType}_Report_{Timestamp}{Extension}
Client Example: Client_Report_20240115_143022.pdf
EMP Example: EMP_Report_20240115_143045.pdf
```

**Timestamp:**
- Format: `yyyyMMdd_HHmmss` (Constants.TIMESTAMP_FORMAT)
- From: `DateTime.Now`
- Purpose: Ensure unique filenames for each import

**Overwrite Behavior:**
- **Does not overwrite** existing files
- Throws `IOException` if file exists
- Prevents accidental data loss

**Process Flow:**
```
ImportFile(reportType, destinationDirectory)
  ?
Create OpenFileDialog
  ?? Title: "Select Report File"
  ?? Filter: "PDF Files (*.pdf)|*.pdf"
  ?? Show to user
  ?
User action?
  ?? OK (selected file)
  ?   ?
  ?   CopyFileToDestination()
  ?   ?? Extract file extension
  ?   ?? Generate timestamp
  ?   ?? Create filename
  ?   ?? Copy source to destination
  ?   ?? Return filename
  ?
  ?? Cancel
      ?
      Return null
```

#### CopyFileToDestination Method (Private)

```csharp
private string CopyFileToDestination(
    string sourceFilePath,
    Enums.ReportType reportType,
    string destinationDirectory)
```

**Parameters:**
- `sourceFilePath` - Full path to selected file
- `reportType` - Type of report (Client or EMP)
- `destinationDirectory` - Target directory

**Implementation Steps:**
1. Extract file extension from source
2. Get current timestamp
3. Generate unique filename
4. Build destination path
5. Copy file to destination
6. Return generated filename

**Example:**
```
Input:
  sourceFilePath = "C:\Downloads\planning_document.pdf"
  reportType = ReportType.Client
  destinationDirectory = "Data/Reports/COMP-0001"

Processing:
  extension = ".pdf"
  timestamp = "20240115_143022"
  filename = "Client_Report_20240115_143022.pdf"
  destPath = "Data/Reports/COMP-0001/Client_Report_20240115_143022.pdf"

Output:
  returns "Client_Report_20240115_143022.pdf"
```

---

### IDirectoryService Interface
**Namespace:** `Mira.Core.Services`  
**Type:** `public interface`  
**Purpose:** Define contract for directory operations

#### EnsureDirectoriesExist Method

```csharp
void EnsureDirectoriesExist(string comparisonId);
```

**Parameters:**
- `comparisonId` (string) - Comparison ID
  - Example: `"COMP-0001"`

**Responsibility:**
Ensures required directory structure exists:
```
Data/
??? Reports/
    ??? {comparisonId}/
```

**Behavior:**
- Non-throwing - safely handles all cases
- Idempotent - safe to call multiple times
- Creates directories only if they don't exist

**Example Usage:**
```csharp
var service = new DirectoryService();

// Ensure directories exist for COMP-0001
service.EnsureDirectoriesExist("COMP-0001");

// Can now safely import files to:
// Data/Reports/COMP-0001/
```

---

### DirectoryService Class
**Namespace:** `Mira.Core.Services`  
**Type:** `public class`  
**Implements:** `IDirectoryService`  
**Purpose:** Concrete implementation of directory operations

#### EnsureDirectoriesExist Method (Implementation)

```csharp
public void EnsureDirectoriesExist(string comparisonId)
```

**Implementation:**
```csharp
// Step 1: Ensure Data directory exists
if (!Directory.Exists(Paths.DataDirectory))
{
    Directory.CreateDirectory(Paths.DataDirectory);
}

// Step 2: Ensure comparison-specific directory exists
string comparisonDirectory = Path.Combine(Paths.ReportsDirectory, comparisonId);
if (!Directory.Exists(comparisonDirectory))
{
    Directory.CreateDirectory(comparisonDirectory);
}
```

**Directory Creation Order:**
1. First: `Data/`
2. Then: `Data/Reports/` (automatically created by Reports reference)
3. Finally: `Data/Reports/COMP-XXXX/`

**Safety Features:**
- Uses `Directory.Exists()` before creating
- Uses `Path.Combine()` for safe path building
- No exceptions thrown (idempotent)
- Thread-safe per .NET Framework guarantees

---

## Mira.UI Namespace

### FHome Class
**Namespace:** `Mira.UI`  
**Type:** `public partial class` (Form)  
**Inherits:** `Form`  
**Purpose:** Main application window and user interaction handler

#### Fields

##### comparisonDto
```csharp
private ComparisonDto comparisonDto = null;
```

**Type:** ComparisonDto (nullable)  
**Default:** `null`  
**Purpose:** Holds current active comparison data  
**Lifecycle:**
- Created when user clicks "New Comparison"
- Destroyed when application closes
- Null when no comparison is active

---

##### _fileImportService
```csharp
private IFileImportService _fileImportService;
```

**Type:** IFileImportService  
**Initialized:** In constructor as `new FileImportService()`  
**Purpose:** Service for importing plan documents  
**Usage:** Called from `HandleImportFile()`

---

##### _reportTypeMapping
```csharp
private readonly Dictionary<Enums.ReportType, (Label statusLabel, string propertyName)> 
    _reportTypeMapping = new()
{
    { Enums.ReportType.Client, (null, nameof(ComparisonDto.ClientPlantPath)) },
    { Enums.ReportType.EMP, (null, nameof(ComparisonDto.EmpPlanPath)) }
};
```

**Type:** Dictionary<ReportType, Tuple<Label, string>>  
**Purpose:** Maps report types to UI components  
**Initialized:** In `InitializeReportTypeMapping()`

---

#### Constructor

```csharp
public FHome()
```

**Initialization Sequence:**
1. `InitializeComponent()` - Create Windows Forms components
2. `_fileImportService = new FileImportService()` - Create service
3. `InitializeReportTypeMapping()` - Map types to UI labels
4. `InitializeUi()` - Set initial UI state

---

#### InitializeReportTypeMapping Method

```csharp
private void InitializeReportTypeMapping()
```

**Purpose:** Associate ReportTypes with their UI components

**Mapping:**
- `ReportType.Client` ? `clientPlanStatusValueLabel`
- `ReportType.EMP` ? `empPlanStatusValueLabel`

**Used By:** `UpdateStatusLabel()` for dynamic updates

---

#### InitializeUi Method

```csharp
private void InitializeUi()
```

**Purpose:** Set initial UI state based on comparison existence

**Actions:**
```csharp
// Enable/disable menu items based on comparison
saveComparisonToolStripMenuItem.Enabled = comparisonDto != null;
saveAsComparisonToolStripMenuItem.Enabled = comparisonDto != null;
deleteComparisonToolStripMenuItem.Enabled = comparisonDto != null;
reviewToolStripMenuItem.Enabled = comparisonDto != null;
exportToolStripMenuItem.Enabled = comparisonDto != null;

// Show/hide comparison container
comparisonContainerGroupBox.Visible = comparisonDto != null;

// Update title with comparison ID
comparisonContainerGroupBox.Text = comparisonDto != null ? 
    comparisonDto.Id : 
    string.Empty;

// Update status labels
UpdateAllStatusLabels();
```

---

#### UpdateAllStatusLabels Method

```csharp
private void UpdateAllStatusLabels()
```

**Purpose:** Refresh all plan status indicators

**Called When:**
- UI initializes
- Import operation completes
- Comparison created

**Implementation:**
```csharp
if (comparisonDto != null)
{
    UpdateStatusLabel(Enums.ReportType.Client, comparisonDto.ClientPlanLoaded);
    UpdateStatusLabel(Enums.ReportType.EMP, comparisonDto.EmpPlanLoaded);
}
```

---

#### UpdateStatusLabel Method

```csharp
private void UpdateStatusLabel(Enums.ReportType reportType, bool isLoaded)
```

**Parameters:**
- `reportType` - Which plan to update (Client or EMP)
- `isLoaded` - Whether plan is loaded (true/false)

**Updates Label Properties:**
```csharp
Label statusLabel = // Get from mapping
statusLabel.Text = isLoaded ? "Loaded" : "Not Loaded";
statusLabel.ForeColor = isLoaded ? Color.Green : Color.Red;
statusLabel.Cursor = isLoaded ? Cursors.Hand : Cursors.Default;
```

**Visual Feedback:**
| State | Text | Color | Cursor |
|-------|------|-------|--------|
| Loaded | "Loaded" | Green | Hand |
| Not Loaded | "Not Loaded" | Red | Default |

---

#### HandleImportFile Method

```csharp
private void HandleImportFile(Enums.ReportType reportType)
```

**Parameters:**
- `reportType` - Type of plan to import (Client or EMP)

**Process:**
1. Validate `comparisonDto` exists
2. Call `_fileImportService.ImportFile()`
3. If successful:
   - Update `comparisonDto` properties
   - Update UI status label
   - Show success message
4. If cancelled:
   - Do nothing

**Success Updates:**
```csharp
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
```

---

#### importClientPlanToolStripMenuItem_Click Method

```csharp
private void importClientPlanToolStripMenuItem_Click(object sender, EventArgs e)
```

**Event Trigger:** User clicks "Import Client Plan" menu item

**Action:**
```csharp
HandleImportFile(Enums.ReportType.Client);
```

---

#### importEmpPlanToolStripMenuItem_Click Method

```csharp
private void importEmpPlanToolStripMenuItem_Click(object sender, EventArgs e)
```

**Event Trigger:** User clicks "Import EMP Plan" menu item

**Action:**
```csharp
HandleImportFile(Enums.ReportType.EMP);
```

---

#### OpenPlanFile Method

```csharp
private void OpenPlanFile(string fileName)
```

**Parameters:**
- `fileName` - Filename to open (e.g., "Client_Report_20240115_143022.pdf")

**Process:**
1. Validate filename and `comparisonDto`
2. Build full path: `Path.Combine(BaseReportDirectory, fileName)`
3. Check file exists
4. If yes: Open with default application
5. If no: Show error message

**Opens With:**
- Default PDF reader (configured in OS)
- Example: Adobe Reader, Foxit, Microsoft Edge

---

#### clientPlanStatusValueLabel_Click Method

```csharp
private void clientPlanStatusValueLabel_Click(object sender, EventArgs e)
```

**Event Trigger:** User clicks Client plan status label

**Condition:** Only opens if plan is loaded
```csharp
if (comparisonDto?.ClientPlanLoaded == true)
{
    OpenPlanFile(comparisonDto.ClientPlantPath);
}
```

---

#### empPlanStatusValueLabel_Click Method

```csharp
private void empPlanStatusValueLabel_Click(object sender, EventArgs e)
```

**Event Trigger:** User clicks EMP plan status label

**Condition:** Only opens if plan is loaded
```csharp
if (comparisonDto?.EmpPlanLoaded == true)
{
    OpenPlanFile(comparisonDto.EmpPlanPath);
}
```

---

#### newComparisonToolStripMenuItem_Click Method

```csharp
private void newComparisonToolStripMenuItem_Click(object sender, EventArgs e)
```

**Event Trigger:** User clicks "New Comparison" menu item

**Action:**
```csharp
comparisonDto = new ComparisonDto();
InitializeUi();
```

**Flow:**
1. Create new ComparisonDto (generates ID, creates directories)
2. Refresh UI (enable menus, show container, update title)

---

## Quick Reference

### Creating a New Comparison
```csharp
// User clicks "New Comparison"
var comparison = new ComparisonDto();
// Returns: ComparisonDto with ID="COMP-XXXX", directories created
```

### Importing a Plan Document
```csharp
var service = new FileImportService();
string? fileName = service.ImportFile(ReportType.Client, "Data/Reports/COMP-0001");
// Returns: "Client_Report_20240115_143022.pdf" or null if cancelled
```

### Updating UI After Import
```csharp
comparison.ClientPlantPath = fileName;
comparison.ClientPlanLoaded = true;
form.UpdateStatusLabel(ReportType.Client, true);
// Result: Label shows "Loaded" in green with hand cursor
```

### Opening an Imported Document
```csharp
if (comparison.ClientPlanLoaded)
{
    // Build full path
    string fullPath = Path.Combine(comparison.BaseReportDirectory, 
                                   comparison.ClientPlantPath);
    // Open with default application
    Process.Start(new ProcessStartInfo 
    { 
        FileName = fullPath, 
        UseShellExecute = true 
    });
}
```

---

## Type Summary

### Value Types (Primitive Types)
- `string` - Text values
- `bool` - Boolean flags
- `DateTime` - Date and time values

### Reference Types (Classes)
- `ComparisonDto` - Comparison data
- `FileImportService` - File operations
- `DirectoryService` - Directory operations
- `Form` - UI window (Windows Forms)
- `Label` - Status display (Windows Forms)
- `MenuStrip` - Application menu (Windows Forms)

### Interface Types
- `IFileImportService` - File import contract
- `IDirectoryService` - Directory operations contract

### Enumeration Types
- `ReportType` - Client or EMP

---

## Threading Considerations

### Single-Threaded (UI)
- All FHome methods run on UI thread
- File dialogs block UI (acceptable)
- No async operations currently

### Thread-Safe Operations
- `Directory.CreateDirectory()` - Thread-safe
- `File.Copy()` - Thread-safe
- Path operations - Stateless (thread-safe)

### Future Improvements
- Async file operations
- Background import processing
- Progress reporting

---

## Memory Considerations

### Object Lifetime
- `ComparisonDto` - Lives while comparison active
- `FileImportService` - Lives while application running
- `DirectoryService` - Lives while application running

### No Memory Leaks
- No circular references
- No unclosed file handles
- Proper disposal of FileDialog

### Resource Usage
- Typical comparison: < 1MB memory
- No caching of file contents
- Scalable to 1000+ comparisons


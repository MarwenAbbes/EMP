# Developer Guide & Setup Instructions

## Table of Contents
1. [Getting Started](#getting-started)
2. [Project Setup](#project-setup)
3. [Building & Running](#building--running)
4. [Development Workflow](#development-workflow)
5. [Debugging Guide](#debugging-guide)
6. [Common Tasks](#common-tasks)
7. [Testing Guidelines](#testing-guidelines)
8. [Troubleshooting](#troubleshooting)

---

## Getting Started

### Prerequisites

**System Requirements:**
- Windows 7 or later (for Windows Forms)
- 4GB RAM minimum
- 500MB disk space for development tools

**Software Requirements:**
- .NET 9.0 SDK (or later)
- Visual Studio 2022 (Community or higher) OR JetBrains Rider
- Git (for version control)

### Installation Steps

#### 1. Install .NET 9.0 SDK

**From Official Source:**
1. Visit https://dotnet.microsoft.com/download
2. Download .NET 9.0 SDK
3. Run installer
4. Verify: `dotnet --version` (should show 9.0.x)

#### 2. Install IDE (Choose One)

**Option A: Visual Studio 2022**
1. Download from https://visualstudio.microsoft.com/
2. Run installer
3. Select workloads:
   - .NET desktop development
   - .NET multi-platform app development
4. Complete installation

**Option B: JetBrains Rider**
1. Download from https://www.jetbrains.com/rider/
2. Run installer
3. Follow setup wizard
4. License or trial activation

**Option C: VS Code**
1. Download from https://code.visualstudio.com/
2. Install C# Dev Kit extension
3. Install .NET Install Tool extension

#### 3. Clone Repository

```bash
# Clone the repository
git clone https://github.com/MarwenAbbes/EMP.git

# Navigate to project directory
cd EMP

# Verify structure
dir
```

**Expected Structure:**
```
EMP/
??? Mira.Core/
??? Mira.UI/
??? EMP.sln
??? .git/
```

---

## Project Setup

### Opening the Project

#### Visual Studio 2022

1. Launch Visual Studio
2. Click "Open a project or solution"
3. Navigate to `EMP/EMP.sln`
4. Click "Open"
5. Wait for solution to load (first time may take 1-2 minutes)

#### JetBrains Rider

1. Launch Rider
2. Click "Open"
3. Navigate to `EMP/` directory
4. Click "Open"
5. Trust project when prompted

#### VS Code

1. Launch VS Code
2. File ? Open Folder
3. Navigate to `EMP/`
4. Click "Select Folder"

### Installing Dependencies

**Restore NuGet Packages:**

```bash
# From project root
dotnet restore
```

Or automatically done by IDE on project load.

### Understanding Project Structure

```
EMP/
?
??? Mira.Core/                          # Core library (no UI)
?   ??? Mira.Core.csproj               # Project file
?   ??? Properties/
?   ??? obj/                           # Build output (ignore)
?   ??? bin/                           # Compiled output (ignore)
?   ?
?   ??? Enums.cs                       # Enumerations (9 lines)
?   ??? Constants.cs                   # Constants (18 lines)
?   ??? Utils.cs                       # Utilities (52 lines)
?   ?
?   ??? DTO/
?   ?   ??? ComparisonDto.cs          # Data model (48 lines)
?   ?
?   ??? Services/
?       ??? DirectoryService.cs       # Directory management (35 lines)
?       ??? FileImportService.cs      # File operations (58 lines)
?
??? Mira.UI/                           # Windows Forms UI
?   ??? Mira.UI.csproj                # Project file
?   ??? Properties/
?   ??? obj/                          # Build output (ignore)
?   ??? bin/                          # Compiled output (ignore)
?   ?
?   ??? FHome.cs                      # Main form code-behind (160 lines)
?   ??? FHome.Designer.cs             # Auto-generated UI (520 lines)
?   ??? FHome.resx                    # Form resources
?   ?
?   ??? Program.cs                    # Entry point (15 lines)
?
??? EMP.sln                            # Solution file
??? .gitignore                         # Git ignore rules
??? README.md                          # Project documentation

```

### Project Configuration

#### Mira.Core.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
</Project>
```

**Properties:**
- `TargetFramework` - .NET 9.0 for Windows
- `ImplicitUsings` - Auto-import common namespaces
- `Nullable` - Enable nullable reference types
- `UseWindowsForms` - Enable Windows Forms support

#### Mira.UI.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWindowsForms>true</UseWindowsForms>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Mira.Core\Mira.Core.csproj" />
  </ItemGroup>
</Project>
```

**Key Settings:**
- `OutputType` - WinExe (Windows executable)
- `ProjectReference` - Dependency on Mira.Core

---

## Building & Running

### Command Line (Cross-IDE)

#### Build Solution
```bash
# Debug build
dotnet build

# Release build
dotnet build --configuration Release

# Verbose output
dotnet build --verbosity diagnostic
```

#### Run Application
```bash
# Run UI project
dotnet run --project Mira.UI

# Run with arguments
dotnet run --project Mira.UI -- --help
```

#### Run Tests (Future)
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "TestClassName"

# Verbose test output
dotnet test --verbosity=diagnostic
```

### IDE-Specific (Recommended)

#### Visual Studio 2022

**Build:**
- Build ? Build Solution (Ctrl+Shift+B)
- Build ? Rebuild Solution (Clean then Build)

**Run:**
- Debug ? Start Debugging (F5)
- Debug ? Start Without Debugging (Ctrl+F5)

**Stop:**
- Debug ? Stop Debugging (Shift+F5)

**Debug Features:**
- Breakpoints: Click line number gutter
- Step Into: F11
- Step Over: F10
- Continue: F5

#### JetBrains Rider

**Build:**
- Build ? Build Project (Ctrl+F9)
- Build ? Rebuild Project

**Run:**
- Run ? Run (Shift+F10)
- Run ? Debug (Shift+F9)

**Stop:**
- Red stop button in run panel

**Debug Features:**
- Breakpoints: Click line number gutter
- Step Into: F7
- Step Over: F8
- Continue: F9

---

## Development Workflow

### Git Workflow

#### Initial Clone
```bash
git clone https://github.com/MarwenAbbes/EMP.git
cd EMP
git branch -a  # View all branches
```

#### Creating Feature Branch
```bash
# Current branch: 6-create-the-comparison-dto
git checkout -b 7-feature-name

# Or checkout existing branch
git checkout 6-create-the-comparison-dto
```

#### Making Changes
```bash
# View status
git status

# Stage changes
git add Mira.Core/NewFile.cs
git add -A  # Stage all

# Commit changes
git commit -m "feat: add new feature description"

# View log
git log --oneline
```

#### Pushing Changes
```bash
# Push to remote
git push origin 7-feature-name

# Create pull request on GitHub
# Go to https://github.com/MarwenAbbes/EMP/pulls
```

#### Syncing with Main
```bash
# Fetch latest
git fetch origin

# Merge main into current branch
git merge origin/main

# Or rebase (cleaner history)
git rebase origin/main
```

### Code Style

#### Naming Conventions

**Classes & Namespaces**
```csharp
public class ComparisonDto { }        // PascalCase
public class FileImportService { }    // PascalCase
public interface IFileImportService { } // I + PascalCase for interfaces
```

**Methods & Properties**
```csharp
public string GetNextComparisonId() { }  // PascalCase
public string ProjectName { get; set; } // PascalCase
```

**Fields (Private)**
```csharp
private string _id;                  // Underscore prefix + camelCase
private readonly string _constant;   // Readonly fields underscore + camelCase
```

**Local Variables**
```csharp
string fileName = "report.pdf";     // camelCase
bool isLoaded = true;               // camelCase (no prefix)
```

**Constants**
```csharp
public const string PREFIX = "COMP";  // UPPER_CASE (constants uppercase)
```

#### Formatting

**Indentation**
```csharp
// Use 4 spaces (not tabs)
public class Example
{
    public void Method()
    {
        if (condition)
        {
            // 4 spaces per level
        }
    }
}
```

**Braces**
```csharp
// Allman style (opening brace on new line)
public void Method()
{
    if (condition)
    {
        // code
    }
}
```

**Documentation**
```csharp
/// <summary>
/// Short description of method/class
/// </summary>
/// <param name="parameter">Description of parameter</param>
/// <returns>Description of return value</returns>
public string GetData(string parameter)
{
    return parameter;
}
```

#### Code Organization

**File Structure**
```csharp
using System;              // Imports at top
using System.Collections;

namespace Mira.Core        // Namespace
{
    /// <summary>Documentation</summary>
    public class MyClass   // Class definition
    {
        // Fields (private first)
        private string _field;
        
        // Properties (public)
        public string Property { get; set; }
        
        // Constructor
        public MyClass() { }
        
        // Methods (public first)
        public void PublicMethod() { }
        
        // Private methods
        private void PrivateMethod() { }
    }
}
```

---

## Debugging Guide

### Setting Breakpoints

**Visual Studio**
1. Click left margin (gutter) next to line number
2. Red dot appears
3. Run application (F5)
4. Execution pauses at breakpoint

**Rider**
1. Click left margin next to line number
2. Red circle appears
3. Run in debug mode (Shift+F9)
4. Execution pauses at breakpoint

### Inspecting Variables

**During Debug Session**

**Visual Studio:**
- Hover over variable to see value
- Use Watch window: Debug ? Windows ? Watch
- Use Locals window: Debug ? Windows ? Locals
- Use Quick Watch: Shift+Alt+Q

**Rider:**
- Hover over variable to see value
- Variables tab shows all locals
- Use Evaluate Expression: Alt+F9

### Common Debugging Scenarios

#### Scenario 1: Import Not Working
```csharp
// Set breakpoint in HandleImportFile
private void HandleImportFile(Enums.ReportType reportType)
{
    // Breakpoint here - step through
    if (comparisonDto == null)  // Check condition
    {
        return;
    }
    
    // Watch _fileImportService to verify it exists
    string? importedFileName = _fileImportService.ImportFile(reportType, 
                                                             comparisonDto.BaseReportDirectory);
    
    if (importedFileName != null)
    {
        // Verify fileName was returned
    }
}
```

#### Scenario 2: File Not Found
```csharp
// Set breakpoint in OpenPlanFile
private void OpenPlanFile(string fileName)
{
    string filePath = Path.Combine(comparisonDto.BaseReportDirectory, fileName);
    
    // Check filePath value
    // Verify directory exists: Debug ? Immediate Window
    // > System.IO.Directory.Exists(comparisonDto.BaseReportDirectory)
    
    if (File.Exists(filePath))  // Check result
    {
        // ...
    }
}
```

### Debug Output Window

**Visual Studio:**
1. Debug ? Windows ? Output
2. Diagnostic output appears here

**Rider:**
1. View ? Tool Windows ? Debug Console
2. Runtime output shown here

**Add Debug Output:**
```csharp
System.Diagnostics.Debug.WriteLine($"Value: {value}");
Console.WriteLine("Debug message");
```

---

## Common Tasks

### Adding a New Feature

#### 1. Create Feature Branch
```bash
git checkout -b feature/new-feature-name
```

#### 2. Implement Changes

Example: Add save functionality

```csharp
// In Mira.Core/Services/SaveService.cs
public class SaveService
{
    public void SaveComparison(ComparisonDto comparison)
    {
        // Implementation
    }
}

// Update Mira.UI/FHome.cs
private void saveComparisonToolStripMenuItem_Click(object sender, EventArgs e)
{
    if (comparisonDto != null)
    {
        var saveService = new SaveService();
        saveService.SaveComparison(comparisonDto);
    }
}
```

#### 3. Build & Test
```bash
dotnet build
dotnet run --project Mira.UI
```

#### 4. Commit & Push
```bash
git add -A
git commit -m "feat: add save functionality"
git push origin feature/new-feature-name
```

### Modifying Constants

#### Before:
```csharp
// Magic string scattered in code
MessageBox.Show("Success!", "Success");
```

#### After:
```csharp
// 1. Add to Constants.cs
public const string SUCCESS_TITLE = "Success";
public const string SUCCESS_MESSAGE = "Operation completed!";

// 2. Use constant
MessageBox.Show(Constants.SUCCESS_MESSAGE, Constants.SUCCESS_TITLE);
```

**Benefits:**
- Single source of truth
- Easy to find and change
- Type-safe
- Better maintainability

### Updating Service Implementation

#### Scenario: Change file import behavior

```csharp
// In FileImportService.cs
public string? ImportFile(Enums.ReportType reportType, string destinationDirectory)
{
    using (var openFileDialog = new OpenFileDialog())
    {
        openFileDialog.Title = Constants.SELECT_REPORT_FILE_TITLE;
        openFileDialog.Filter = Constants.PDF_FILTER;
        
        // NEW: Add multi-select capability
        openFileDialog.Multiselect = true;
        
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            // Handle multiple files
            var fileNames = new List<string>();
            foreach (var fileName in openFileDialog.FileNames)
            {
                fileNames.Add(CopyFileToDestination(fileName, reportType, destinationDirectory));
            }
            
            return string.Join(";", fileNames);  // Return comma-separated
        }
    }
    
    return null;
}
```

### Adding a New Menu Item

#### 1. In FHome.Designer.cs (Generated)
Add via designer:
- Right-click menu ? Edit Items
- Add new ToolStripMenuItem
- Set Name: `newMenuItem`
- Set Text: "New Menu Text"
- Wire Click event

#### 2. In FHome.cs
```csharp
private void newMenuItem_Click(object sender, EventArgs e)
{
    // Handle menu click
    MessageBox.Show("Menu clicked!");
}
```

---

## Testing Guidelines

### Unit Testing (Future)

#### Creating Test Project
```bash
# Create test project
dotnet new xunit -n Mira.Tests

# Add reference to Mira.Core
dotnet add Mira.Tests reference Mira.Core
```

#### Writing Tests
```csharp
// In Mira.Tests/UtilsTests.cs
using Xunit;
using Mira.Core;

public class UtilsTests
{
    [Fact]
    public void GetNextComparisonId_EmptyDirectory_ReturnsFirstId()
    {
        // Arrange
        // Act
        string result = Utils.GetNextComparisonId();
        
        // Assert
        Assert.Equal("COMP-0001", result);
    }
    
    [Fact]
    public void GetNextComparisonId_WithExistingComparisons_ReturnsNextId()
    {
        // Arrange - Create existing comparison directories
        // Act
        string result = Utils.GetNextComparisonId();
        
        // Assert
        Assert.StartsWith("COMP-", result);
    }
}
```

#### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter "GetNextComparisonId"
```

### Manual Testing Checklist

#### New Comparison
- [ ] Click "New Comparison"
- [ ] Verify ID is generated (COMP-XXXX)
- [ ] Verify container is visible
- [ ] Verify menus are enabled

#### Import Client Plan
- [ ] Click "Import Client Plan"
- [ ] Select a PDF file
- [ ] Verify status shows "Loaded" in green
- [ ] Verify file is copied to correct directory
- [ ] Verify status label is clickable (hand cursor)

#### Import EMP Plan
- [ ] Click "Import EMP Plan"
- [ ] Select a PDF file
- [ ] Verify status shows "Loaded" in green
- [ ] Verify cursor is hand on label

#### Open Imported Document
- [ ] Click "Loaded" status for Client plan
- [ ] Verify PDF opens in default reader
- [ ] Close PDF reader
- [ ] Click "Loaded" status for EMP plan
- [ ] Verify EMP PDF opens

#### Error Scenarios
- [ ] Click "Import Plan" without active comparison
  - Expected: Nothing happens
- [ ] Import same file twice
  - Expected: File copy fails safely
- [ ] Click "Loaded" when no comparison active
  - Expected: Nothing happens
- [ ] Delete imported file manually
- [ ] Click "Loaded" status
  - Expected: Error message shows

---

## Troubleshooting

### Build Issues

#### Error: "The target platform must be set to Windows"
**Solution:**
```xml
<!-- In .csproj -->
<TargetFramework>net9.0-windows</TargetFramework>  <!-- Add -windows -->
<UseWindowsForms>true</UseWindowsForms>
```

#### Error: "project.json not found"
**Solution:**
- Delete `obj/` and `bin/` folders
- Clean and rebuild: `dotnet clean && dotnet build`

#### Error: "NuGet package version not found"
**Solution:**
```bash
# Restore with latest versions
dotnet restore --no-cache
dotnet build --no-restore
```

### Runtime Issues

#### File Dialog Not Appearing
**Cause:** Running as console app instead of WinExe

**Solution:**
```xml
<!-- In Mira.UI.csproj -->
<OutputType>WinExe</OutputType>  <!-- Ensure this is set -->
```

#### File Not Found When Importing
**Debug Steps:**
```csharp
// Add debug output
System.Diagnostics.Debug.WriteLine($"Directory: {comparisonDto.BaseReportDirectory}");
System.Diagnostics.Debug.WriteLine($"File: {fileName}");

// Check directory exists
if (!Directory.Exists(comparisonDto.BaseReportDirectory))
{
    MessageBox.Show("Directory does not exist!");
}
```

#### "Comparison is null" Error
**Cause:** Trying to use comparison before creating one

**Solution:**
```csharp
// Always check null
if (comparisonDto != null)
{
    // Safe to use
}

// Or use null-coalescing
if (comparisonDto?.ClientPlanLoaded == true)
{
    // Safe
}
```

### Performance Issues

#### Slow File Dialog
- This is normal on first open
- Subsequent opens are faster (caching)
- No action needed

#### Slow Directory Scanning
- Occurs when Reports folder has many comparisons
- Only happens on app startup
- Consider indexing for 1000+ comparisons

### Debugging Tips

#### Enable Verbose Logging
```csharp
// In Program.cs
static void Main()
{
    // Add diagnostic output
    System.Diagnostics.Trace.WriteLine("Application started");
    
    ApplicationConfiguration.Initialize();
    Application.Run(new FHome());
}
```

#### Use Immediate Window
**Visual Studio:**
1. Debug ? Windows ? Immediate (Ctrl+Alt+I)
2. Enter: `System.IO.Directory.Exists(@"C:\path")`
3. Press Enter to execute

**Rider:**
1. Debug Console tab
2. Run debugger expressions

#### Inspect File System
```csharp
// List comparisons
var dirs = Directory.GetDirectories(Paths.ReportsDirectory);
foreach (var dir in dirs)
{
    System.Diagnostics.Debug.WriteLine(dir);
}
```

---

## IDE Shortcuts Reference

### Visual Studio 2022

| Task | Shortcut |
|------|----------|
| Build Solution | Ctrl+Shift+B |
| Start Debugging | F5 |
| Stop Debugging | Shift+F5 |
| Step Into | F11 |
| Step Over | F10 |
| Toggle Breakpoint | F9 |
| Quick Watch | Shift+Alt+Q |
| Go to Definition | F12 |
| Find References | Shift+F12 |
| Format Document | Ctrl+K, Ctrl+D |
| Comment Line | Ctrl+K, Ctrl+C |

### JetBrains Rider

| Task | Shortcut |
|------|----------|
| Build Project | Ctrl+F9 |
| Run Program | Shift+F10 |
| Debug Program | Shift+F9 |
| Stop | Ctrl+F2 |
| Step Into | F7 |
| Step Over | F8 |
| Toggle Breakpoint | Ctrl+F8 |
| Evaluate | Alt+F9 |
| Go to Declaration | Ctrl+B |
| Find Usages | Alt+F7 |
| Reformat Code | Ctrl+Alt+L |

---

## Additional Resources

### Official Documentation
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [C# Language Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [Windows Forms](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

### Community Resources
- [Stack Overflow](https://stackoverflow.com/questions/tagged/c%23)
- [GitHub Discussions](https://github.com/MarwenAbbes/EMP/discussions)
- [MSDN Forums](https://social.msdn.microsoft.com/)

### Tools & Extensions
- [Visual Studio Extensions](https://marketplace.visualstudio.com/)
- [NuGet Package Manager](https://www.nuget.org/)
- [GitHub Desktop](https://desktop.github.com/)


# MIRA - Technical Plan Review Application

A Windows Forms application for managing, importing, and comparing technical plans (Client Plans and EMP Plans) with export capabilities in multiple formats.

---

## ?? Project Overview

**MIRA** stands for "Revue Technique des Plans" (Technical Review of Plans). This application is designed to facilitate the comparison and management of technical documentation, specifically comparing Client Plans with EMP (Electromagnetic Pulse or similar engineering specifications) Plans.

### Key Features
- ?? Create, open, and manage comparison sessions
- ?? Import technical plans (Client Plan and EMP Plan)
- ?? Compare two technical plans and identify differences
- ?? View comparison results in a data grid
- ?? Save comparison sessions in multiple formats
- ?? Export results to Excel, Word, and CSV formats

---

## ??? Project Structure

```
EMP (Root)
??? Mira.Core/           # Core business logic and services
?   ??? Mira.Core.csproj
?   ??? Class1.cs        # (To be implemented)
?
??? Mira.UI/             # Windows Forms user interface
?   ??? Mira.UI.csproj
?   ??? Program.cs       # Application entry point
?   ??? FHome.cs         # Main form logic
?   ??? FHome.Designer.cs # Form designer (auto-generated)
?   ??? FHome.resx       # Form resources
?
??? README.md            # This file
```

---

## ??? Technology Stack

- **Framework**: .NET 9
- **C# Version**: 13.0
- **UI Framework**: Windows Forms
- **Target Platforms**: Windows (.NET 9-windows)
- **Package Management**: NuGet

### Project Configurations

#### Mira.Core
- **Target Framework**: .NET 9
- **Type**: Class Library
- **Nullable**: Enabled
- **Implicit Usings**: Enabled

#### Mira.UI
- **Target Framework**: .NET 9-windows
- **Type**: Windows Forms Application (WinExe)
- **Nullable**: Enabled
- **Implicit Usings**: Enabled
- **UseWindowsForms**: Enabled

---

## ?? User Interface Overview

### Main Form: FHome

The main application window (`FHome`) is organized into a container with three primary sections:

#### 1. **General Information Section** (`generalInfoGroupBox`)
Contains metadata for the current comparison:
- **Project Name** (`projectNameTextBox`) - Name of the project being reviewed
- **Responsible Person** (`responsiblePersonTextBox`) - Person responsible for the comparison
- **EMP Plan Reference** (`empPlanRefTextBox`) - Reference number for the EMP plan
- **Client Plan Reference** (`clientPlanRefTextBox`) - Reference number for the client plan
- **Comparison Date** (`comparisonDateTextBox`) - Date of the comparison
- **Compare Button** (`compareButton`) - Triggers the comparison operation

**Layout**: Two rows with centered, uppercase text input fields.

#### 2. **Plan Status Section** (`planStatusGroupBox`)
Displays the import status of both plans with visual indicators:
- **Client Plan Status** 
  - Label: "Plan Client est :"
  - Status Value: Shows "Introuvable" (Not Found) in red if import is incomplete
- **EMP Plan Status**
  - Label: "Plan EMP est :"
  - Status Value: Shows "Introuvable" (Not Found) in red if import is incomplete

**Visual Design**: Status values display in red text when plans are not found, providing clear visual feedback.

#### 3. **Comparison Results Section** (`comparisonResultsGroupBox`)
Displays the comparison analysis results:
- **Data Grid View** (`comparisonDataGridView`) - Shows detailed row-by-row comparison results
- Auto-sized column headers
- Docked to fill available space

**Layout**: Located below the status section, occupying the remaining form space.

---

## ?? Menu Structure

### Comparison Menu (`comparisonToolStripMenuItem`)
File and comparison management operations:
- **Nouveau** (`newComparisonToolStripMenuItem`) - Create a new comparison session
- **Open** (`openComparisonToolStripMenuItem`) - Open a saved comparison
- **Enregistrer** (`saveComparisonToolStripMenuItem`) - Save current comparison
- **Enregistrer Sous** (`saveAsComparisonToolStripMenuItem`) - Save with a new name
- **Supprimer** (`deleteComparisonToolStripMenuItem`) - Delete a comparison
- **Quitter** (`exitApplicationToolStripMenuItem`) - Exit the application

### Review Menu (`reviewToolStripMenuItem`)
Import operations for technical plans:
- **Importer** (`importToolStripMenuItem`)
  - **Plan Client** (`importClientPlanToolStripMenuItem`) - Import client plan document
  - **Plan EMP** (`importEmpPlanToolStripMenuItem`) - Import EMP plan document

### Export Menu (`exportToolStripMenuItem`)
Export comparison results in various formats:
- **Format Excel** (`exportExcelFormatToolStripMenuItem`) - Export as `.xlsx`
- **Format Word** (`exportWordFormatToolStripMenuItem`) - Export as `.docx`
- **Format Csv** (`exportCsvFormatToolStripMenuItem`) - Export as `.csv`

---

## ?? Current Development Progress

### ? Completed (Phase 1: UI Design)

1. **Project Structure**
   - Two-tier architecture: Mira.Core (Business Logic) and Mira.UI (Presentation)
   - Both projects targeting .NET 9

2. **User Interface Design**
   - Main form (`FHome`) fully designed with Windows Forms
   - All controls professionally arranged in logical groupings
   - Form dimensions: 800×450 pixels (scalable)
   - Title: "Mira- Revue Technique des Plans"

3. **Control Naming Refactoring** ? Latest
   - All generic control names renamed to meaningful identifiers
   - Menu items, groupboxes, textboxes, labels all properly named
   - Build successful with no compilation errors

4. **Application Entry Point**
   - `Program.cs` configured to launch `FHome` as the main form

5. **Resource Management**
   - Form resources (`FHome.resx`) properly configured

---

### ? Pending Implementation

#### Phase 2: Core Business Logic
- [ ] Create data models (Plan, Comparison, ComparisonResult)
- [ ] Implement comparison engine
- [ ] Create file I/O services
- [ ] Implement validation services

#### Phase 3: Event Handlers & Form Logic
- [ ] Wire up all menu item click events
- [ ] Implement file dialogs for open/save operations
- [ ] Implement import functionality
- [ ] Implement export functionality (Excel, Word, CSV)
- [ ] Implement compare button logic

#### Phase 4: Data & Storage
- [ ] Design data models
- [ ] Implement file-based storage (JSON/XML)
- [ ] Create repository pattern

#### Phase 5: UI Enhancements
- [ ] Add progress indicators
- [ ] Improve error handling and messaging
- [ ] Add input validation
- [ ] Add status bar with feedback
- [ ] Add icons to menu items and buttons

#### Phase 6: Testing & Documentation
- [ ] Unit tests for core logic
- [ ] Integration tests
- [ ] User documentation
- [ ] Developer guide

---

## ?? Getting Started

### Prerequisites
- .NET 9 SDK or later
- Visual Studio 2022 or JetBrains Rider
- Windows operating system

### Building the Project

```bash
# Clone the repository
git clone https://github.com/MarwenAbbes/EMP.git
cd EMP

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project Mira.UI
```

---

## ?? Dependencies

### Current
- .NET 9 (framework)
- Windows Forms (included with .NET)

### Planned
- EPPlus or ClosedXML (Excel export)
- OpenXML SDK or DocX (Word export)
- CsvHelper (CSV operations)

---

## ?? Typical User Workflow

1. Create a new comparison or open an existing one
2. Enter project information and personnel details
3. Import the Client Plan document
4. Import the EMP Plan document
5. Click "Comparer" to run the comparison
6. Review the results in the data grid
7. Export the results in the desired format
8. Save the comparison session

---

## ?? Git Information

**Repository**: https://github.com/MarwenAbbes/EMP
**Current Branch**: `1-create-the-ui`
**Owner**: Marwen Abbes

---

## ?? Project Statistics

| Metric | Value |
|--------|-------|
| Projects | 2 |
| .NET Target | .NET 9 |
| C# Version | 13.0 |
| Main UI Controls | 30+ (all named) |
| Menu Items | 16+ |
| Development Phase | Early (UI Complete) |
| Build Status | ? Passing |

---

## ?? Version History

### v0.2.0 (Current)
- ? UI control naming refactored
- ?? README documentation created
- ?? Development roadmap added

### v0.1.0 
- ? Initial project setup
- ?? Main form UI designed

---

**Status**: Early Development - UI Phase Complete
**Next**: Core business logic implementation

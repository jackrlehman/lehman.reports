# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a **Blazor WebAssembly** application for generating customizable PDF reports. Currently implements Mobile App Store Performance Reports (Version 1.01) with support for iOS and Android platform metrics.

**Tech Stack:**
- .NET 9.0 Blazor WebAssembly (client-side execution)
- QuestPDF for PDF generation (requires Community license for non-commercial use)
- iText7 for PDF parsing/import
- Bootstrap 5 + custom CSS for UI

## Build & Run Commands

```bash
# Build the project
dotnet build

# Run the application (compiles to WASM, runs on https://localhost:5001)
dotnet run

# Restore packages after project changes
dotnet restore
```

**Note:** Application runs entirely in the browser after initial load. PDF generation and parsing happen client-side.

## Architecture Overview

### Report Versioning System

**Critical:** All reports include a version number (currently "1.01") stored in `MobileAppReportConfig.Version`. This appears in the PDF footer and enables:
- Forward compatibility checks when importing PDFs
- Version-specific parsing logic in `PdfReportParser.cs`
- Tracking report format changes over time

When creating new report versions, increment the version and ensure `PdfReportParser.cs` can handle legacy formats.

### Core Architecture Pattern

The application uses a **generic report generator pattern** designed for extensibility:

1. **Model Layer** (`Models/`): Configuration classes with nullable doubles for metrics
   - All numeric values use `double?` (nullable) for optional data
   - Computed properties automatically calculate percentage changes
   - Models contain business logic methods (e.g., `SyncPlatformComparison()`, `CalculateDownloadSourcePercentages()`)

2. **Service Layer** (`Services/`):
   - `IReportGenerator<TConfig>` interface for type-safe report generators
   - `MobileAppReportGenerator` implements QuestPDF document composition
   - `PdfReportParser` uses regex patterns to extract data from previously generated PDFs

3. **Component Layer** (`Components/Pages/`):
   - Blazor pages use two-way binding with `InputNumber<double?>` components
   - Readonly calculated fields display auto-computed values
   - JavaScript interop via `download.js` for file operations

### Auto-Calculation System

**Important:** Several values auto-calculate and should NEVER have manual input fields:

1. **Percentage Changes**: Computed properties in model classes automatically calculate `(current - last) / last * 100`
2. **Download Source Percentages**: `IOSMetrics.CalculateDownloadSourcePercentages()` computes percentages from raw download counts
3. **Platform Comparison Metrics**: `MobileAppReportConfig.SyncPlatformComparison()` populates from iOS/Android data

These calculations happen:
- On UI value change (via `OnDownloadSourceChanged()` handler)
- Before PDF generation (in `GenerateReport()` method)

### PDF Conditional Rendering

The PDF generator has **two rendering modes** controlled by `IncludeLastPeriodData`:

- **With comparison mode**: Tables include current, last period, and % change columns
- **Without comparison mode**: Tables show only current period columns (2-3 columns vs 4-7)

All table composition methods (`ComposeIOSMetricsTable`, `ComposeDownloadSourcesTable`, etc.) check this flag and adjust:
- Column definitions
- Header cells
- Row cell rendering via `AddTableRow()` and `AddPlatformComparisonRow()` helper methods

### Import/Export Flow

**PDF Generation with Embedded JSON:**
1. QuestPDF generates the visual PDF report
2. JSON-serialized `MobileAppReportConfig` is embedded as invisible white text: `<!--REPORT_CONFIG_JSON:{json}-->`
3. PDFs are self-contained with both visual report and structured data

**PDF Import Workflow:**
1. User uploads PDF → `HandlePdfImport()` reads file bytes using `CopyToAsync()`
2. `PdfReportParser.ParseMobileAppReport()` extracts text using iText7 (read-only, no licensing issues)
3. Regex finds the JSON marker `<!--REPORT_CONFIG_JSON:...-->` and extracts embedded JSON
4. JSON is deserialized back to `MobileAppReportConfig` object
5. User chooses import mode via dialog:
   - **"Previous Period"**: Imported current values → new last period fields (comparison data)
   - **"Current Period"**: Imported data loaded as-is into current fields

**Import Methods:**
- `ImportToPreviousPeriod()`: Maps imported current values to last period fields for all metrics (iOS, Android, Platform Comparison)
- `ImportToCurrentPeriod()`: Replaces entire config with imported config

## Key Implementation Details

### Version Compatibility

When modifying report structure:
1. Update `Version` property in `MobileAppReportConfig.cs`
2. Test PDF import with previous version reports
3. Add version-specific parsing logic to `PdfReportParser.cs` if needed
4. Update PDF footer rendering in `MobileAppReportGenerator.ComposeFooter()`

### Adding New Report Types

To add a new report generator:
1. Create model class in `Models/` (follow nullable double pattern for metrics)
2. Implement `IReportGenerator<TYourConfig>` in `Services/`
3. Create Blazor page in `Components/Pages/` with form inputs
4. Add navigation link to `Components/Layout/NavMenu.razor`
5. Consider PDF import parser if needed

### QuestPDF Usage Pattern

QuestPDF uses fluent API for document composition. Common pattern:

```csharp
container.Table(table =>
{
    // Define columns
    table.ColumnsDefinition(columns => { ... });

    // Headers
    table.Header(header => { ... });

    // Rows (use helper methods for consistency)
    AddTableRow(table, "Metric Name", current, last, change, includeLastPeriod);
});
```

**Important:** Check `config.IncludeLastPeriodData` before rendering last period columns.

### Styling System

**IMPORTANT: All UI components must be stylized for dark mode.**

The application uses a modern dark theme with CSS variables defined in `app.css`:

**Dark Mode Color Variables:**
- `--bg-primary`: #0a0e1a (main background)
- `--bg-secondary`: #111827 (secondary backgrounds)
- `--bg-tertiary`: #1f2937 (cards, inputs)
- `--bg-elevated`: #1e293b (modals, elevated elements)
- `--text-primary`: #f1f5f9 (primary text)
- `--text-secondary`: #94a3b8 (secondary text)
- `--text-muted`: #64748b (muted text)

**Accent Colors:**
- `--accent-cyan`: #06b6d4
- `--accent-purple`: #a855f7
- `--accent-blue`: #3b82f6
- `--gradient-cyan`: linear-gradient(135deg, #06b6d4 0%, #3b82f6 100%)

**UI Guidelines:**
- All new components (modals, dialogs, cards) must use dark mode variables
- Text must be readable with sufficient contrast (use `--text-primary` for main content)
- Modals should have semi-transparent dark backdrops (`rgba(0, 0, 0, 0.75)`)
- Form inputs should use `--bg-tertiary` background
- Borders should use semi-transparent colors (e.g., `rgba(148, 163, 184, 0.2)`)
- Never use white backgrounds or black text (use dark mode variables instead)

Apply consistent styling to new components using existing variable names.

## Common Gotchas

1. **Blazor WASM Limitations**: No `HttpContext` access (removed from `Error.razor`). All processing is client-side.

2. **InputNumber Binding**: Always use `InputNumber TValue="double?"` for nullable numeric fields. Regular `InputText` won't work with double types.

3. **Auto-Calculation Timing**: Call calculation methods before PDF generation but after user input. Order matters:
   ```csharp
   config.IOSMetrics.CalculateDownloadSourcePercentages();
   config.SyncPlatformComparison();
   generator.GeneratePdf(config);
   ```

4. **PDF Table Column Count**: Must match between `ColumnsDefinition`, header cells, and data row cells. Use conditional logic for last period columns.

5. **QuestPDF Licensing**: Community license is set in code (`QuestPDF.Settings.License = LicenseType.Community`). Required for non-commercial use.

6. **iText7 PDF Reading**: Only used for reading PDFs (not writing), so no commercial license required. Extracts embedded JSON marker from PDFs generated by this application. External PDFs without the JSON marker cannot be imported.

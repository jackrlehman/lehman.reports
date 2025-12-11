# API Reference

Key classes and methods.

## Services

### IReportGenerator<TConfig>

Interface for generating reports.

```csharp
public interface IReportGenerator<TConfig> {
    Task<byte[]> GenerateAsync(TConfig config);
}
```

**Type Parameter**: Configuration class for the report

**Returns**: PDF file as byte array

**Example**:
```csharp
@inject IReportGenerator<MobileAppReportConfig> ReportGenerator

var pdf = await ReportGenerator.GenerateAsync(config);
```

### MobileAppReportGenerator

Generates mobile app performance reports.

Implements: `IReportGenerator<MobileAppReportConfig>`

**Methods**:
```csharp
public async Task<byte[]> GenerateAsync(MobileAppReportConfig config)
```

**Features**:
- iOS metrics
- Android metrics
- Download sources
- Platform comparison
- Professional PDF layout

### PdfReportParser

Extracts data from PDF files.

```csharp
public class PdfReportParser {
    public async Task<MobileAppReportConfig> ExtractDataAsync(Stream pdfStream)
}
```

**Parameters**:
- `pdfStream` - PDF file as stream

**Returns**: Parsed configuration object

**Throws**: `PdfException` if parsing fails

**Example**:
```csharp
var config = await parser.ExtractDataAsync(pdfStream);
```

### ToastService

Displays notifications to user.

```csharp
public class ToastService {
    public void ShowSuccess(string message)
    public void ShowError(string message)
    public void ShowWarning(string message)
    public void ShowInfo(string message)
}
```

**Usage**:
```razor
@inject ToastService Toast

@code {
    protected override void OnInitialized() {
        Toast.ShowSuccess("Loaded successfully!");
    }
}
```

## Models

### MobileAppReportConfig

Configuration for mobile app report.

```csharp
public class MobileAppReportConfig {
    // General
    public string AppName { get; set; }
    public string CreatedBy { get; set; }
    public string CreatorTitle { get; set; }
    public Month ReportMonth { get; set; }
    public Month PreviousMonth { get; set; }
    public string Version { get; set; }
    
    // Section toggles
    public bool IncludeIosMetrics { get; set; }
    public bool IncludeAndroidMetrics { get; set; }
    public bool IncludeDownloadSources { get; set; }
    public bool IncludePlatformComparison { get; set; }
    public bool IncludeTechnicalSpecs { get; set; }
    
    // iOS metrics
    public decimal IosCurrentDownloads { get; set; }
    public decimal IosPreviousDownloads { get; set; }
    public decimal IosDownloadsChangePercent { get; set; }
    
    // More fields...
    
    // Validation
    public bool IsValid() { }
}
```

### Month

Enum for months.

```csharp
public enum Month {
    January = 1,
    February = 2,
    // ... through December = 12
}
```

## Exceptions

### PdfException

Thrown when PDF operation fails.

```csharp
public class PdfException : Exception {
    public PdfException(string message) : base(message) { }
    public PdfException(string message, Exception inner) 
        : base(message, inner) { }
}
```

**Thrown by**:
- `MobileAppReportGenerator.GenerateAsync()`
- `PdfReportParser.ExtractDataAsync()`

### InvalidReportConfigException

Thrown when configuration is invalid.

```csharp
public class InvalidReportConfigException : Exception {
    public InvalidReportConfigException(string message) 
        : base(message) { }
}
```

**Thrown by**:
- Any validation check

### MissingReportFieldException

Thrown when required field is missing.

```csharp
public class MissingReportFieldException : Exception {
    public MissingReportFieldException(string fieldName) 
        : base($"Missing required field: {fieldName}") { }
}
```

**Thrown by**:
- Data extraction logic

## Components

### MobileAppReport.razor

Main report form.

**Parameters**: None

**Events**:
- Form submission triggers PDF generation

**Usage**:
```
Navigate to: /report
```

### Home.razor

Home page with feature overview.

**Parameters**: None

**Navigation**:
- Links to reports
- Quick start info

**Usage**:
```
Navigate to: /
```

### NavMenu.razor

Navigation menu component.

**Shows**: Links to all reports and pages

**Reusable**: Yes, included in MainLayout

## Constants

### AppConstants

Central configuration class.

```csharp
public static class AppConstants {
    public const string LocalhostUrl = "http://localhost:5000";
    public const int LocalhostPort = 5000;
    
    public const int MaxPdfFileSizeBytes = 10 * 1024 * 1024;
    
    public const string AppVersion = "1.04";
    
    public static class LogEvents {
        public const int AppStartup = 1000;
        public const int PdfGeneration = 2000;
        // ... more events
    }
}
```

**Access**:
```csharp
var url = AppConstants.LocalhostUrl;
_logger.LogInformation(LogEvents.AppStartup, "Started");
```

## Dependency Injection

### Registered Services

```csharp
// In Program.cs
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<
    IReportGenerator<MobileAppReportConfig>, 
    MobileAppReportGenerator>();
builder.Services.AddScoped<PdfReportParser>();
```

### Lifetimes

- **Scoped**: One per request/connection
- **Transient**: New instance every time
- **Singleton**: One for entire application

Most services are **Scoped**.

## JSON Serialization

### MonthJsonConverter

Converts Month enum to/from string.

```json
{
  "ReportMonth": "December",
  "PreviousMonth": "November"
}
```

### AppSizeJsonConverter

Converts app size to/from GB/MB format.

```json
{
  "AppSize": "250 MB",
  "PreviousAppSize": "230 MB"
}
```

## Logging Events

```csharp
public static class LogEvents {
    public const int AppStartup = 1000;
    public const int PdfGeneration = 2000;
    public const int PdfParsing = 2001;
    public const int FormValidation = 3000;
}
```

**Usage**:
```csharp
_logger.LogInformation(LogEvents.PdfGeneration, "Generating PDF");
```

## HTTP Endpoints

### Built-in Endpoints

| Endpoint | Purpose |
|----------|---------|
| `/` | Home page |
| `/report` | Mobile app report form |
| `/_framework/...` | Blazor framework files |
| `/lib/...` | Static libraries (Bootstrap, etc.) |

### No REST API

This is a Blazor Server app (not API).

For REST API, create separate ASP.NET Core API project.

---

**See Also**:
- [Code Patterns](../development/code-patterns.md)
- [Architecture](../development/architecture.md)

# Architecture

Understand how the application is structured.

## High-Level Overview

```
┌─────────────────────────────────────────┐
│          User Browser                   │
│  (Blazor UI + HTML/CSS/JavaScript)     │
└────────────────────┬────────────────────┘
                     │ HTTP
                     ▼
┌─────────────────────────────────────────┐
│      ASP.NET Core Web Server            │
│   (Blazor Server + .NET Runtime)        │
└──────────────────────────────────────────┘
         │
    ┌────┴─────────┬──────────────┐
    ▼              ▼              ▼
┌────────┐  ┌─────────────┐ ┌──────────┐
│Services│  │  Components │ │  Models  │
│        │  │             │ │          │
│Generate│  │ Pages       │ │ Config   │
│  PDFs  │  │ Layout      │ │ Shared   │
│        │  │ Shared      │ │ Data     │
└────────┘  └─────────────┘ └──────────┘
```

## Project Structure

### Components/
Contains all UI components:

```
Components/
├── Pages/
│   ├── Home.razor              # Home page
│   ├── MobileAppReport.razor   # Report form
│   └── Error.razor             # Error page
├── Layout/
│   ├── MainLayout.razor        # Main layout
│   └── NavMenu.razor           # Navigation
├── Shared/
│   └── [Reusable components]
├── App.razor                   # Root component
└── Routes.razor                # URL routing
```

**Technology**: Blazor Server, ASP.NET Razor components

### Models/
Data structures:

```
Models/
├── MobileAppReportConfig.cs
│   └── Report configuration data
├── MonthJsonConverter.cs
│   └── Serialize month names
└── AppSizeJsonConverter.cs
    └── Format app sizes
```

**Technology**: C# POCOs, JSON serialization

### Services/
Business logic:

```
Services/
├── IReportGenerator.cs
│   └── Interface for report creation
├── MobileAppReportGenerator.cs
│   └── PDF generation logic
├── PdfReportParser.cs
│   └── PDF import/parsing
├── ToastService.cs
│   └── Notification service
└── Exceptions.cs
    └── Custom exception types
```

**Technology**: QuestPDF, iText7, Dependency Injection

### Config/
Configuration:

```
Config/
├── appsettings.json
│   └── Production settings
├── appsettings.Development.json
│   └── Development settings
└── .editorconfig
    └── Code style rules
```

### wwwroot/
Static assets:

```
wwwroot/
├── lib/
│   └── Bootstrap, jQuery, etc.
├── index.html
├── app.css
└── download.js
```

## Data Flow

### Report Generation Flow

```
1. User fills form
           ↓
2. Click "Generate PDF"
           ↓
3. Form validation
           ↓
4. MobileAppReportGenerator.Generate()
           ↓
5. QuestPDF creates PDF in memory
           ↓
6. PDF returned to browser
           ↓
7. Browser downloads file
```

### PDF Import Flow

```
1. User clicks "Import PDF"
           ↓
2. Select PDF file
           ↓
3. PdfReportParser.ExtractData(pdf)
           ↓
4. Regex patterns find values
           ↓
5. Map values to form fields
           ↓
6. Form auto-fills
           ↓
7. User enters new data and generates
```

## Dependency Injection

Services registered in `Program.cs`:

```csharp
// All services registered here
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<IReportGenerator<MobileAppReportConfig>, 
                           MobileAppReportGenerator>();
builder.Services.AddScoped<PdfReportParser>();
```

Usage in components:

```razor
@inject IReportGenerator<MobileAppReportConfig> ReportGenerator
@inject ToastService Toast

@code {
    protected override async Task OnInitializedAsync() {
        var pdf = await ReportGenerator.GenerateAsync(config);
    }
}
```

## Request Pipeline

How a request flows through the application:

```
1. Browser requests page
           ↓
2. Program.cs middleware processes
           ↓
3. ASP.NET Core routes to component
           ↓
4. Component renders HTML
           ↓
5. Blazor establishes WebSocket connection
           ↓
6. User interactions sent to server
           ↓
7. Event handler executes
           ↓
8. Component updates
           ↓
9. HTML diff sent to browser
           ↓
10. Browser updates DOM
```

## Key Technologies

### Blazor Server

- Server-side rendering with real-time DOM updates
- WebSocket connection to browser
- C# instead of JavaScript
- Full .NET capabilities

### QuestPDF

- Modern PDF generation library
- Fluent API
- Professional layouts
- No external dependencies

### iText7

- PDF text extraction
- Import previous reports
- Regex pattern matching

### Bootstrap 5

- Responsive UI framework
- Pre-built components
- Theming support

## Configuration Management

All constants in `AppConstants.cs`:

```csharp
public static class AppConstants {
    // Server
    public const string LocalhostUrl = "http://localhost:5000";
    public const int LocalhostPort = 5000;
    
    // Files
    public const int MaxPdfFileSizeBytes = 10 * 1024 * 1024; // 10 MB
    
    // Logging
    public static class LogEvents {
        public const int AppStartup = 1000;
        public const int PdfGeneration = 2000;
    }
}
```

Single source of truth for configuration.

## Error Handling

Three custom exceptions:

```csharp
// PDF operation errors
throw new PdfException("Could not parse PDF");

// Configuration validation
throw new InvalidReportConfigException("Missing required field");

// Missing data
throw new MissingReportFieldException("Company name required");
```

Catch specific exceptions for precise handling.

## Logging

Structured logging throughout:

```csharp
logger.LogInformation(LogEvents.AppStartup, 
    "App started on {Url}", AppConstants.LocalhostUrl);
    
logger.LogError(LogEvents.PdfGeneration, ex, 
    "PDF generation failed for {Company}", config.AppName);
```

Log levels:
- **Debug** - Development only
- **Information** - General info
- **Warning** - Non-critical issues
- **Error** - Errors needing attention

## Extending the Architecture

### Adding a New Report Type

1. **Create Model** (`Models/`)
   ```csharp
   public class MyReportConfig { }
   ```

2. **Implement Service** (`Services/`)
   ```csharp
   public class MyReportGenerator : 
       IReportGenerator<MyReportConfig> { }
   ```

3. **Register in DI** (`Program.cs`)
   ```csharp
   builder.Services.AddScoped<IReportGenerator<MyReportConfig>, 
                              MyReportGenerator>();
   ```

4. **Create Component** (`Components/Pages/`)
   ```razor
   @page "/myreport"
   @inject IReportGenerator<MyReportConfig> Generator
   ```

## Best Practices

- Keep business logic in Services
- Keep UI in Components
- Keep data in Models
- Use dependency injection
- Log important events
- Validate input data
- Use specific exceptions

---

**Next**: [Code Patterns](code-patterns.md)

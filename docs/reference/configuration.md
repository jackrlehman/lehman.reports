# Configuration Reference

All configurable settings and constants.

## AppConstants.cs

Central configuration file in root directory.

### Server Settings

```csharp
public const string LocalhostUrl = "http://localhost:5000";
public const int LocalhostPort = 5000;
```

Change these to use a different port or protocol.

### File Limits

```csharp
public const int MaxPdfFileSizeBytes = 10 * 1024 * 1024; // 10 MB
```

Maximum file size for PDF imports.

### Application Metadata

```csharp
public const string AppName = "Report Builder";
public const string AppVersion = "1.04";
```

Update `AppVersion` before each release.

### Logging Events

```csharp
public static class LogEvents {
    public const int AppStartup = 1000;
    public const int PdfGeneration = 2000;
    public const int PdfParsing = 2001;
    public const int FormValidation = 3000;
}
```

Use these when logging important events.

## appsettings.json

Runtime configuration in `Config/` folder.

### Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

Log levels:
- `Debug` - Very detailed (dev only)
- `Information` - General info
- `Warning` - Non-critical issues
- `Error` - Errors
- `Critical` - App-breaking errors

### Server

```json
{
  "Urls": "http://localhost:5000",
  "AllowedHosts": "*"
}
```

- `Urls` - Where to listen
- `AllowedHosts` - Allowed host headers (* = any)

### QuestPDF

```json
{
  "QuestPDF": {
    "ProxyUsage": "Environment"
  }
}
```

QuestPDF license management.

## appsettings.Development.json

Development-specific overrides (in `Config/` folder).

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

More verbose logging in development.

## .editorconfig

Code style rules (in `Config/` folder).

### Indentation

```
[*.cs]
indent_style = space
indent_size = 4
```

Use 4-space indentation.

### Naming Conventions

```
# Public types: PascalCase
dotnet_naming_rule.public_types_must_be_pascal_case.symbols = public_symbols
dotnet_naming_symbols.public_symbols.applicable_kinds = class,struct,interface,enum
dotnet_naming_symbols.public_symbols.applicable_accessibilities = public
dotnet_naming_symbols.public_symbols.required_modifiers = 
dotnet_naming_style.pascal_case_style.capitalization = pascal_case

# Private fields: _camelCase
dotnet_naming_rule.private_fields_must_be_camel_case.symbols = private_symbols
dotnet_naming_symbols.private_symbols.applicable_kinds = field
dotnet_naming_symbols.private_symbols.applicable_accessibilities = private
dotnet_naming_style.camel_case_style.capitalization = camel_case
dotnet_naming_style.camel_case_style.required_prefix = _
```

## Port Configuration

### Default Port

```
http://localhost:5000
```

### Change Port

**Option 1: Environment Variable** (best for distribution)

```powershell
$env:ASPNETCORE_URLS = "http://localhost:5001"
dotnet run
```

**Option 2: Edit appsettings.json**

```json
{
  "Urls": "http://localhost:5001"
}
```

**Option 3: Edit AppConstants.cs**

```csharp
public const string LocalhostUrl = "http://localhost:5001";
public const int LocalhostPort = 5001;
```

### For Distribution Users

Edit `dist/app/appsettings.json`:

```json
{
  "Urls": "http://localhost:5001"
}
```

Then restart `ReportBuilder.bat`.

## Database Configuration

**Not applicable** - this app is stateless.

No database is used. All data is:
- Received from user input
- Processed in memory
- Returned as PDF
- Not stored anywhere

## Authentication

**Not implemented** - open application.

This app has no login or authentication. If you need it:

1. Install ASP.NET Core Authentication
2. Add identity provider (Azure AD, Google, etc.)
3. Protect routes with `[Authorize]`
4. Add login/logout UI

## CORS Configuration

**Not needed** - single-domain app.

Currently allows all origins. If you need CORS:

```csharp
// In Program.cs
builder.Services.AddCors(options => {
    options.AddPolicy("AllowSpecific", policy => {
        policy
            .WithOrigins("https://example.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

app.UseCors("AllowSpecific");
```

## Feature Flags

Not implemented but easy to add:

```csharp
public static class Features {
    public const bool EnableEmailExport = false;
    public const bool EnableDatabaseStorage = false;
    public const bool EnableUserAuth = false;
}
```

Use in code:

```csharp
if (Features.EnableEmailExport) {
    // Show email export button
}
```

## Environment Variables

### Development

Set in Visual Studio Properties or `launchSettings.json`:

```json
{
  "ASPNETCORE_ENVIRONMENT": "Development",
  "ASPNETCORE_URLS": "http://localhost:5000"
}
```

### Production

Set via system environment or deployment platform:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://0.0.0.0:80"
```

## Version Management

Update version in multiple places:

**1. AppConstants.cs**
```csharp
public const string AppVersion = "1.05";
```

**2. Models/MobileAppReportConfig.cs**
```csharp
public string Version { get; set; } = "1.05";
```

**3. Build Distribution**
```powershell
.\build-distribution.ps1 -Version "1.05"
```

**4. Documentation** (optional)
```markdown
# Version 1.05
```

## Logging Configuration

### Log Levels

| Level | Use Case |
|-------|----------|
| `Debug` | Detailed development info (dev only) |
| `Information` | General app flow and important events |
| `Warning` | Recoverable issues |
| `Error` | Errors that need attention |
| `Critical` | App-breaking errors |

### Log Categories

Organize logs by area:

```csharp
// Per-class logging
private readonly ILogger<MyClass> _logger;

// Log specific events
_logger.LogInformation(LogEvents.MyEvent, "Message: {Value}", value);
```

### View Logs

**Development**:
- Console output
- IDE debug window
- Browser console (F12)

**Production**:
- File logging (add sink)
- Cloud logging (Azure, DataDog, etc.)
- Event Viewer

## Secrets Management

**Development**: Use `appsettings.Development.json`

```json
{
  "ApiKey": "dev-key-12345"
}
```

**Production**: Use environment variables

```powershell
$env:ApiKey = "prod-key-xxxxx"
```

Never commit secrets to git!

---

**See Also**:
- [Quick Start](../guides/quick-start.md)
- [Development Setup](../development/setup.md)

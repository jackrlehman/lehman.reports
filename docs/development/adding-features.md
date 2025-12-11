# Adding Features

Step-by-step guide to adding new features.

## Adding a New Report Type

### Step 1: Create Data Model

Create `Models/MyReportConfig.cs`:

```csharp
using System.Text.Json.Serialization;

public class MyReportConfig {
    public string AppName { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime ReportDate { get; set; } = DateTime.Now;
    
    [JsonIgnore]
    public string Version { get; set; } = "1.04";
    
    public bool IsValid() {
        return !string.IsNullOrEmpty(AppName) &&
               !string.IsNullOrEmpty(CreatedBy);
    }
}
```

### Step 2: Implement Generator Service

Create `Services/MyReportGenerator.cs`:

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class MyReportGenerator : IReportGenerator<MyReportConfig> {
    private readonly ILogger<MyReportGenerator> _logger;
    
    public MyReportGenerator(ILogger<MyReportGenerator> logger) {
        _logger = logger;
    }
    
    public async Task<byte[]> GenerateAsync(MyReportConfig config) {
        if (!config.IsValid()) {
            throw new InvalidReportConfigException("Config is invalid");
        }
        
        try {
            _logger.LogInformation(
                LogEvents.PdfGeneration, 
                "Generating report for {AppName}", 
                config.AppName
            );
            
            var document = Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.A4);
                    page.Margin(2, PageSizes.Centimetre);
                    
                    page.Header().Text($"Report: {config.AppName}")
                        .FontSize(24).Bold();
                    
                    page.Content().Column(column => {
                        column.Item().Text($"Created by: {config.CreatedBy}");
                        column.Item().Text($"Date: {config.ReportDate:yyyy-MM-dd}");
                    });
                });
            });
            
            var pdf = document.GeneratePdf();
            return await Task.FromResult(pdf);
        }
        catch (Exception ex) {
            _logger.LogError(
                LogEvents.PdfGeneration, ex, 
                "Report generation failed for {AppName}", 
                config.AppName
            );
            throw new PdfException("Could not generate report", ex);
        }
    }
}
```

### Step 3: Register Service

Edit `Program.cs`:

```csharp
// Add after other service registrations
builder.Services.AddScoped<
    IReportGenerator<MyReportConfig>, 
    MyReportGenerator>();
```

### Step 4: Create UI Component

Create `Components/Pages/MyReport.razor`:

```razor
@page "/myreport"
@inject IReportGenerator<MyReportConfig> ReportGenerator
@inject ToastService Toast
@inject ILogger<MyReport> Logger

<h1>My Custom Report</h1>

<form @onsubmit="HandleGenerateReport">
    <div class="mb-3">
        <label class="form-label">App Name</label>
        <input class="form-control" @bind="Config.AppName" />
    </div>
    
    <div class="mb-3">
        <label class="form-label">Created By</label>
        <input class="form-control" @bind="Config.CreatedBy" />
    </div>
    
    <button type="submit" class="btn btn-primary" disabled="@_isGenerating">
        @if (_isGenerating) {
            <span class="spinner-border spinner-border-sm me-2"></span>
            Generating...
        } else {
            Generate PDF Report
        }
    </button>
</form>

@code {
    private MyReportConfig Config = new();
    private bool _isGenerating = false;
    
    private async Task HandleGenerateReport() {
        if (!Config.IsValid()) {
            Toast.ShowError("Please fill in all required fields");
            return;
        }
        
        try {
            _isGenerating = true;
            
            Logger.LogInformation("Generating report");
            var pdf = await ReportGenerator.GenerateAsync(Config);
            
            // Download file
            await DownloadFile(pdf, "MyReport.pdf");
            
            Toast.ShowSuccess("Report generated successfully!");
            
            Logger.LogInformation("Report generated");
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Report generation failed");
            Toast.ShowError($"Error: {ex.Message}");
        }
        finally {
            _isGenerating = false;
        }
    }
    
    private async Task DownloadFile(byte[] fileBytes, string filename) {
        // See wwwroot/download.js for implementation
        await JS.InvokeVoidAsync("downloadFile", fileBytes, filename);
    }
}
```

### Step 5: Add Navigation Link

Edit `Components/Layout/NavMenu.razor`:

```razor
<nav class="navbar">
    <a class="nav-link" href="myreport">
        My Custom Report
    </a>
</nav>
```

## Adding a Service

### Example: Email Service

Create `Services/EmailService.cs`:

```csharp
public interface IEmailService {
    Task SendAsync(string to, string subject, string body);
}

public class EmailService : IEmailService {
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(
        IConfiguration config,
        ILogger<EmailService> logger
    ) {
        _config = config;
        _logger = logger;
    }
    
    public async Task SendAsync(string to, string subject, string body) {
        try {
            _logger.LogInformation(
                "Sending email to {To}", to
            );
            
            // Implementation here
            
            await Task.CompletedTask;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Email send failed");
            throw;
        }
    }
}
```

Register in `Program.cs`:

```csharp
builder.Services.AddScoped<IEmailService, EmailService>();
```

Use in components:

```razor
@inject IEmailService EmailService

@code {
    private async Task SendEmail() {
        await EmailService.SendAsync(
            "user@example.com", 
            "Subject", 
            "Body"
        );
    }
}
```

## Adding Configuration

Add to `AppConstants.cs`:

```csharp
public static class AppConstants {
    // Existing constants...
    
    public static class Email {
        public const string SmtpServer = "smtp.gmail.com";
        public const int SmtpPort = 587;
    }
}
```

Use in code:

```csharp
var server = AppConstants.Email.SmtpServer;
```

## Adding Logging Events

Edit `AppConstants.cs`:

```csharp
public static class LogEvents {
    public const int AppStartup = 1000;
    public const int PdfGeneration = 2000;
    public const int PdfParsing = 2001;
    public const int FormValidation = 3000;
    // Add your event
    public const int EmailSent = 4000;
}
```

Use in code:

```csharp
_logger.LogInformation(
    LogEvents.EmailSent, 
    "Email sent to {Email}", 
    address
);
```

## Testing Your Feature

1. **Build project**
   ```powershell
   dotnet build
   ```

2. **Run application**
   ```powershell
   dotnet watch run
   ```

3. **Navigate to your feature**
   - Open browser
   - Find link in navigation menu
   - Test all functionality

4. **Check logs**
   - Open browser console (F12)
   - Watch PowerShell output
   - Verify logging messages appear

5. **Test error cases**
   - Try invalid input
   - Test error messages
   - Verify error is logged

## Deployment

After adding a feature:

1. **Test thoroughly** in development
2. **Build release**
   ```powershell
   .\build-distribution.ps1 -Version "1.05"
   ```
3. **Test in distribution**
   - Extract ZIP
   - Run ReportBuilder.bat
   - Verify feature works
4. **Distribute to users**

---

**References**:
- [Code Patterns](code-patterns.md)
- [Architecture](architecture.md)
- [Setup Guide](setup.md)

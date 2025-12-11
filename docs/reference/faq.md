# FAQ - Frequently Asked Questions

Common questions and answers.

## Installation & Setup

### Q: Do I need .NET installed to run the app?

**A:** Only for development. Users receiving the distribution ZIP don't need .NET - everything is included.

### Q: Can I run this on macOS or Linux?

**A:** Yes! The code is cross-platform. Just install .NET 9.0 SDK and run `dotnet watch run`.

### Q: How much disk space do I need?

**A:**
- Development: ~500 MB
- Distribution: ~64 MB (ZIP)
- Extracted: ~200 MB

### Q: Can I install multiple versions?

**A:** Yes, extract to different folders or use different ports in `appsettings.json`.

## Usage

### Q: How do I import a previous month's report?

**A:**
1. Click "Import Last Month's PDF"
2. Select your previous month's PDF file
3. Form auto-fills with old data
4. Enter new current period values
5. Generate PDF

### Q: Can I export my form data?

**A:** Yes, click "Export Config (JSON)" to download your form values as JSON file.

### Q: Can I import a JSON config?

**A:** Currently, JSON export is for backup only. Manual import coming in future version.

### Q: What report formats are supported?

**A:** Currently PDFs generated from this app. Importing works best with PDFs from this app.

### Q: Can I customize the report layout?

**A:** Yes, edit `Services/MobileAppReportGenerator.cs` and modify the QuestPDF layout code.

## Technical

### Q: Where is my data stored?

**A:** Nowhere! All processing happens in memory. No database, no cloud storage, no logs of your data.

### Q: Is the application secure?

**A:** 
- No authentication required
- No data transmission to external servers
- All processing happens locally
- PDFs are generated in memory

If you need security (auth, encryption), add it yourself.

### Q: What if the PDF import fails?

**A:**
- Manual entry (just fill the form)
- Use JSON export from previous month as reference
- Contact support if pattern matching fails

### Q: How big can PDF files be?

**A:** Maximum 10 MB (configurable in `AppConstants.cs`).

### Q: Can I use this on a web server?

**A:** Yes, but requires modification:
1. Change `localhost:5000` to your server URL
2. Add authentication if exposed publicly
3. Add HTTPS configuration
4. Deploy to Windows Server or Azure

### Q: Can I automate report generation?

**A:** Yes, create a service that:
1. Calls `IReportGenerator<MobileAppReportConfig>`
2. Passes configuration
3. Saves PDF to file

## Development

### Q: How do I add a new report type?

**A:** See [Adding Features](../development/adding-features.md) - step-by-step guide.

### Q: How do I add a database?

**A:**
1. Install Entity Framework: `dotnet add package Microsoft.EntityFrameworkCore`
2. Create DbContext class
3. Configure connection string in `appsettings.json`
4. Register in DI: `builder.Services.AddDbContext<MyDbContext>()`
5. Inject in services

### Q: How do I add authentication?

**A:**
1. Install: `dotnet add package Microsoft.AspNetCore.Identity`
2. Create User entity
3. Configure in `Program.cs`
4. Add login/logout pages
5. Protect routes with `[Authorize]`

### Q: How do I add email sending?

**A:**
1. Create `IEmailService` interface
2. Implement with SMTP or SendGrid
3. Register in DI
4. Inject in components
5. Call `await emailService.SendAsync(...)`

See [Code Patterns](../development/code-patterns.md) for examples.

### Q: Can I use JavaScript frameworks?

**A:** Blazor already handles interactivity. For extra JS:
1. Add files to `wwwroot/`
2. Reference in `_Host.cshtml`
3. Call from Blazor with `JS.InvokeAsync()`

### Q: How do I debug?

**A:**
- **Visual Studio**: Press F5
- **VS Code**: Open debug tab, select .NET
- **Rider**: Press Ctrl+D

Set breakpoints by clicking in margin.

### Q: How do I improve performance?

**A:**
1. Build in Release mode: `dotnet build -c Release`
2. Enable tiered compilation
3. Profile with Visual Studio Profiler
4. Check logs for slow operations

### Q: How do I handle errors better?

**A:** Add custom exceptions and logging:
```csharp
throw new MyException("Detailed message");
_logger.LogError(LogEvents.MyEvent, ex, "Error: {Error}", ex.Message);
```

## Distribution

### Q: How do I build a distribution?

**A:**
```powershell
.\build-distribution.ps1 -Version "1.05"
```

Creates `ReportBuilder-v1.05.zip` ready to share.

### Q: How do I update the app for users?

**A:**
1. Make code changes
2. Update version number
3. Build distribution: `.\build-distribution.ps1 -Version "1.05"`
4. Distribute new ZIP to users
5. Users extract and run

### Q: Can I customize the launcher?

**A:** Yes, edit `dist/ReportBuilder.bat` and `dist/Install.bat`.

### Q: How do I host the documentation?

**A:** 
1. Build MkDocs: `mkdocs build` (creates `site/` folder)
2. Upload `site/` to web server
3. Users open in browser
4. Or generate PDF from MkDocs

### Q: How big is the download?

**A:** ~64 MB ZIP file (includes .NET runtime).

### Q: Does the user need admin rights?

**A:** No, unless they want to:
- Create Start Menu shortcuts (optional)
- Install to Program Files

## Troubleshooting

### Q: "Port 5000 already in use"

**A:**
```powershell
taskkill /F /IM dotnet.exe
dotnet watch run
```

Or change port in `appsettings.json`.

### Q: "Connection refused" in browser

**A:**
1. Wait 10-15 seconds (first start takes time)
2. Refresh browser
3. Check PowerShell for error messages

### Q: Build fails with errors

**A:**
```powershell
dotnet clean
dotnet restore
dotnet build
```

### Q: Hot reload not working

**A:** Restart `dotnet watch run`.

### Q: PDF generation very slow

**A:** 
1. First generation is slower (compiling)
2. Subsequent ones are fast
3. Release build is faster: `dotnet build -c Release`

### Q: Form won't validate

**A:** Check browser console (F12) for JavaScript errors.

### Q: Can't import PDF

**A:**
1. Verify PDF is from this app
2. Check PDF size < 10 MB
3. Try simpler PDF first (fewer pages)
4. Check logs for error message

## Getting Help

1. **Check Documentation**: [Main docs](../index.md)
2. **Review Code**: Code examples in source
3. **Check Logs**: PowerShell output and browser console (F12)
4. **Search FAQ**: Ctrl+F on this page
5. **Contact Developer**: Create issue on GitHub

## Reporting Bugs

Include:
- What you were doing
- What went wrong
- Error message (if any)
- Operating system
- App version

## Feature Requests

Create issue with:
- What feature you want
- Why you need it
- How it should work
- Expected benefits

---

**Still stuck?** Check [Quick Start](../guides/quick-start.md) or [Setup Guide](../development/setup.md).

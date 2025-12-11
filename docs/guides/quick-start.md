# Getting Started

Get up and running in minutes.

## For Developers

### Prerequisites
- .NET 9.0 SDK - [Download](https://dotnet.microsoft.com/download)
- A code editor (VS Code, Visual Studio, etc.)

### Setup

1. **Verify .NET Installation**
   ```powershell
   dotnet --version  # Should show 9.0.x or higher
   ```

2. **Clone Repository**
   ```powershell
   git clone <repo-url>
   cd lehman.reports
   ```

3. **Restore & Run**
   ```powershell
   dotnet watch run
   ```
   App launches at `http://localhost:5000` with auto-reload.

### Troubleshooting

**Port 5000 in use?**
```powershell
taskkill /F /IM dotnet.exe
dotnet watch run
```

**Connection refused?**
- Wait 10-15 seconds for startup
- Refresh browser
- Check PowerShell for errors

**Large first build?**
- Normal - .NET is downloading dependencies
- Takes 2-3 minutes first time only

---

## For End Users

### Prerequisites
- Windows 10+ (or macOS/Linux)
- ~500 MB free disk space
- Any modern web browser

### Installation

1. **Download** `ReportBuilder-v1.05.zip`
2. **Extract** - Right-click ZIP → Extract All
3. **Run** - Double-click `ReportBuilder.bat` in the extracted folder
4. **Wait** - 10 seconds for app to start
5. **Browser opens** automatically to the application

**That's it!** No installation required.

### System Requirements

**Minimum:**
- Windows 10 / macOS 10.14+ / Ubuntu 18.04+
- 4 GB RAM
- Modern browser (Chrome, Edge, Firefox, Safari)

**Recommended:**
- Windows 11 / macOS 12+ / Ubuntu 22.04+
- 8 GB RAM
- Latest browser version

### Port Already in Use?

If you get "address already in use" error:

1. Open `appsettings.json` (in the extracted folder)
2. Find: `"localhost:5000"`
3. Change to: `"localhost:5001"`
4. Save and re-run `ReportBuilder.bat`

### Uninstall

Just delete the extracted folder. No registry entries or leftover files.

---

## Next Steps

- [Creating Your First Report](first-report.md)
- [Building a Distribution](../development/adding-features.md)
- [View Full Documentation](../index.md)

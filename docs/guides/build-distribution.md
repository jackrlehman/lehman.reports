# Build Distribution Package

Create a self-contained ZIP file for distributing to end users.

## Quick Build

One command:

```powershell
cd C:\git\lehman.reports
.\build-distribution.ps1 -Version "1.04"
```

**Time**: 2-3 minutes  
**Output**: `ReportBuilder-v1.04.zip` (64 MB)

## What Gets Built

The ZIP contains:

```
ReportBuilder-v1.04.zip
├── app/                          # Executable application
│   ├── ReportBuilder.Web.exe     # Main application
│   ├── appsettings.json          # Configuration
│   ├── appsettings.Development.json
│   └── [all .NET runtime files]
├── ReportBuilder.bat             # Windows launcher
├── Install.bat                   # Create shortcuts
├── README.md                     # User instructions
└── INSTALLATION.txt              # Setup guide
```

## Build Process

The script automatically:

1. ✓ Checks .NET is installed
2. ✓ Cleans previous builds
3. ✓ Restores NuGet packages
4. ✓ Builds in Release mode
5. ✓ Publishes self-contained app
6. ✓ Copies to distribution folder
7. ✓ Creates ZIP archive

## Updating Version

Before each build, update the version:

```powershell
# 1. Update in code
# Models/MobileAppReportConfig.cs
public string Version { get; set; } = "1.05";

# 2. Build with new version
.\build-distribution.ps1 -Version "1.05"
```

Creates: `ReportBuilder-v1.05.zip`

## Verification

After building, verify the package:

```powershell
# 1. Extract ZIP
Expand-Archive ReportBuilder-v1.04.zip -DestinationPath test-extract\

# 2. Run the app
.\test-extract\ReportBuilder\app\ReportBuilder.Web.exe

# 3. Wait 10 seconds and navigate to http://localhost:5000

# 4. Test all features
#    - Create a report
#    - Import a PDF
#    - Export JSON config

# 5. Delete test folder
Remove-Item test-extract -Recurse
```

## Distribution Methods

### Email

Attach or link the ZIP:

```
Subject: Report Builder v1.04 - Download

Hi Team,

New version of Report Builder is available.

Download: ReportBuilder-v1.04.zip (64 MB)

Instructions:
1. Download and extract ZIP
2. Run ReportBuilder.bat
3. App opens in browser

No installation needed!
```

### Cloud Storage

Upload to shared location:

- **OneDrive**: Share link with download permissions
- **Google Drive**: Upload and share
- **Dropbox**: Upload and get shareable link

### GitHub Release

```powershell
# 1. Navigate to your repo
# 2. GitHub → Releases → Create new release

# 3. Upload ReportBuilder-v1.04.zip
# 4. Add release notes
# 5. Publish

# Users download from: github.com/your-repo/releases
```

### Website Hosting

Upload ZIP to your web server:

```
https://your-domain.com/downloads/ReportBuilder-v1.04.zip
```

Share the download link.

## User Instructions

Provide users with:

```
REPORT BUILDER v1.04 - USER INSTRUCTIONS

1. DOWNLOAD
   Download ReportBuilder-v1.04.zip

2. EXTRACT
   Right-click ZIP → Extract All
   Choose a folder

3. RUN
   Double-click ReportBuilder.bat
   Wait 10 seconds for browser to open

4. USE
   Fill out report form
   Click "Generate PDF Report"
   Download your PDF

SYSTEM REQUIREMENTS:
- Windows 10+ (or macOS/Linux)
- No installation required
- All dependencies included

TROUBLESHOOTING:
Port 5000 in use?
  → Edit: app\appsettings.json
  → Change port number
  → Restart app

Browser won't connect?
  → Wait 15 seconds and refresh
  → Check Windows Firewall
```

## Troubleshooting Build

### Build Fails

```powershell
# 1. Clean everything
dotnet clean
Remove-Item -Recurse -Force bin, obj, publish

# 2. Restore packages
dotnet restore

# 3. Try build again
.\build-distribution.ps1 -Version "1.04"
```

### Executable Too Large

Normal for self-contained .NET:
- Runtime: 50 MB
- Application: 14 MB
- Total: ~64 MB

### Missing Files in ZIP

Verify the build:

```powershell
# Check dist\app folder
Get-ChildItem dist\app -Recurse | Measure-Object

# Should have 100+ files
```

## Version Numbering

Suggested versioning:

```
1.00 - Initial release
1.01 - Bug fixes
1.02 - Bug fixes
1.03 - Bug fixes
1.04 - Current version

2.00 - Major feature release
2.01 - Fixes for v2.0
```

## Next Steps

- [Deploy to users](deployment.md)
- [Update application](../development/adding-features.md)
- [Monitor usage](../reference/faq.md)

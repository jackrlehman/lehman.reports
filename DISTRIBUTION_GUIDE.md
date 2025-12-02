# Report Builder - Distributable Package

## Summary

Your Report Builder application is now ready for distribution! There are two ways to distribute it:

### Option 1: ZIP File (Recommended)
**File**: `ReportBuilder.zip` (64 MB)

This is a self-contained, portable package that includes everything needed to run the application.

**To distribute:**
1. Send `ReportBuilder.zip` to users
2. Users extract it to any folder (e.g., `C:\ReportBuilder`)
3. Users double-click `ReportBuilder.bat` to launch
4. Application starts and opens in their browser at `http://localhost:5000`

**Advantages:**
- No installation required (portable)
- Works on any Windows 7+ 64-bit system
- Can be run from USB drive
- All dependencies included

### Option 2: Installer Package (Advanced)
You can create an MSI installer for professional distribution:
- Users double-click the installer
- Application installs to Program Files
- Start Menu shortcuts are created
- Uninstall through Windows Add/Remove Programs

(This would require Windows Installer XML (WiX) tools)

---

## What's in the Distribution Package

```
ReportBuilder.zip
├── ReportBuilder.bat          ← Main launcher (double-click to run)
├── Install.bat                ← Optional: Creates Start Menu shortcuts
├── ReportBuilder.ps1          ← Alternative launcher (PowerShell)
├── README.md                  ← User documentation
├── INSTALLATION.txt           ← Installation instructions
├── INSTALLATION.txt           ← This file
└── app/                        ← Application files (all included)
    ├── ReportBuilder.Web.exe
    ├── *.dll (all dependencies)
    ├── wwwroot/ (web assets)
    └── ... (143 files total, 142 MB)
```

---

## How Users Run It

### Quick Start
1. Extract `ReportBuilder.zip`
2. Double-click `ReportBuilder.bat`
3. Wait 5-10 seconds
4. Browser opens to the application

### With Shortcuts
1. Extract `ReportBuilder.zip`
2. Double-click `Install.bat`
3. Desktop/Start Menu shortcuts appear
4. Click shortcuts to launch anytime

---

## Technical Details

**Platform**: Windows 7, 8, 10, 11 (64-bit only)

**Requirements**: None! 
- No .NET Framework needed (included in published app)
- No external dependencies
- Offline capable (no internet required)

**Launch Method**: Batch script starts the ASP.NET Core Kestrel web server on localhost:5000

**Port**: 5000 (configurable in `appsettings.json`)

**Browser**: Opens default browser automatically to `http://localhost:5000`

---

## Files Ready for Distribution

1. **`ReportBuilder.zip`** - The main distributable (64 MB)
   - Located at: `C:\git\lehman.reports\ReportBuilder.zip`
   - All files ready to send to users

2. **Distribution folder** - Individual files at:
   - `C:\git\lehman.reports\dist\`

---

## Distribution Methods

### Method A: Direct ZIP Download
Send `ReportBuilder.zip` via email, cloud storage, or download link.

### Method B: GitHub Releases
1. Go to your GitHub repository
2. Create a new Release
3. Upload `ReportBuilder.zip` as an asset
4. Users download from the release page

### Method C: Website Download
Host `ReportBuilder.zip` on your website for download.

### Method D: USB Drive / Physical Media
Copy `ReportBuilder.zip` to USB and ship to users.

---

## Important Notes

### File Paths
- The launcher assumes the app files are in a subdirectory called `app`
- Do NOT rename or move files without updating the batch scripts

### Port 5000
- The application runs on `localhost:5000`
- Users can change the port in `app/appsettings.json` if needed
- Make sure port 5000 is not in use by other applications

### First Run
- First startup may take 10-15 seconds (loading .NET runtime)
- Subsequent runs are faster

### Stopping the Application
- Close the command prompt window that appears
- Or press Ctrl+C in the console

---

## Customization for Users

Users can customize the application by editing `app/appsettings.json`:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5000"
      }
    }
  }
}
```

To use a different port, change `5000` to any port number (1024-65535).

---

## Version Information

- **Application**: Report Builder v1.0.0
- **Framework**: .NET 9.0
- **Runtime**: Self-contained (no .NET installation needed)
- **Build Date**: 2025-12-02
- **Size**: 63.95 MB (compressed ZIP)

---

## Next Steps

1. ✅ Test the launcher locally:
   ```powershell
   cd C:\git\lehman.reports\dist
   .\ReportBuilder.bat
   ```

2. ✅ Verify it opens in browser at `http://localhost:5000`

3. ✅ Test the Install.bat for shortcuts:
   ```cmd
   Install.bat
   ```

4. ✅ Upload `ReportBuilder.zip` to your distribution method

5. ✅ Share with users!

---

## Troubleshooting for Users

### Issue: "Connection refused" in browser
**Solution**: Wait 10-15 seconds and refresh the page. The server may still be starting.

### Issue: Port 5000 already in use
**Solution**: Edit `app/appsettings.json` and change the port number.

### Issue: Windows Defender warning
**Solution**: Normal for unsigned executables. Users can click "More info" → "Run anyway".

### Issue: Browser doesn't open automatically  
**Solution**: Manually navigate to `http://localhost:5000` in any browser.

---

## Support & Updates

For issues or feature requests:
- GitHub: https://github.com/jackrlehman/lehman.reports
- Email: (Your contact info)

---

**Everything is ready for distribution! Enjoy!** 🎉

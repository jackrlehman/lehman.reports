# Quick Build Script - Self-Contained Distribution

# USAGE: .\build-distribution.ps1 [VERSION]
# EXAMPLE: .\build-distribution.ps1 "1.04"

param(
    [string]$Version = "1.04"
)

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host $Text -ForegroundColor Cyan
    Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Status {
    param([string]$Text)
    Write-Host "→ $Text" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Text)
    Write-Host "✓ $Text" -ForegroundColor Green
}

function Write-Error {
    param([string]$Text)
    Write-Host "✗ $Text" -ForegroundColor Red
}

# Start
Write-Header "Report Builder Distribution Build v$Version"

# Verify prerequisites
Write-Status "Checking prerequisites..."
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET SDK not found. Install from https://dotnet.microsoft.com/download"
    exit 1
}
Write-Success "dotnet SDK found"

# Clean previous builds
Write-Status "Cleaning previous builds..."
Remove-Item -Recurse -Force bin -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force obj -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force publish -ErrorAction SilentlyContinue
Write-Success "Clean complete"

# Restore packages
Write-Status "Restoring NuGet packages..."
dotnet restore ReportBuilder.Web.csproj -q
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed!"
    exit 1
}
Write-Success "Packages restored"

# Build Release
Write-Status "Building Release configuration..."
dotnet build ReportBuilder.Web.csproj -c Release -q
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}
Write-Success "Build successful"

# Publish Self-Contained
Write-Status "Publishing self-contained Windows application..."
Write-Host "  (This may take 2-3 minutes...)" -ForegroundColor Gray

dotnet publish ReportBuilder.Web.csproj `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:SelfContained=true `
    -p:IncludeAllContentForSelfExtract=true `
    -p:DebugType=none `
    -p:DebugSymbols=false `
    -o publish `
    -q

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed!"
    exit 1
}
Write-Success "Publish complete"

# Verify publish output
$exeFile = "publish\ReportBuilder.Web.exe"
if (-not (Test-Path $exeFile)) {
    Write-Error "Executable not found at $exeFile"
    exit 1
}
Write-Success "Executable verified"

# Build documentation with MkDocs
Write-Status "Building documentation with MkDocs..."
try {
    python -m mkdocs build -q
    if ($LASTEXITCODE -ne 0) {
        Write-Error "MkDocs build failed!"
        exit 1
    }
    Write-Success "Documentation built"
} catch {
    Write-Error "Python/MkDocs not found - skipping documentation build"
    Write-Host "  (Optional: Install with: pip install mkdocs mkdocs-material)" -ForegroundColor Gray
}

# Copy to distribution folder
Write-Status "Copying to distribution folder..."
$distAppPath = "dist\app"
if (Test-Path $distAppPath) {
    Remove-Item -Recurse -Force $distAppPath
}
New-Item -ItemType Directory -Path "dist" -Force | Out-Null
Copy-Item -Recurse -Force "publish\*" "$distAppPath\"

# Copy app icon
if (Test-Path "app-icon.ico") {
    Copy-Item "app-icon.ico" "$distAppPath\"
    Write-Success "App icon included"
}

# Copy documentation if it was built
if (Test-Path "site") {
    Write-Status "Including local documentation..."
    Copy-Item -Recurse -Force "site" "dist\documentation"
    Write-Success "Documentation included in distribution"
}

# Create desktop shortcut with icon
Write-Status "Creating desktop shortcut..."
$shortcutPath = "$distAppPath\Report Builder.lnk"
$targetPath = "$distAppPath\ReportBuilder.Web.exe"
$iconPath = "$distAppPath\app-icon.ico"

$WshShell = New-Object -ComObject WScript.Shell
$shortcut = $WshShell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $targetPath
$shortcut.WorkingDirectory = $distAppPath
$shortcut.IconLocation = $iconPath
$shortcut.Description = "Report Builder - App Analytics Dashboard"
$shortcut.Save()
Write-Success "Desktop shortcut created with app icon"

Write-Success "Distribution folder updated"

# Create ZIP archive
Write-Status "Creating ZIP archive..."
$zipName = "ReportBuilder-v$Version.zip"
if (Test-Path $zipName) {
    Remove-Item $zipName
}
Compress-Archive -Path "dist\*" -DestinationPath $zipName -Force -CompressionLevel Optimal
Write-Success "ZIP archive created"

# Get file info
$zipSize = (Get-Item $zipName).Length / 1MB
$appExeSize = (Get-Item "$distAppPath\ReportBuilder.Web.exe").Length / 1MB
$totalFiles = (Get-ChildItem -Recurse $distAppPath | Measure-Object).Count

# Final summary
Write-Header "Build Complete ✓"

Write-Host "Distribution Package Information:" -ForegroundColor Cyan
Write-Host "  Version: $Version" -ForegroundColor White
Write-Host "  Archive: $zipName" -ForegroundColor White
Write-Host "  Size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor White
Write-Host "  App Path: $distAppPath" -ForegroundColor White
Write-Host "  Total Files: $totalFiles" -ForegroundColor White
Write-Host ""

Write-Host "Distribution Ready:" -ForegroundColor Green
Write-Host "  ✓ Executable: $distAppPath\ReportBuilder.Web.exe" -ForegroundColor Green
Write-Host "  ✓ Documentation: dist\documentation (local offline docs)" -ForegroundColor Green
Write-Host "  ✓ Archive: $zipName" -ForegroundColor Green
Write-Host ""

Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Test the build:"
Write-Host "     cd dist\app"
Write-Host "     .\ReportBuilder.Web.exe" -ForegroundColor Yellow
Write-Host ""
Write-Host "  2. Distribute the ZIP:"
Write-Host "     - Email: Send $zipName to users"
Write-Host "     - Cloud: Upload to OneDrive/Google Drive"
Write-Host "     - GitHub: Create release and upload"
Write-Host "     - Website: Host for download"
Write-Host ""
Write-Host "  3. Users extract and run:"
Write-Host "     - Extract $zipName"
Write-Host "     - Open dist\app folder"
Write-Host "     - Double-click ReportBuilder.Web.exe"
Write-Host "     - Application opens in browser"
Write-Host ""

Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host "Ready for distribution! 🚀" -ForegroundColor Green
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan
Write-Host ""

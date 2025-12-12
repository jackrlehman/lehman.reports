# Quick Build Script - Self-Contained Distribution

# USAGE: .\build-distribution.ps1 [VERSION]
# EXAMPLE: .\build-distribution.ps1 "1.04"

param(
    [string]$Version = "1.04"
)

function Write-Header {
    param([string]$Text)
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host $Text -ForegroundColor Cyan
    Write-Host "============================================================" -ForegroundColor Cyan
    Write-Host ""
}

function Write-Status {
    param([string]$Text)
    Write-Host "-> $Text" -ForegroundColor Yellow
}

function Write-Success {
    param([string]$Text)
    Write-Host "[OK] $Text" -ForegroundColor Green
}

function Write-Error {
    param([string]$Text)
    Write-Host "[ERROR] $Text" -ForegroundColor Red
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
dotnet clean ReportBuilder.Web.csproj -c Release -q
Remove-Item -Recurse -Force publish -ErrorAction SilentlyContinue
Write-Success "Clean complete"

# Restore packages
Write-Status "Restoring NuGet packages..."
dotnet restore ReportBuilder.Web.csproj --nologo --verbosity quiet 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed!"
    exit 1
}
Write-Success "Packages restored"

# Build Release
Write-Status "Building Release configuration..."
# Ensure staticwebassets directory exists (workaround for permissions issue)
New-Item -ItemType Directory -Force -Path "obj\Release\net9.0\staticwebassets" -ErrorAction SilentlyContinue | Out-Null
dotnet build ReportBuilder.Web.csproj -c Release --nologo --verbosity quiet 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed!"
    exit 1
}
Write-Success "Build successful"

# Publish Self-Contained
Write-Status "Publishing self-contained Windows application..."
Write-Host "  (This may take 2-3 minutes...)" -ForegroundColor Gray

# Ensure staticwebassets directory exists for publish
New-Item -ItemType Directory -Force -Path "obj\Release\net9.0\win-x64\staticwebassets" -ErrorAction SilentlyContinue | Out-Null

# Use a single-line publish call to avoid fragile backtick continuations
dotnet publish ReportBuilder.Web.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:SelfContained=true -p:IncludeAllContentForSelfExtract=true -p:DebugType=none -p:DebugSymbols=false -o publish --nologo --verbosity quiet 2>&1 | Out-Null

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
$pythonExists = Get-Command python -ErrorAction SilentlyContinue
if ($pythonExists) {
    python -m mkdocs build -q 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Documentation built"
    } else {
        Write-Host "[WARN] MkDocs not installed - skipping documentation" -ForegroundColor Yellow
        Write-Host "  (To install: pip install mkdocs mkdocs-material)" -ForegroundColor Gray
    }
} else {
    Write-Host "[WARN] Python not found - skipping documentation" -ForegroundColor Yellow
    Write-Host "  (Optional: Install Python from python.org)" -ForegroundColor Gray
}

# Copy to distribution folder (robust pure-PowerShell copy)
Write-Status "Copying to distribution folder..."
$distAppPath = "dist\app"

# Ensure a clean destination
if (Test-Path $distAppPath) {
    Remove-Item -Recurse -Force $distAppPath -ErrorAction SilentlyContinue
}
# Ensure parent 'dist' exists, then create dist\app
New-Item -ItemType Directory -Path "dist" -Force | Out-Null
New-Item -ItemType Directory -Path $distAppPath -Force | Out-Null

if (-not (Test-Path "publish")) {
    Write-Error "Publish folder not found; nothing to copy."
    exit 1
}

# Copy each top-level item from publish into dist\app, removing conflicts first
Get-ChildItem -LiteralPath "publish" -Force | ForEach-Object {
    $src = $_.FullName
    $dest = Join-Path $distAppPath $_.Name

    if (Test-Path $dest) {
        Remove-Item -Force -Recurse -ErrorAction SilentlyContinue $dest
    }

    if ($_.PSIsContainer) {
        Copy-Item -LiteralPath $src -Destination $dest -Recurse -Force
    } else {
        Copy-Item -LiteralPath $src -Destination $distAppPath -Force
    }
}

Write-Success "Files copied to distribution folder"

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

# Create desktop shortcut with icon (use absolute paths and fail gracefully)
Write-Status "Creating desktop shortcut..."
# Resolve absolute path for $distAppPath
try {
    $distAppFull = (Resolve-Path -LiteralPath $distAppPath).Path
} catch {
    # Fallback to combining current directory with the relative path
    $distAppFull = Join-Path (Get-Location).Path $distAppPath
}

# Build absolute shortcut, target and icon paths
$shortcutPath = Join-Path $distAppFull "Report Builder.lnk"
$targetPath = Join-Path $distAppFull "ReportBuilder.Web.exe"
$iconPath = Join-Path $distAppFull "app-icon.ico"

# Create the shortcut and save it, handling possible errors
$WshShell = New-Object -ComObject WScript.Shell
try {
    $shortcut = $WshShell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = $targetPath
    $shortcut.WorkingDirectory = $distAppFull
    $shortcut.IconLocation = $iconPath
    $shortcut.Description = "Report Builder - App Analytics Dashboard"
    $shortcut.Save()
    Write-Success "Desktop shortcut created with app icon"
} catch {
    Write-Error "Unable to create desktop shortcut at '$shortcutPath': $($_.Exception.Message)"
    # do not exit; continue with distribution (shortcut is optional)
}

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
Write-Header "Build Complete [OK]"

Write-Host "Distribution Package Information:" -ForegroundColor Cyan
Write-Host "  Version: $Version" -ForegroundColor White
Write-Host "  Archive: $zipName" -ForegroundColor White
Write-Host "  Size: $([math]::Round($zipSize, 2)) MB" -ForegroundColor White
Write-Host "  App Path: $distAppPath" -ForegroundColor White
Write-Host "  Total Files: $totalFiles" -ForegroundColor White
Write-Host ""

Write-Host "Distribution Ready:" -ForegroundColor Green
Write-Host "  [OK] Executable: $distAppPath\ReportBuilder.Web.exe" -ForegroundColor Green
Write-Host "  [OK] Documentation: dist\documentation (local offline docs)" -ForegroundColor Green
Write-Host "  [OK] Archive: $zipName" -ForegroundColor Green
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

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "Ready for distribution!" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
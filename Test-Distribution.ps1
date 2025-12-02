#!/usr/bin/env pwsh
# Report Builder Distribution Test Script
# This script tests the distribution package to ensure it's working correctly

param(
    [switch]$Quick = $false,
    [switch]$Full = $false
)

$testDir = "C:\git\lehman.reports\dist"
$appDir = Join-Path $testDir "app"

Write-Host "`n" -ForegroundColor Cyan
Write-Host "╔═══════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   Report Builder Distribution Test Script     ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Test 1: File Structure
Write-Host "Test 1: Checking file structure..." -ForegroundColor Yellow
$files = @(
    "ReportBuilder.bat",
    "Install.bat",
    "ReportBuilder.ps1",
    "README.md",
    "INSTALLATION.txt"
)

$allFound = $true
foreach ($file in $files) {
    $filePath = Join-Path $testDir $file
    if (Test-Path $filePath) {
        Write-Host "  ✓ $file" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $file NOT FOUND" -ForegroundColor Red
        $allFound = $false
    }
}

# Test 2: App Directory
Write-Host "`nTest 2: Checking app directory..." -ForegroundColor Yellow
if (Test-Path $appDir) {
    $fileCount = (Get-ChildItem $appDir -Recurse).Count
    $dirSize = ((Get-ChildItem $appDir -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB)
    Write-Host "  ✓ App directory exists" -ForegroundColor Green
    Write-Host "    - Files: $fileCount" -ForegroundColor Gray
    Write-Host "    - Size: $($dirSize.ToString('F2')) MB" -ForegroundColor Gray
} else {
    Write-Host "  ✗ App directory not found" -ForegroundColor Red
    $allFound = $false
}

# Test 3: Main Executable
Write-Host "`nTest 3: Checking main executable..." -ForegroundColor Yellow
$exePath = Join-Path $appDir "ReportBuilder.Web.exe"
if (Test-Path $exePath) {
    $exeSize = ((Get-Item $exePath).Length / 1MB)
    Write-Host "  ✓ ReportBuilder.Web.exe found" -ForegroundColor Green
    Write-Host "    - Size: $($exeSize.ToString('F2')) MB" -ForegroundColor Gray
} else {
    Write-Host "  ✗ ReportBuilder.Web.exe not found" -ForegroundColor Red
    $allFound = $false
}

# Test 4: ZIP File
Write-Host "`nTest 4: Checking ZIP distribution..." -ForegroundColor Yellow
$zipPath = "C:\git\lehman.reports\ReportBuilder.zip"
if (Test-Path $zipPath) {
    $zipSize = ((Get-Item $zipPath).Length / 1MB)
    Write-Host "  ✓ ReportBuilder.zip found" -ForegroundColor Green
    Write-Host "    - Size: $($zipSize.ToString('F2')) MB" -ForegroundColor Gray
    Write-Host "    - Location: $zipPath" -ForegroundColor Gray
} else {
    Write-Host "  ✗ ReportBuilder.zip not found" -ForegroundColor Red
    Write-Host "    (Run 'Create-Distribution' to generate the ZIP)" -ForegroundColor Yellow
}

# Test 5: Documentation
Write-Host "`nTest 5: Checking documentation..." -ForegroundColor Yellow
$docs = @(
    ("README.md", 10),
    ("INSTALLATION.txt", 100),
    ("Install.bat", 500)
)

foreach ($docInfo in $docs) {
    $doc = $docInfo[0]
    $minSize = $docInfo[1]
    $docPath = Join-Path $testDir $doc
    
    if (Test-Path $docPath) {
        $docSize = (Get-Item $docPath).Length
        if ($docSize -gt $minSize) {
            Write-Host "  ✓ $doc" -ForegroundColor Green
        } else {
            Write-Host "  ⚠ $doc (size: $docSize bytes, expected > $minSize)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ✗ $doc not found" -ForegroundColor Red
    }
}

# Summary
Write-Host "`n" -ForegroundColor Cyan
if ($allFound) {
    Write-Host "✓ All tests passed! Distribution package is ready." -ForegroundColor Green
    Write-Host "`nYou can now:" -ForegroundColor Cyan
    Write-Host "  1. Double-click ReportBuilder.bat to test" -ForegroundColor Gray
    Write-Host "  2. Share ReportBuilder.zip with users" -ForegroundColor Gray
    Write-Host "  3. Upload to GitHub Releases or your website" -ForegroundColor Gray
} else {
    Write-Host "✗ Some tests failed. Please review the errors above." -ForegroundColor Red
}

Write-Host "`nFor detailed distribution info, see: DISTRIBUTION_GUIDE.md" -ForegroundColor Cyan
Write-Host "`n"

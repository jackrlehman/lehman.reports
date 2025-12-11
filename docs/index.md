# Report Builder

A professional C# Blazor application for generating customizable PDF reports with an intuitive web interface.

## ✨ Features

- **Multiple Report Support** - Architecture designed for extensibility
- **PDF Generation** - Professional output using QuestPDF
- **Customizable Sections** - Toggle report sections on/off
- **Dynamic Company Branding** - Customize company name throughout
- **Interactive UI** - User-friendly Blazor interface
- **PDF Import** - Intelligently extract data from previous reports
- **Mobile App Reports** - Comprehensive iOS & Android performance tracking

## 🚀 Quick Start

=== "First Time Setup"
    ```bash
    # 1. Install .NET 9.0 SDK
    # Download from: https://dotnet.microsoft.com/download
    
    # 2. Clone or navigate to project
    cd C:\git\lehman.reports
    
    # 3. Run development server
    dotnet watch run
    ```
    
    App opens automatically at `http://localhost:5000`

=== "Distribution"
    ```powershell
    # Build production package
    .\build-distribution.ps1 -Version "1.04"
    
    # Creates: ReportBuilder-v1.04.zip (ready to distribute)
    ```

## 📋 What You Can Do

### For End Users
- ✓ Open web interface in any browser
- ✓ Create reports with custom data
- ✓ Import previous month's report (auto-fills old data)
- ✓ Export configuration as JSON
- ✓ Download PDF reports

### For Developers
- ✓ Add new report types
- ✓ Customize styling and branding
- ✓ Extend with new features
- ✓ Deploy to production

## 📚 Documentation Sections

- **[Getting Started](guides/quick-start.md)** - Set up and first report
- **[Development](development/setup.md)** - Architecture and code patterns
- **[Distribution](guides/build-distribution.md)** - Build and deploy
- **[Reference](reference/api-reference.md)** - Configuration and API

## 🏗️ Technology Stack

- **Framework**: ASP.NET Core 9.0 Blazor Server
- **Language**: C#
- **PDF**: QuestPDF library
- **UI**: Bootstrap 5 + Blazor components
- **Database**: None (stateless)

## 📦 Distribution

Users receive a self-contained ZIP file with:
- ✓ Complete application executable
- ✓ All dependencies included
- ✓ No installation required
- ✓ Windows batch launcher
- ✓ User documentation

**Size**: 64 MB ZIP | **Build Time**: 2-3 minutes

## ✅ Current Status

- **Version**: 1.04
- **Status**: Production Ready ✓
- **Last Updated**: December 2025

---

**New here?** Start with [Quick Start Guide](guides/quick-start.md) →

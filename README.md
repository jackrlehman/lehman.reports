# Report Builder

A professional C# Blazor application for generating customizable PDF reports. This application is designed to support multiple report types, with the initial implementation featuring a comprehensive Mobile App Store Performance Report.

## Features

- **Multiple Report Support**: Architecture designed to support multiple report types
- **PDF Generation**: Clean, professional PDF output using QuestPDF
- **Customizable Sections**: Toggle report sections on/off based on your needs
- **Dynamic Company Branding**: Replace company name throughout the report
- **Interactive UI**: User-friendly Blazor interface with form validation
- **Professional Formatting**: Clean tables, color-coded headers, and alternating row colors

## Current Reports

### Mobile App Store Performance Report

A comprehensive report for tracking mobile app performance across iOS and Android platforms.

**Sections Include:**
- Executive Summary
- iOS Platform Performance
  - Key Metrics Overview
  - Download Sources Analysis
- Android Platform Performance
- Platform Comparison
- Technical Specifications

**Customizable Fields:**
- Company name (replaces "Circa" in the sample)
- Report dates (current and previous period)
- Creator information (name and title)
- All metrics for iOS and Android platforms
- Download source breakdown
- Platform comparison metrics
- Technical specifications

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- A modern web browser

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd F:\Git\lehmidt\lehmidt.report.builder
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to the URL shown in the console (typically `https://localhost:5001` or `http://localhost:5000`)

### Using the Mobile App Report

#### Quick Start with PDF Import (Recommended)

1. Click on "Mobile App Store Performance Report" from the navigation menu
2. Click **"📂 Import Last Month's PDF"** to upload your previous month's report
3. The system will automatically:
   - Extract all values from the PDF
   - Map current period values → last period fields
   - Increment the report date to next month
   - Pre-fill all comparison values
4. Fill in the new current period values
5. Click "Generate PDF Report" to download your updated report

#### Manual Entry Method

1. Click on "Mobile App Store Performance Report" from the navigation menu
2. Fill in the General Information section with your company name and report dates
3. Toggle the sections you want to include in your report
4. Fill in the metrics for each enabled section:
   - iOS Platform Metrics (current, last period, and % change)
   - Download Sources (add/remove sources as needed)
   - Android Platform Metrics
   - Platform Comparison
   - Technical Specifications
5. Click "Generate PDF Report" to download your customized report

#### Export Configuration (Optional)

- Click **"💾 Export Config (JSON)"** to save your current form values as a JSON file
- Use this as a backup or to share configurations between team members

## Project Structure

```
ReportBuilder/
├── Models/
│   └── MobileAppReportConfig.cs    # Data models for report configuration
├── Services/
│   ├── IReportGenerator.cs         # Interface for report generators
│   ├── MobileAppReportGenerator.cs # Mobile app report PDF generator
│   └── PdfReportParser.cs          # PDF parser for importing reports
├── Components/
│   ├── Pages/
│   │   ├── Home.razor              # Home page
│   │   └── MobileAppReport.razor   # Mobile app report form
│   └── Layout/
│       ├── MainLayout.razor        # Main layout
│       └── NavMenu.razor           # Navigation menu
└── wwwroot/
    ├── download.js                 # PDF & JSON download JavaScript helpers
    └── ...                         # Bootstrap and other assets
```

## Adding New Reports

The application is architected to support multiple report types. To add a new report:

1. Create a new model in the `Models` folder
2. Create a new report generator implementing `IReportGenerator<TConfig>`
3. Create a new Blazor page in `Components/Pages`
4. Add navigation link in `NavMenu.razor`

## Technologies Used

- **Blazor Web App**: Interactive server-side web framework
- **QuestPDF**: Modern PDF generation library
- **iText7**: PDF parsing and text extraction library
- **Bootstrap 5**: UI styling and components
- **.NET 9.0**: Latest .NET platform

## PDF Formatting Features

- Professional color scheme (Blue headers)
- Alternating row colors for better readability
- Proper spacing and margins
- Clear section headers
- Page numbering
- Responsive table layouts

## PDF Import Feature

The application includes an intelligent PDF import feature that dramatically reduces data entry time:

### How It Works

1. **Upload Last Month's Report**: Click the import button and select your previous month's PDF
2. **Automatic Text Extraction**: The system uses iText7 to extract all text content from the PDF
3. **Smart Pattern Matching**: Regular expressions identify and extract each metric value
4. **Intelligent Mapping**: Current period values become last period values for the new report
5. **Date Increment**: Report date automatically advances to the next month
6. **Ready to Use**: Form is pre-filled and ready for you to enter new current period data

### Benefits

- **Time Savings**: Reduces data entry by ~50% each month
- **Accuracy**: Eliminates transcription errors when copying previous month's data
- **Consistency**: Ensures all comparison values match the previous report exactly
- **Convenience**: No need to manually reference the old report

### Supported Fields

The PDF parser extracts:
- Company name and report metadata
- All iOS platform metrics (8 metrics × 3 values each)
- Download source breakdown (5 sources × 5 values each)
- All Android platform metrics (3 metrics × 3 values each)
- Platform comparison data (4 categories × 6 values each)
- Technical specifications

### Fallback Options

If PDF import doesn't work perfectly:
- Manually edit any incorrectly parsed fields
- Use the JSON export/import as an alternative
- Fill in the form manually from scratch

## License

This project is provided as-is for your use.

## Notes

- The QuestPDF library requires a Community license for non-commercial use, which is set in the code
- All report data is processed client-side and not stored anywhere
- PDF generation and parsing happen on the server; no data is stored permanently
- PDF import works best with reports generated by this application
- Supported PDF size limit: 10 MB per file

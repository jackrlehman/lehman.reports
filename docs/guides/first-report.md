# First Report

Walk through creating your first Mobile App Store Performance report.

## Step 1: Open the Application

1. Run: `dotnet watch run` (development) or `ReportBuilder.bat` (distribution)
2. Wait for browser to open
3. Navigate to `http://localhost:5000` if it doesn't open automatically

## Step 2: Navigate to Reports

Click **"Mobile App Store Performance Report"** in the navigation menu.

You'll see a form with several sections:

- General Information
- Report Display Options
- iOS Platform Metrics
- Download Sources
- Android Platform Metrics
- Platform Comparison

## Step 3: Fill General Information

| Field | Example |
|-------|---------|
| Company Name | My App Company |
| Report Month | December 2025 |
| Previous Month | November 2025 |
| Creator Name | Jane Doe |
| Creator Title | Analytics Manager |

## Step 4: Choose Report Sections

Toggle which sections to include:

- ✓ iOS Platform Metrics (recommended)
- ✓ Download Sources (recommended)
- ✓ Android Platform Metrics
- ✓ Platform Comparison

Only enabled sections appear in the PDF.

## Step 5: Fill iOS Metrics

For each metric, enter:

| Column | Meaning |
|--------|---------|
| Current Period | This month's value |
| Last Period | Previous month's value |
| % Change | Percentage difference |

**Example:**
- Total Downloads (Current): 150,000
- Total Downloads (Last): 120,000
- % Change: +25%

Metrics include:
- Total Downloads
- Active Users (30-day)
- Crash-Free Users %
- App Size (MB)
- And more...

## Step 6: Add Download Sources

Click **"Add Download Source"** to add sources:

1. Enter source name (e.g., "App Store Organic")
2. Enter metrics for that source
3. Click **"Add"**
4. Repeat for other sources

**Example sources:**
- App Store Organic Search
- App Store Browse
- Web Referral
- Social Media
- Direct Install

## Step 7: Fill Android Metrics

Same format as iOS - enter current, previous, and % change.

## Step 8: Platform Comparison

Compare iOS vs Android across categories:

- User Metrics
- Engagement
- Technical Performance
- Market Share

## Step 9: Generate Report

1. Review all entered data
2. Click **"Generate PDF Report"** button
3. Browser downloads `MobileAppReport.pdf`
4. Open and verify the PDF

## Using PDF Import (Faster Next Time)

For next month's report:

1. Click **"📂 Import Last Month's PDF"**
2. Select the PDF you just created
3. The form auto-fills with last month's data
4. Just enter new "Current Period" values
5. Click Generate

This cuts data entry time by ~50%!

## Exporting Your Config

To save your form data:

1. Click **"💾 Export Config (JSON)"**
2. Browser downloads `report-config.json`
3. Keep for backup or team sharing

## Tips & Tricks

### Validating Data
- All fields are validated before generation
- Red error messages show what needs fixing
- Cannot generate with invalid data

### Batch Operations
- Export one report's JSON
- Use it as template for next month
- Just change the values and reuse

### Troubleshooting
- **PDF won't generate?** Check browser console (F12)
- **Missing data?** Check for red error messages
- **Wrong values?** Use browser's back button to check fields

## Next Steps

- [Build for distribution](build-distribution.md)
- [Learn the architecture](../development/architecture.md)
- [Advanced customization](../development/adding-features.md)

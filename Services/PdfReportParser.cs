using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using ReportBuilder.Models;
using System.Text.RegularExpressions;

namespace ReportBuilder.Services;

public class PdfReportParser
{
    public MobileAppReportConfig ParseMobileAppReport(byte[] pdfBytes)
    {
        var config = new MobileAppReportConfig();

        using var ms = new MemoryStream(pdfBytes);
        using var pdfReader = new PdfReader(ms);
        using var pdfDoc = new PdfDocument(pdfReader);

        // Extract text from all pages
        var fullText = string.Empty;
        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        {
            var page = pdfDoc.GetPage(i);
            var strategy = new SimpleTextExtractionStrategy();
            fullText += PdfTextExtractor.GetTextFromPage(page, strategy) + "\n";
        }

        // Parse the extracted text
        ParseGeneralInfo(fullText, config);
        ParseIOSMetrics(fullText, config);
        ParseAndroidMetrics(fullText, config);
        ParsePlatformComparison(fullText, config);
        ParseTechnicalSpecs(fullText, config);

        // Map current values to last values for the next report
        MapCurrentToLast(config);

        return config;
    }

    private void ParseGeneralInfo(string text, MobileAppReportConfig config)
    {
        // Extract company name from title - looks for "XXX App Performance Report"
        var companyMatch = Regex.Match(text, @"^(.+?)\s+App Performance Report", RegexOptions.Multiline);
        if (companyMatch.Success)
        {
            config.CompanyName = companyMatch.Groups[1].Value.Trim();
        }

        // Try new date format first: "MM/DD/YY, Created by" (Version 1.02+)
        var newDateMatch = Regex.Match(text, @"(\d{1,2})/(\d{1,2})/(\d{2}),\s+Created by\s+(.+?),\s+(.+?)(?:\n|$)");
        if (newDateMatch.Success)
        {
            config.ReportMonth = int.Parse(newDateMatch.Groups[1].Value);
            config.ReportDay = int.Parse(newDateMatch.Groups[2].Value);
            // Convert 2-digit year to 4-digit year
            var year = int.Parse(newDateMatch.Groups[3].Value);
            config.ReportYear = year < 50 ? 2000 + year : 1900 + year;
            config.CreatedByName = newDateMatch.Groups[4].Value.Trim();
            config.CreatedByTitle = newDateMatch.Groups[5].Value.Trim();
        }
        else
        {
            // Fall back to old format: "Month Day, Year, Created by" (Version 1.01)
            var oldDateMatch = Regex.Match(text, @"(\w+)\s+(\d+),\s+(\d+),\s+Created by\s+(.+?),\s+(.+?)(?:\n|$)");
            if (oldDateMatch.Success)
            {
                var monthName = oldDateMatch.Groups[1].Value;
                config.ReportMonth = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                config.ReportDay = int.Parse(oldDateMatch.Groups[2].Value);
                config.ReportYear = int.Parse(oldDateMatch.Groups[3].Value);
                config.CreatedByName = oldDateMatch.Groups[4].Value.Trim();
                config.CreatedByTitle = oldDateMatch.Groups[5].Value.Trim();
            }
        }

        // Extract last report date from table headers - try new format first
        var newLastDateMatches = Regex.Matches(text, @"As of (\d{1,2})/(\d{1,2})/(\d{2})");
        if (newLastDateMatches.Count > 1)
        {
            // Second occurrence is the "last report" date
            config.LastReportMonth = int.Parse(newLastDateMatches[1].Groups[1].Value);
            config.LastReportDay = int.Parse(newLastDateMatches[1].Groups[2].Value);
            var year = int.Parse(newLastDateMatches[1].Groups[3].Value);
            config.LastReportYear = year < 50 ? 2000 + year : 1900 + year;
        }
        else
        {
            // Fall back to old format
            var oldLastDateMatches = Regex.Matches(text, @"(?:As of\s+)?(\w+)\s+(\d+),?\s+(\d+)");
            if (oldLastDateMatches.Count > 1)
            {
                var monthName = oldLastDateMatches[1].Groups[1].Value;
                try
                {
                    config.LastReportMonth = DateTime.ParseExact(monthName, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month;
                    config.LastReportDay = int.Parse(oldLastDateMatches[1].Groups[2].Value);
                    config.LastReportYear = int.Parse(oldLastDateMatches[1].Groups[3].Value);
                }
                catch
                {
                    // Ignore parse errors for last date
                }
            }
        }
    }

    private void ParseIOSMetrics(string text, MobileAppReportConfig config)
    {
        // Parse Key Metrics table
        var metrics = new Dictionary<string, (double? current, double? last)>
        {
            ["Impressions"] = ExtractMetricRow(text, "Impressions"),
            ["Product Page Views"] = ExtractMetricRow(text, "Product Page Views"),
            ["Conversion Rate"] = ExtractMetricRow(text, "Conversion Rate"),
            ["Total Downloads"] = ExtractMetricRow(text, "Total Downloads"),
            ["Daily Downloads"] = ExtractMetricRow(text, "Daily Downloads"),
            ["Sessions per Device"] = ExtractMetricRow(text, "Sessions per Device"),
            ["Crash Rate per Session"] = ExtractMetricRow(text, "Crash Rate per Session"),
            ["Total Crashes"] = ExtractMetricRow(text, "Total Crashes")
        };

        config.IOSMetrics.Impressions = metrics["Impressions"].current;
        config.IOSMetrics.ImpressionsLast = metrics["Impressions"].last;

        config.IOSMetrics.ProductPageViews = metrics["Product Page Views"].current;
        config.IOSMetrics.ProductPageViewsLast = metrics["Product Page Views"].last;

        config.IOSMetrics.ConversionRate = metrics["Conversion Rate"].current;
        config.IOSMetrics.ConversionRateLast = metrics["Conversion Rate"].last;

        config.IOSMetrics.TotalDownloads = metrics["Total Downloads"].current;
        config.IOSMetrics.TotalDownloadsLast = metrics["Total Downloads"].last;

        config.IOSMetrics.DailyDownloads = metrics["Daily Downloads"].current;
        config.IOSMetrics.DailyDownloadsLast = metrics["Daily Downloads"].last;

        config.IOSMetrics.SessionsPerDevice = metrics["Sessions per Device"].current;
        config.IOSMetrics.SessionsPerDeviceLast = metrics["Sessions per Device"].last;

        config.IOSMetrics.CrashRatePerSession = metrics["Crash Rate per Session"].current;
        config.IOSMetrics.CrashRatePerSessionLast = metrics["Crash Rate per Session"].last;

        config.IOSMetrics.TotalCrashes = metrics["Total Crashes"].current;
        config.IOSMetrics.TotalCrashesLast = metrics["Total Crashes"].last;

        // Parse Download Sources
        ParseDownloadSources(text, config);
    }

    private void ParseDownloadSources(string text, MobileAppReportConfig config)
    {
        var sources = new[] { "App Store Search", "Web Referrer", "App Referrer", "App Store Browse", "Unavailable" };
        config.IOSMetrics.DownloadSources = new List<DownloadSource>();

        foreach (var sourceName in sources)
        {
            var source = new DownloadSource { Name = sourceName };

            // Try to find the source row - look for pattern like "Source Name {%} {downloads} {%} {downloads}" (5th value is % change which we ignore)
            var pattern = Regex.Escape(sourceName) + @"\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)";
            var match = Regex.Match(text, pattern);

            if (match.Success)
            {
                source.CurrentPercentage = ParseDouble(match.Groups[1].Value);
                source.CurrentDownloads = ParseDouble(match.Groups[2].Value);
                source.LastPercentage = ParseDouble(match.Groups[3].Value);
                source.LastDownloads = ParseDouble(match.Groups[4].Value);
            }

            config.IOSMetrics.DownloadSources.Add(source);
        }
    }

    private void ParseAndroidMetrics(string text, MobileAppReportConfig config)
    {
        // Look for Android Platform Performance section
        var androidMatch = Regex.Match(text, @"Android Platform Performance.*?Total Installs\s+([\d.,%-]+)\s+([\d.,%-]+)", RegexOptions.Singleline);
        if (androidMatch.Success)
        {
            config.AndroidMetrics.TotalInstalls = ParseDouble(androidMatch.Groups[1].Value);
            config.AndroidMetrics.TotalInstallsLast = ParseDouble(androidMatch.Groups[2].Value);
        }

        var dailyMatch = Regex.Match(text, @"(?:Android.*)?Daily Downloads\s+([\d.,%-]+)\s+([\d.,%-]+)", RegexOptions.Singleline);
        if (dailyMatch.Success)
        {
            config.AndroidMetrics.DailyDownloads = ParseDouble(dailyMatch.Groups[1].Value);
            config.AndroidMetrics.DailyDownloadsLast = ParseDouble(dailyMatch.Groups[2].Value);
        }

        var crashMatch = Regex.Match(text, @"(?:Android.*)?Crash Rate per\s+Session\s+([\d.,%-]+)\s+([\d.,%-]+)", RegexOptions.Singleline);
        if (crashMatch.Success)
        {
            config.AndroidMetrics.CrashRatePerSession = ParseDouble(crashMatch.Groups[1].Value);
            config.AndroidMetrics.CrashRatePerSessionLast = ParseDouble(crashMatch.Groups[2].Value);
        }
    }

    private void ParsePlatformComparison(string text, MobileAppReportConfig config)
    {
        // Platform Comparison section - extract values for each metric across both platforms
        var comparisonSection = Regex.Match(text, @"Platform Comparison(.*?)(?:Technical Specifications|$)", RegexOptions.Singleline);

        if (comparisonSection.Success)
        {
            var sectionText = comparisonSection.Groups[1].Value;

            // Total Downloads
            var totalDownloadsMatch = Regex.Match(sectionText, @"Total\s+Downloads\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)");
            if (totalDownloadsMatch.Success)
            {
                config.PlatformComparison.IOSTotalDownloads = ParseDouble(totalDownloadsMatch.Groups[1].Value);
                config.PlatformComparison.AndroidTotalDownloads = ParseDouble(totalDownloadsMatch.Groups[2].Value);
                config.PlatformComparison.IOSTotalDownloadsLast = ParseDouble(totalDownloadsMatch.Groups[3].Value);
                config.PlatformComparison.AndroidTotalDownloadsLast = ParseDouble(totalDownloadsMatch.Groups[4].Value);
            }

            // User % of all Apps (old label) or Download base (new label v1.02+)
            var userPercentMatch = Regex.Match(sectionText, @"(?:User % of all.*?Apps|Download base of.*?All)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)");
            if (userPercentMatch.Success)
            {
                config.PlatformComparison.IOSUserPercent = ParseDouble(userPercentMatch.Groups[1].Value);
                config.PlatformComparison.AndroidUserPercent = ParseDouble(userPercentMatch.Groups[2].Value);
                config.PlatformComparison.IOSUserPercentLast = ParseDouble(userPercentMatch.Groups[3].Value);
                config.PlatformComparison.AndroidUserPercentLast = ParseDouble(userPercentMatch.Groups[4].Value);
            }

            // Daily Downloads (in comparison section)
            var dailyMatch = Regex.Match(sectionText, @"Daily\s+Downloads\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)");
            if (dailyMatch.Success)
            {
                config.PlatformComparison.IOSDailyDownloads = ParseDouble(dailyMatch.Groups[1].Value);
                config.PlatformComparison.AndroidDailyDownloads = ParseDouble(dailyMatch.Groups[2].Value);
                config.PlatformComparison.IOSDailyDownloadsLast = ParseDouble(dailyMatch.Groups[3].Value);
                config.PlatformComparison.AndroidDailyDownloadsLast = ParseDouble(dailyMatch.Groups[4].Value);
            }

            // Crash Rate
            var crashMatch = Regex.Match(sectionText, @"Crash Rate\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)\s+([\d.,%-]+)");
            if (crashMatch.Success)
            {
                config.PlatformComparison.IOSCrashRate = ParseDouble(crashMatch.Groups[1].Value);
                config.PlatformComparison.AndroidCrashRate = ParseDouble(crashMatch.Groups[2].Value);
                config.PlatformComparison.IOSCrashRateLast = ParseDouble(crashMatch.Groups[3].Value);
                config.PlatformComparison.AndroidCrashRateLast = ParseDouble(crashMatch.Groups[4].Value);
            }
        }
    }

    private void ParseTechnicalSpecs(string text, MobileAppReportConfig config)
    {
        // Parse: "App Size: 45.0 MB" or "App Size: 1.2 GB"
        var appSizeMatch = Regex.Match(text, @"App Size:\s+([\d.]+)\s*(KB|MB|GB)");
        if (appSizeMatch.Success)
        {
            config.AppSize = ParseDouble(appSizeMatch.Groups[1].Value);
            config.AppSizeUnit = appSizeMatch.Groups[2].Value;
        }
        else
        {
            // Old format without unit or just with MB
            var oldMatch = Regex.Match(text, @"App Size:\s+([\d.]+)");
            if (oldMatch.Success)
            {
                config.AppSize = ParseDouble(oldMatch.Groups[1].Value);
                config.AppSizeUnit = "MB"; // Default to MB for old formats
            }
        }
    }

    private (double? current, double? last) ExtractMetricRow(string text, string metricName)
    {
        // Look for pattern: "MetricName {VALUE} {VALUE} {VALUE}" (3rd value is % change which we ignore)
        var pattern = Regex.Escape(metricName) + @"\s+([\d.,%-]+)\s+([\d.,%-]+)";
        var match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return (ParseDouble(match.Groups[1].Value), ParseDouble(match.Groups[2].Value));
        }

        return (null, null);
    }

    private double? ParseDouble(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "-")
            return null;

        // Remove common formatting: commas, percentage signs, plus signs
        value = value.Replace(",", "").Replace("%", "").Replace("+", "").Trim();

        if (double.TryParse(value, out var result))
            return result;

        return null;
    }

    private void MapCurrentToLast(MobileAppReportConfig config)
    {
        // Map the current period values from the imported report to the "last" period values
        // This allows you to import last month's report and have it pre-fill the comparison values

        // Move the current report dates to last report dates
        config.LastReportMonth = config.ReportMonth;
        config.LastReportDay = config.ReportDay;
        config.LastReportYear = config.ReportYear;

        // Clear current date so user fills in new month
        if (config.ReportYear.HasValue && config.ReportMonth.HasValue && config.ReportDay.HasValue)
        {
            var nextMonth = new DateTime(config.ReportYear.Value, config.ReportMonth.Value, config.ReportDay.Value).AddMonths(1);
            config.ReportMonth = nextMonth.Month;
            config.ReportDay = nextMonth.Day;
            config.ReportYear = nextMonth.Year;
        }

        // iOS Metrics - move current to last
        config.IOSMetrics.ImpressionsLast = config.IOSMetrics.Impressions;
        config.IOSMetrics.ProductPageViewsLast = config.IOSMetrics.ProductPageViews;
        config.IOSMetrics.ConversionRateLast = config.IOSMetrics.ConversionRate;
        config.IOSMetrics.TotalDownloadsLast = config.IOSMetrics.TotalDownloads;
        config.IOSMetrics.DailyDownloadsLast = config.IOSMetrics.DailyDownloads;
        config.IOSMetrics.SessionsPerDeviceLast = config.IOSMetrics.SessionsPerDevice;
        config.IOSMetrics.CrashRatePerSessionLast = config.IOSMetrics.CrashRatePerSession;
        config.IOSMetrics.TotalCrashesLast = config.IOSMetrics.TotalCrashes;

        // Clear current values (change values are computed properties, don't need to clear)
        config.IOSMetrics.Impressions = null;
        config.IOSMetrics.ProductPageViews = null;
        config.IOSMetrics.ConversionRate = null;
        config.IOSMetrics.TotalDownloads = null;
        config.IOSMetrics.DailyDownloads = null;
        config.IOSMetrics.SessionsPerDevice = null;
        config.IOSMetrics.CrashRatePerSession = null;
        config.IOSMetrics.TotalCrashes = null;

        // Download Sources - move current to last
        foreach (var source in config.IOSMetrics.DownloadSources)
        {
            source.LastPercentage = source.CurrentPercentage;
            source.LastDownloads = source.CurrentDownloads;
            source.CurrentPercentage = null;
            source.CurrentDownloads = null;
        }

        // Android Metrics - move current to last
        config.AndroidMetrics.TotalInstallsLast = config.AndroidMetrics.TotalInstalls;
        config.AndroidMetrics.DailyDownloadsLast = config.AndroidMetrics.DailyDownloads;
        config.AndroidMetrics.CrashRatePerSessionLast = config.AndroidMetrics.CrashRatePerSession;

        config.AndroidMetrics.TotalInstalls = null;
        config.AndroidMetrics.DailyDownloads = null;
        config.AndroidMetrics.CrashRatePerSession = null;

        // Platform Comparison - move current to last
        config.PlatformComparison.IOSTotalDownloadsLast = config.PlatformComparison.IOSTotalDownloads;
        config.PlatformComparison.AndroidTotalDownloadsLast = config.PlatformComparison.AndroidTotalDownloads;
        config.PlatformComparison.IOSUserPercentLast = config.PlatformComparison.IOSUserPercent;
        config.PlatformComparison.AndroidUserPercentLast = config.PlatformComparison.AndroidUserPercent;
        config.PlatformComparison.IOSDailyDownloadsLast = config.PlatformComparison.IOSDailyDownloads;
        config.PlatformComparison.AndroidDailyDownloadsLast = config.PlatformComparison.AndroidDailyDownloads;
        config.PlatformComparison.IOSCrashRateLast = config.PlatformComparison.IOSCrashRate;
        config.PlatformComparison.AndroidCrashRateLast = config.PlatformComparison.AndroidCrashRate;

        config.PlatformComparison.IOSTotalDownloads = null;
        config.PlatformComparison.AndroidTotalDownloads = null;
        config.PlatformComparison.IOSUserPercent = null;
        config.PlatformComparison.AndroidUserPercent = null;
        config.PlatformComparison.IOSDailyDownloads = null;
        config.PlatformComparison.AndroidDailyDownloads = null;
        config.PlatformComparison.IOSCrashRate = null;
        config.PlatformComparison.AndroidCrashRate = null;
    }
}

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportBuilder.Models;
using System.Text.Json;

namespace ReportBuilder.Services;

public class MobileAppReportGenerator : IReportGenerator<MobileAppReportConfig>
{
    public string ReportName => "Mobile App Store Performance Report";

    public byte[] GeneratePdf(MobileAppReportConfig config)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        // Serialize config to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false // Compact to save space
        };
        var configJson = JsonSerializer.Serialize(config, jsonOptions);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().ShowOnce().Element(c => ComposeHeader(c, config));
                page.Content().Element(c => ComposeContent(c, config));
                page.Footer().Element(c =>
                {
                    c.Column(column =>
                    {
                        column.Item().Element(container => ComposeFooter(container, config));

                        // Add invisible JSON data marker at the end of the document
                        column.Item().Text($"<!--REPORT_CONFIG_JSON:{configJson}-->")
                            .FontSize(0.1f)
                            .FontColor(Colors.White); // Invisible white text
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text($"{config.CompanyName} App Performance Report")
                .FontSize(20)
                .Bold()
                .FontColor(Colors.Blue.Darken2);

            column.Item().PaddingTop(5).Text($"{config.GetFormattedReportDate()}, Created by {config.CreatedByName}, {config.CreatedByTitle}")
                .FontSize(10)
                .FontColor(Colors.Grey.Darken1);

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private void ComposeFooter(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span($"Report Version {config.Version} | Generated: {DateTime.Now:M/d/yy HH:mm}");
                });

                row.RelativeItem().AlignCenter(); // Empty space for balance

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                });
            });
        });
    }

    private void ComposeContent(IContainer container, MobileAppReportConfig config)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Executive Summary
            if (config.IncludeExecutiveSummary)
            {
                column.Item().Element(c => ComposeExecutiveSummary(c, config));
                column.Item().PaddingTop(20);
            }

            // iOS Platform Performance
            if (config.IncludeIOSSection)
            {
                column.Item().Element(c => ComposeIOSSection(c, config));
                column.Item().PaddingTop(20);
            }

            // Android Platform Performance
            if (config.IncludeAndroidSection)
            {
                column.Item().Element(c => ComposeAndroidSection(c, config));
                column.Item().PaddingTop(20);
            }

            // Platform Comparison
            if (config.IncludePlatformComparison)
            {
                column.Item().Element(c => ComposePlatformComparison(c, config));
                column.Item().PaddingTop(20);
            }

            // High Variance Metrics
            if (config.IncludeHighVarianceMetrics && config.IncludeLastPeriodData)
            {
                column.Item().Element(c => ComposeHighVarianceMetrics(c, config));
                column.Item().PaddingTop(20);
            }

            // Technical Specifications
            if (config.IncludeTechnicalSpecifications)
            {
                column.Item().Element(c => ComposeTechnicalSpecifications(c, config));
            }
        });
    }

    private void ComposeExecutiveSummary(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text("Executive Summary")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            // Use user-entered summary if provided, otherwise show a default message
            var summaryText = !string.IsNullOrWhiteSpace(config.ExecutiveSummary)
                ? config.ExecutiveSummary
                : $"This report provides a comprehensive overview of the {config.CompanyName} app performance metrics as of {config.GetFormattedReportDate()}, covering both iOS and Android platforms.";

            column.Item().PaddingTop(8).Text(summaryText)
                .FontSize(10)
                .LineHeight(1.5f);
        });
    }

    private void ComposeIOSSection(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text("iOS Platform Performance")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(10).Text("Key Metrics Overview")
                .FontSize(13)
                .SemiBold()
                .FontColor(Colors.Blue.Darken1);

            column.Item().PaddingTop(8).Element(c => ComposeIOSMetricsTable(c, config));

            // Download Sources
            if (config.IOSMetrics.DownloadSources.Any())
            {
                column.Item().PaddingTop(15).Text("Download Sources")
                    .FontSize(13)
                    .SemiBold()
                    .FontColor(Colors.Blue.Darken1);

                column.Item().PaddingTop(5).Text($"Analysis of {FormatValue(config.IOSMetrics.TotalDownloads)} total downloads by acquisition channel:")
                    .FontSize(10)
                    .Italic();

                column.Item().PaddingTop(8).Element(c => ComposeDownloadSourcesTable(c, config));
            }
        });
    }

    private void ComposeIOSMetricsTable(IContainer container, MobileAppReportConfig config)
    {
        container.Table(table =>
        {
            if (config.IncludeLastPeriodData)
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Percent Change").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Impressions", config.IOSMetrics.Impressions, config.IOSMetrics.ImpressionsLast, config.IOSMetrics.ImpressionsChange, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Product Page Views", config.IOSMetrics.ProductPageViews, config.IOSMetrics.ProductPageViewsLast, config.IOSMetrics.ProductPageViewsChange, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Conversion Rate", config.IOSMetrics.ConversionRate, config.IOSMetrics.ConversionRateLast, config.IOSMetrics.ConversionRateChange, config.IncludeLastPeriodData, 2, true);
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, config.IOSMetrics.TotalDownloadsLast, config.IOSMetrics.TotalDownloadsChange, config.IncludeLastPeriodData, 3);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, config.IOSMetrics.DailyDownloadsLast, config.IOSMetrics.DailyDownloadsChange, config.IncludeLastPeriodData, 4);
                AddTableRow(table, "Sessions per Device", config.IOSMetrics.SessionsPerDevice, config.IOSMetrics.SessionsPerDeviceLast, config.IOSMetrics.SessionsPerDeviceChange, config.IncludeLastPeriodData, 5);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, config.IOSMetrics.TotalCrashesLast, config.IOSMetrics.TotalCrashesChange, config.IncludeLastPeriodData, 6);
                AddTableRow(table, "Devices Active within Past 30 Days", config.IOSMetrics.DevicesActiveWithin30Days, config.IOSMetrics.DevicesActiveWithin30DaysLast, config.IOSMetrics.DevicesActiveWithin30DaysChange, config.IncludeLastPeriodData, 7);
                AddTableRow(table, "Lifetime Deletions", config.IOSMetrics.LifetimeDeletions, config.IOSMetrics.LifetimeDeletionsLast, config.IOSMetrics.LifetimeDeletionsChange, config.IncludeLastPeriodData, 8);
                AddTableRow(table, "Lifetime Re-Downloads", config.IOSMetrics.LifetimeReDownloads, config.IOSMetrics.LifetimeReDownloadsLast, config.IOSMetrics.LifetimeReDownloadsChange, config.IncludeLastPeriodData, 9);
            }
            else
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1.5f);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Impressions", config.IOSMetrics.Impressions, null, null, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Product Page Views", config.IOSMetrics.ProductPageViews, null, null, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Conversion Rate", config.IOSMetrics.ConversionRate, null, null, config.IncludeLastPeriodData, 2);
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, null, null, config.IncludeLastPeriodData, 3);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData, 4);
                AddTableRow(table, "Sessions per Device", config.IOSMetrics.SessionsPerDevice, null, null, config.IncludeLastPeriodData, 5);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, null, null, config.IncludeLastPeriodData, 6);
                AddTableRow(table, "Devices Active within Past 30 Days", config.IOSMetrics.DevicesActiveWithin30Days, null, null, config.IncludeLastPeriodData, 7);
                AddTableRow(table, "Lifetime Deletions", config.IOSMetrics.LifetimeDeletions, null, null, config.IncludeLastPeriodData, 8);
                AddTableRow(table, "Lifetime Re-Downloads", config.IOSMetrics.LifetimeReDownloads, null, null, config.IncludeLastPeriodData, 9);
            }
        });
    }

    private void ComposeDownloadSourcesTable(IContainer container, MobileAppReportConfig config)
    {
        container.Table(table =>
        {
            if (config.IncludeLastPeriodData)
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Source").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Percent, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Downloads, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(8);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Percent, as of {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Downloads, as of {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold().FontSize(8);
                });

                // Rows
                int rowIndex = 0;
                foreach (var source in config.IOSMetrics.DownloadSources)
                {
                    var bgColor = (rowIndex % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
                    table.Cell().Background(bgColor).Padding(6).Text(source.Name);
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.CurrentPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.CurrentDownloads));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.LastPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.LastDownloads));
                    rowIndex++;
                }
            }
            else
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Source").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Percent, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Downloads, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(8);
                });

                // Rows
                int rowIndex = 0;
                foreach (var source in config.IOSMetrics.DownloadSources)
                {
                    var bgColor = (rowIndex % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
                    table.Cell().Background(bgColor).Padding(6).Text(source.Name);
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.CurrentPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.CurrentDownloads));
                    rowIndex++;
                }
            }
        });
    }

    private void ComposeAndroidSection(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text("Android Platform Performance")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(8).Element(c => ComposeAndroidMetricsTable(c, config));
        });
    }

    private void ComposeAndroidMetricsTable(IContainer container, MobileAppReportConfig config)
    {
        container.Table(table =>
        {
            if (config.IncludeLastPeriodData)
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Percent Change").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Total Installs", config.AndroidMetrics.TotalInstalls, config.AndroidMetrics.TotalInstallsLast, config.AndroidMetrics.TotalInstallsChange, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, config.AndroidMetrics.DailyDownloadsLast, config.AndroidMetrics.DailyDownloadsChange, config.IncludeLastPeriodData, 1);
            }
            else
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1.5f);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"As of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Total Installs", config.AndroidMetrics.TotalInstalls, null, null, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData, 1);
            }
        });
    }

    private void ComposePlatformComparison(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text("Platform Comparison")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(8).Element(c => ComposePlatformComparisonTable(c, config));
        });
    }

    private void ComposePlatformComparisonTable(IContainer container, MobileAppReportConfig config)
    {
        container.Table(table =>
        {
            if (config.IncludeLastPeriodData)
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1f);
                    columns.RelativeColumn(1f);
                });

                // Header - Two rows for better organization
                table.Header(header =>
                {
                    // First header row - Platform grouping
                    header.Cell().RowSpan(2).Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().ColumnSpan(2).Background(Colors.Blue.Darken3).Padding(8).Text($"Current Period (as of {config.GetFormattedReportDate()})").FontColor(Colors.White).Bold().FontSize(9).AlignCenter();
                    header.Cell().ColumnSpan(2).Background(Colors.Blue.Darken3).Padding(8).Text("Percent Change").FontColor(Colors.White).Bold().FontSize(9).AlignCenter();

                    // Second header row - Specific platforms
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("iOS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Android").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("iOS").FontColor(Colors.White).Bold().FontSize(8);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Android").FontColor(Colors.White).Bold().FontSize(8);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads,
                    config.PlatformComparison.IOSTotalDownloadsLast, config.PlatformComparison.AndroidTotalDownloadsLast,
                    config.PlatformComparison.IOSTotalDownloadsChange, config.PlatformComparison.AndroidTotalDownloadsChange, config.IncludeLastPeriodData, 0);

                AddPlatformComparisonRow(table, $"User Percent of all {config.CompanyName} Apps",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent,
                    config.PlatformComparison.IOSUserPercentLast, config.PlatformComparison.AndroidUserPercentLast,
                    config.PlatformComparison.IOSUserPercentChange, config.PlatformComparison.AndroidUserPercentChange, config.IncludeLastPeriodData, 1, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads,
                    config.PlatformComparison.IOSDailyDownloadsLast, config.PlatformComparison.AndroidDailyDownloadsLast,
                    config.PlatformComparison.IOSDailyDownloadsChange, config.PlatformComparison.AndroidDailyDownloadsChange, config.IncludeLastPeriodData, 2);

                AddPlatformComparisonRow(table, "Crash Rate",
                    config.PlatformComparison.IOSCrashRate, config.PlatformComparison.AndroidCrashRate,
                    config.PlatformComparison.IOSCrashRateLast, config.PlatformComparison.AndroidCrashRateLast,
                    config.PlatformComparison.IOSCrashRateChange, config.PlatformComparison.AndroidCrashRateChange, config.IncludeLastPeriodData, 3, true);
            }
            else
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"iOS, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Android, as of {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads,
                    null, null, null, null, config.IncludeLastPeriodData, 0);

                AddPlatformComparisonRow(table, $"User Percent of all {config.CompanyName} Apps",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent,
                    null, null, null, null, config.IncludeLastPeriodData, 1, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads,
                    null, null, null, null, config.IncludeLastPeriodData, 2);

                AddPlatformComparisonRow(table, "Crash Rate",
                    config.PlatformComparison.IOSCrashRate, config.PlatformComparison.AndroidCrashRate,
                    null, null, null, null, config.IncludeLastPeriodData, 3, true);
            }
        });
    }

    private void ComposeTechnicalSpecifications(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().Text("Technical Specifications")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(8).Column(col =>
            {
                if (config.AppSize.HasValue)
                {
                    var unit = string.IsNullOrEmpty(config.AppSizeUnit) ? "MB" : config.AppSizeUnit;
                    col.Item().Text($"App Size: {config.AppSize.Value:N1} {unit}").FontSize(10).LineHeight(1.5f);
                }
                col.Item().Text($"Report Date: {config.GetFormattedReportDate()}").FontSize(10).LineHeight(1.5f);
                col.Item().Text("Data Sources: Apple App Store Connect, Google Play Console").FontSize(10).LineHeight(1.5f);
            });
        });
    }

    private void ComposeHighVarianceMetrics(IContainer container, MobileAppReportConfig config)
    {
        var varianceThreshold = config.HighVarianceThreshold;
        var highVarianceItems = new List<(string metric, string section, double? change)>();

        // Collect iOS metrics with high variance
        if (config.IOSMetrics.ImpressionsChange.HasValue && Math.Abs(config.IOSMetrics.ImpressionsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Impressions (iOS)", "iOS Metrics", config.IOSMetrics.ImpressionsChange.Value));
        if (config.IOSMetrics.ProductPageViewsChange.HasValue && Math.Abs(config.IOSMetrics.ProductPageViewsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Product Page Views (iOS)", "iOS Metrics", config.IOSMetrics.ProductPageViewsChange.Value));
        if (config.IOSMetrics.ConversionRateChange.HasValue && Math.Abs(config.IOSMetrics.ConversionRateChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Conversion Rate (iOS)", "iOS Metrics", config.IOSMetrics.ConversionRateChange.Value));
        if (config.IOSMetrics.TotalDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.TotalDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Downloads (iOS)", "iOS Metrics", config.IOSMetrics.TotalDownloadsChange.Value));
        if (config.IOSMetrics.DailyDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.DailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads (iOS)", "iOS Metrics", config.IOSMetrics.DailyDownloadsChange.Value));
        if (config.IOSMetrics.SessionsPerDeviceChange.HasValue && Math.Abs(config.IOSMetrics.SessionsPerDeviceChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Sessions per Device (iOS)", "iOS Metrics", config.IOSMetrics.SessionsPerDeviceChange.Value));
        if (config.IOSMetrics.TotalCrashesChange.HasValue && Math.Abs(config.IOSMetrics.TotalCrashesChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Crashes (iOS)", "iOS Metrics", config.IOSMetrics.TotalCrashesChange.Value));
        if (config.IOSMetrics.DevicesActiveWithin30DaysChange.HasValue && Math.Abs(config.IOSMetrics.DevicesActiveWithin30DaysChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Devices Active within Past 30 Days (iOS)", "iOS Metrics", config.IOSMetrics.DevicesActiveWithin30DaysChange.Value));
        if (config.IOSMetrics.LifetimeDeletionsChange.HasValue && Math.Abs(config.IOSMetrics.LifetimeDeletionsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Lifetime Deletions (iOS)", "iOS Metrics", config.IOSMetrics.LifetimeDeletionsChange.Value));
        if (config.IOSMetrics.LifetimeReDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.LifetimeReDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Lifetime Re-Downloads (iOS)", "iOS Metrics", config.IOSMetrics.LifetimeReDownloadsChange.Value));

        // Collect Android metrics with high variance
        if (config.AndroidMetrics.TotalInstallsChange.HasValue && Math.Abs(config.AndroidMetrics.TotalInstallsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Installs (Android)", "Android Metrics", config.AndroidMetrics.TotalInstallsChange.Value));
        if (config.AndroidMetrics.DailyDownloadsChange.HasValue && Math.Abs(config.AndroidMetrics.DailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads (Android)", "Android Metrics", config.AndroidMetrics.DailyDownloadsChange.Value));
        if (config.AndroidMetrics.CrashRatePerSessionChange.HasValue && Math.Abs(config.AndroidMetrics.CrashRatePerSessionChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Crash Rate per Session (Android)", "Android Metrics", config.AndroidMetrics.CrashRatePerSessionChange.Value));

        // Collect Platform Comparison metrics with high variance
        if (config.PlatformComparison.IOSTotalDownloadsChange.HasValue && Math.Abs(config.PlatformComparison.IOSTotalDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Downloads - iOS", "Platform Comparison", config.PlatformComparison.IOSTotalDownloadsChange.Value));
        if (config.PlatformComparison.AndroidTotalDownloadsChange.HasValue && Math.Abs(config.PlatformComparison.AndroidTotalDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Downloads - Android", "Platform Comparison", config.PlatformComparison.AndroidTotalDownloadsChange.Value));
        if (config.PlatformComparison.IOSUserPercentChange.HasValue && Math.Abs(config.PlatformComparison.IOSUserPercentChange.Value) > varianceThreshold)
            highVarianceItems.Add(("User Percent - iOS", "Platform Comparison", config.PlatformComparison.IOSUserPercentChange.Value));
        if (config.PlatformComparison.AndroidUserPercentChange.HasValue && Math.Abs(config.PlatformComparison.AndroidUserPercentChange.Value) > varianceThreshold)
            highVarianceItems.Add(("User Percent - Android", "Platform Comparison", config.PlatformComparison.AndroidUserPercentChange.Value));
        if (config.PlatformComparison.IOSDailyDownloadsChange.HasValue && Math.Abs(config.PlatformComparison.IOSDailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads - iOS", "Platform Comparison", config.PlatformComparison.IOSDailyDownloadsChange.Value));
        if (config.PlatformComparison.AndroidDailyDownloadsChange.HasValue && Math.Abs(config.PlatformComparison.AndroidDailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads - Android", "Platform Comparison", config.PlatformComparison.AndroidDailyDownloadsChange.Value));
        if (config.PlatformComparison.IOSCrashRateChange.HasValue && Math.Abs(config.PlatformComparison.IOSCrashRateChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Crash Rate - iOS", "Platform Comparison", config.PlatformComparison.IOSCrashRateChange.Value));
        if (config.PlatformComparison.AndroidCrashRateChange.HasValue && Math.Abs(config.PlatformComparison.AndroidCrashRateChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Crash Rate - Android", "Platform Comparison", config.PlatformComparison.AndroidCrashRateChange.Value));

        if (highVarianceItems.Count == 0)
            return;

        container.Column(column =>
        {
            column.Item().Text($"High Variance Metrics (over ±{config.HighVarianceThreshold}%)")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(8).Text($"The following metrics experienced significant changes (greater than ±{config.HighVarianceThreshold}%) from the previous period:")
                .FontSize(10)
                .Italic();

            column.Item().PaddingTop(10).Element(c =>
            {
                c.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(1.5f);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Section").FontColor(Colors.White).Bold();
                        header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Percent Change").FontColor(Colors.White).Bold();
                    });

                    // Rows
                    for (int i = 0; i < highVarianceItems.Count; i++)
                    {
                        var (metric, section, change) = highVarianceItems[i];
                        var bgColor = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
                        table.Cell().Background(bgColor).Padding(6).Text(metric);
                        table.Cell().Background(bgColor).Padding(6).Text(section);
                        table.Cell().Background(bgColor).Padding(6).Text(FormatPercentChange(change));
                    }
                });
            });
        });
    }

    private void AddTableRow(TableDescriptor table, string metric, double? current, double? last, double? change, bool includeLastPeriod, int rowIndex = 0, bool isPercent = false)
    {
        var bgColor = (rowIndex % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
        table.Cell().Background(bgColor).Padding(6).Text(metric);
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(current) : FormatValue(current));

        if (includeLastPeriod)
        {
            table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(last) : FormatValue(last));
            table.Cell().Background(bgColor).Padding(6).Text(FormatPercentChange(change));
        }
    }

    private void AddPlatformComparisonRow(TableDescriptor table, string metric,
        double? iosCurrent, double? androidCurrent,
        double? iosLast, double? androidLast,
        double? iosChange, double? androidChange, bool includeLastPeriod, int rowIndex = 0, bool isPercent = false)
    {
        var bgColor = (rowIndex % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
        table.Cell().Background(bgColor).Padding(6).Text(metric);
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(iosCurrent) : FormatValue(iosCurrent));
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(androidCurrent) : FormatValue(androidCurrent));

        if (includeLastPeriod)
        {
            table.Cell().Background(bgColor).Padding(6).Text(FormatPercentChange(iosChange));
            table.Cell().Background(bgColor).Padding(6).Text(FormatPercentChange(androidChange));
        }
    }

    private string FormatValue(double? value)
    {
        if (!value.HasValue)
            return "-";

        // Format with thousands separator
        return value.Value.ToString("N0");
    }

    private string FormatPercentage(double? value)
    {
        if (!value.HasValue)
            return "-";

        // Use more decimal places if value is small (less than 1)
        if (Math.Abs(value.Value) < 1)
            return $"{value.Value:0.##}%";

        return $"{value.Value:N2}%";
    }

    private string FormatPercentChange(double? value)
    {
        if (!value.HasValue)
            return "-";

        var sign = value.Value >= 0 ? "+" : "";

        // Use more decimal places if value is small (less than 1)
        if (Math.Abs(value.Value) < 1)
            return $"{sign}{value.Value:0.##}%";

        return $"{sign}{value.Value:N2}%";
    }
}







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
            column.Item().PageBreak();
            column.Item().Text("iOS Platform Performance")
                .FontSize(16)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingTop(10).Text("Key Metrics Overview")
                .FontSize(13)
                .SemiBold()
                .FontColor(Colors.Blue.Darken1);

            column.Item().PaddingTop(8).Element(c => ComposeIOSMetricsTable(c, config));

            // DAU by Version Breakdown
            if (config.IncludeIOSDailyActiveUsersByVersion && 
                config.IOSMetrics.DailyActiveUsersByVersionBreakdown.Any(v => !string.IsNullOrWhiteSpace(v.VersionNumber) && v.DailyActiveUsers.HasValue))
            {
                column.Item().PaddingTop(15).Element(c => ComposeVersionBreakdownVisualization(c, config, true));
            }

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
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, config.IOSMetrics.TotalDownloadsLast, config.IOSMetrics.TotalDownloadsChange, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, config.IOSMetrics.DailyDownloadsLast, config.IOSMetrics.DailyDownloadsChange, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, config.IOSMetrics.TotalCrashesLast, config.IOSMetrics.TotalCrashesChange, config.IncludeLastPeriodData, 2);
                AddTableRow(table, "Daily Active Users (30-Day Avg)", config.IOSMetrics.DevicesActiveWithin30Days, config.IOSMetrics.DevicesActiveWithin30DaysLast, config.IOSMetrics.DevicesActiveWithin30DaysChange, config.IncludeLastPeriodData, 3);
                AddTableRow(table, "Lifetime Deletions", config.IOSMetrics.LifetimeDeletions, config.IOSMetrics.LifetimeDeletionsLast, config.IOSMetrics.LifetimeDeletionsChange, config.IncludeLastPeriodData, 4);
                AddTableRow(table, "Lifetime Re-Downloads", config.IOSMetrics.LifetimeReDownloads, config.IOSMetrics.LifetimeReDownloadsLast, config.IOSMetrics.LifetimeReDownloadsChange, config.IncludeLastPeriodData, 5);
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
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, null, null, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, null, null, config.IncludeLastPeriodData, 2);
                AddTableRow(table, "Daily Active Users (30-Day Avg)", config.IOSMetrics.DevicesActiveWithin30Days, null, null, config.IncludeLastPeriodData, 3);
                AddTableRow(table, "Lifetime Deletions", config.IOSMetrics.LifetimeDeletions, null, null, config.IncludeLastPeriodData, 4);
                AddTableRow(table, "Lifetime Re-Downloads", config.IOSMetrics.LifetimeReDownloads, null, null, config.IncludeLastPeriodData, 6);
            }
        });
    }

    private void ComposeDownloadSourcesTable(IContainer container, MobileAppReportConfig config)
    {
        var sources = config.IOSMetrics.DownloadSources.Where(s => !string.IsNullOrWhiteSpace(s.Name)).ToList();
        if (!sources.Any()) return;

        // Use total downloads as the scale, not the max value among sources
        var totalDownloads = config.IOSMetrics.TotalDownloads ?? 0;

        container.Column(column =>
        {
            column.Item().PaddingTop(10).Column(barColumn =>
            {
                foreach (var source in sources.OrderByDescending(s => s.CurrentDownloads))
                {
                    var currentDownloads = source.CurrentDownloads ?? 0;
                    var currentPercentage = source.CurrentPercentage ?? 0;
                    var currentBarWidth = totalDownloads > 0 ? (currentDownloads / (double)totalDownloads) : 0;

                    barColumn.Item().PaddingBottom(10).Row(row =>
                    {
                        // Left side: Source name (fixed width, bold and black like DAU)
                        row.ConstantItem(120).AlignMiddle().Text(source.Name)
                            .FontSize(9)
                            .Bold()
                            .FontColor(Colors.Black);

                        // Right side: Bar with value
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Row(barRow =>
                            {
                                if (currentBarWidth > 0)
                                {
                                    barRow.RelativeItem((float)currentBarWidth)
                                        .Height(20)
                                        .Background(Colors.Blue.Medium)
                                        .AlignMiddle()
                                        .PaddingLeft(5)
                                        .Text($"{FormatValue(currentDownloads)} ({currentPercentage:F1}%)")
                                        .FontSize(8)
                                        .FontColor(Colors.White)
                                        .Bold();
                                }

                                if (currentBarWidth < 1.0)
                                {
                                    barRow.RelativeItem((float)(1.0 - currentBarWidth))
                                        .Height(20)
                                        .Background(Colors.Grey.Lighten3);
                                }
                            });
                        });
                    });
                }
            });
        });
    }

    private void ComposeVersionBreakdownVisualization(IContainer container, MobileAppReportConfig config, bool isIOS)
    {
        var versions = isIOS 
            ? config.IOSMetrics.DailyActiveUsersByVersionBreakdown.Where(v => !string.IsNullOrWhiteSpace(v.VersionNumber) && v.DailyActiveUsers.HasValue).ToList()
            : new List<VersionDAU>(); // Android support can be added later
        
        if (!versions.Any()) return;

        var totalDAU = versions.Sum(v => v.DailyActiveUsers ?? 0);
        var maxDAU = versions.Max(v => v.DailyActiveUsers ?? 0);

        container.Column(column =>
        {
            column.Item().Text("Daily Active Users by App Version (30-Day Avg)")
                .FontSize(13)
                .SemiBold()
                .FontColor(Colors.Blue.Darken1);

            column.Item().PaddingTop(5).Text($"Distribution of {FormatValue(totalDAU)} daily active users across app versions:")
                .FontSize(10)
                .Italic();

            column.Item().PaddingTop(10).Column(barColumn =>
            {
                foreach (var version in versions.OrderByDescending(v => v.DailyActiveUsers))
                {
                    var dau = version.DailyActiveUsers ?? 0;
                    var percentage = totalDAU > 0 ? (dau / (double)totalDAU) * 100 : 0;
                    var barWidth = totalDAU > 0 ? (dau / (double)totalDAU) : 0;

                    barColumn.Item().PaddingBottom(8).Row(row =>
                    {
                        // Version label (left aligned, fixed width)
                        row.ConstantItem(120).AlignMiddle().Text($"Version {version.VersionNumber}")
                            .FontSize(10)
                            .SemiBold()
                            .FontColor(Colors.Black);

                        // Bar visualization
                        row.RelativeItem().AlignMiddle().Row(barRow =>
                        {
                            // Filled portion of bar
                            barRow.RelativeItem((float)barWidth)
                                .Height(20)
                                .Background(Colors.Blue.Medium)
                                .AlignMiddle()
                                .PaddingHorizontal(5)
                                .Text($"{FormatValue(dau)} ({percentage:F1}%)")
                                .FontSize(9)
                                .FontColor(Colors.White)
                                .Bold();

                            // Empty portion of bar
                            if (barWidth < 1.0)
                            {
                                barRow.RelativeItem((float)(1.0 - barWidth))
                                    .Height(20)
                                    .Background(Colors.Grey.Lighten3);
                            }
                        });
                    });
                }
            });
        });
    }

    private void ComposeAndroidSection(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().PageBreak();
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
                AddTableRow(table, "Total Downloads", config.AndroidMetrics.TotalDownloads, config.AndroidMetrics.TotalDownloadsLast, config.AndroidMetrics.TotalDownloadsChange, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, config.AndroidMetrics.DailyDownloadsLast, config.AndroidMetrics.DailyDownloadsChange, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Total Crashes", config.AndroidMetrics.TotalCrashes, config.AndroidMetrics.TotalCrashesLast, config.AndroidMetrics.TotalCrashesChange, config.IncludeLastPeriodData, 2);
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
                AddTableRow(table, "Total Downloads", config.AndroidMetrics.TotalDownloads, null, null, config.IncludeLastPeriodData, 0);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData, 1);
                AddTableRow(table, "Total Crashes", config.AndroidMetrics.TotalCrashes, null, null, config.IncludeLastPeriodData, 2);
            }
        });
    }

    private void ComposePlatformComparison(IContainer container, MobileAppReportConfig config)
    {
        container.Column(column =>
        {
            column.Item().PageBreak();
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
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().RowSpan(1).Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("iOS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Android").FontColor(Colors.White).Bold().FontSize(9);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads, config.IncludeLastPeriodData, 0);

                AddPlatformComparisonRow(table, "User Base Percent",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent, config.IncludeLastPeriodData, 1, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads, config.IncludeLastPeriodData, 2);

                AddPlatformComparisonRow(table, "Total Crashes",
                    config.PlatformComparison.IOSTotalCrashes, config.PlatformComparison.AndroidTotalCrashes, config.IncludeLastPeriodData, 3);
            }
            else
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("iOS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Android").FontColor(Colors.White).Bold().FontSize(9);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads, config.IncludeLastPeriodData, 0);

                AddPlatformComparisonRow(table, "User Base Percent",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent, config.IncludeLastPeriodData, 1, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads, config.IncludeLastPeriodData, 2);

                AddPlatformComparisonRow(table, "Total Crashes",
                    config.PlatformComparison.IOSTotalCrashes, config.PlatformComparison.AndroidTotalCrashes, config.IncludeLastPeriodData, 3);
            }
        });
    }

    private void ComposeHighVarianceMetrics(IContainer container, MobileAppReportConfig config)
    {
        var varianceThreshold = config.HighVarianceThreshold;
        var highVarianceItems = new List<(string metric, string section, double? change)>();

        // Collect iOS metrics with high variance
        if (config.IOSMetrics.TotalDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.TotalDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Downloads (iOS)", "iOS Metrics", config.IOSMetrics.TotalDownloadsChange.Value));
        if (config.IOSMetrics.DailyDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.DailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads (iOS)", "iOS Metrics", config.IOSMetrics.DailyDownloadsChange.Value));
        if (config.IOSMetrics.TotalCrashesChange.HasValue && Math.Abs(config.IOSMetrics.TotalCrashesChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Crashes (iOS)", "iOS Metrics", config.IOSMetrics.TotalCrashesChange.Value));
        if (config.IOSMetrics.DevicesActiveWithin30DaysChange.HasValue && Math.Abs(config.IOSMetrics.DevicesActiveWithin30DaysChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Active Users (30-Day Avg) (iOS)", "iOS Metrics", config.IOSMetrics.DevicesActiveWithin30DaysChange.Value));
        if (config.IOSMetrics.LifetimeDeletionsChange.HasValue && Math.Abs(config.IOSMetrics.LifetimeDeletionsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Lifetime Deletions (iOS)", "iOS Metrics", config.IOSMetrics.LifetimeDeletionsChange.Value));
        if (config.IOSMetrics.LifetimeReDownloadsChange.HasValue && Math.Abs(config.IOSMetrics.LifetimeReDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Lifetime Re-Downloads (iOS)", "iOS Metrics", config.IOSMetrics.LifetimeReDownloadsChange.Value));

        // Collect Android metrics with high variance
        if (config.AndroidMetrics.TotalDownloadsChange.HasValue && Math.Abs(config.AndroidMetrics.TotalDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Downloads (Android)", "Android Metrics", config.AndroidMetrics.TotalDownloadsChange.Value));
        if (config.AndroidMetrics.DailyDownloadsChange.HasValue && Math.Abs(config.AndroidMetrics.DailyDownloadsChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Daily Downloads (Android)", "Android Metrics", config.AndroidMetrics.DailyDownloadsChange.Value));
        if (config.AndroidMetrics.TotalCrashesChange.HasValue && Math.Abs(config.AndroidMetrics.TotalCrashesChange.Value) > varianceThreshold)
            highVarianceItems.Add(("Total Crashes (Android)", "Android Metrics", config.AndroidMetrics.TotalCrashesChange.Value));

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
        double? iosCurrent, double? androidCurrent, bool includeLastPeriod, int rowIndex = 0, bool isPercent = false)
    {
        var bgColor = (rowIndex % 2 == 0) ? Colors.White : Colors.Grey.Lighten3;
        table.Cell().Background(bgColor).Padding(6).Text(metric);
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(iosCurrent) : FormatValue(iosCurrent));
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(androidCurrent) : FormatValue(androidCurrent));
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

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}







using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportBuilder.Models;

namespace ReportBuilder.Services;

public class MobileAppReportGenerator : IReportGenerator<MobileAppReportConfig>
{
    public string ReportName => "Mobile App Store Performance Report";

    public byte[] GeneratePdf(MobileAppReportConfig config)
    {
        QuestPDF.Settings.License = LicenseType.Community;

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
                page.Footer().Element(c => ComposeFooter(c, config));
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
                    text.Span($"Report Version {config.Version} | Generated: {DateTime.Now:M/d/yy h:mm tt}");
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("% Change").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Impressions", config.IOSMetrics.Impressions, config.IOSMetrics.ImpressionsLast, config.IOSMetrics.ImpressionsChange, config.IncludeLastPeriodData);
                AddTableRow(table, "Product Page Views", config.IOSMetrics.ProductPageViews, config.IOSMetrics.ProductPageViewsLast, config.IOSMetrics.ProductPageViewsChange, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Conversion Rate", config.IOSMetrics.ConversionRate, config.IOSMetrics.ConversionRateLast, config.IOSMetrics.ConversionRateChange, config.IncludeLastPeriodData);
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, config.IOSMetrics.TotalDownloadsLast, config.IOSMetrics.TotalDownloadsChange, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, config.IOSMetrics.DailyDownloadsLast, config.IOSMetrics.DailyDownloadsChange, config.IncludeLastPeriodData);
                AddTableRow(table, "Sessions per Device", config.IOSMetrics.SessionsPerDevice, config.IOSMetrics.SessionsPerDeviceLast, config.IOSMetrics.SessionsPerDeviceChange, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Crash Rate per Session", config.IOSMetrics.CrashRatePerSession, config.IOSMetrics.CrashRatePerSessionLast, config.IOSMetrics.CrashRatePerSessionChange, config.IncludeLastPeriodData, true, true);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, config.IOSMetrics.TotalCrashesLast, config.IOSMetrics.TotalCrashesChange, config.IncludeLastPeriodData, true);
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Impressions", config.IOSMetrics.Impressions, null, null, config.IncludeLastPeriodData);
                AddTableRow(table, "Product Page Views", config.IOSMetrics.ProductPageViews, null, null, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Conversion Rate", config.IOSMetrics.ConversionRate, null, null, config.IncludeLastPeriodData);
                AddTableRow(table, "Total Downloads", config.IOSMetrics.TotalDownloads, null, null, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Daily Downloads", config.IOSMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData);
                AddTableRow(table, "Sessions per Device", config.IOSMetrics.SessionsPerDevice, null, null, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Crash Rate per Session", config.IOSMetrics.CrashRatePerSession, null, null, config.IncludeLastPeriodData, true, true);
                AddTableRow(table, "Total Crashes", config.IOSMetrics.TotalCrashes, null, null, config.IncludeLastPeriodData, true);
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
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Source").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()} %").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()} Downloads").FontColor(Colors.White).Bold().FontSize(8);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedLastReportDate()} %").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedLastReportDate()} Downloads").FontColor(Colors.White).Bold().FontSize(8);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("% Change").FontColor(Colors.White).Bold();
                });

                // Rows
                bool alternate = false;
                foreach (var source in config.IOSMetrics.DownloadSources)
                {
                    var bgColor = alternate ? Colors.Grey.Lighten3 : Colors.White;
                    table.Cell().Background(bgColor).Padding(6).Text(source.Name);
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.CurrentPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.CurrentDownloads));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.LastPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.LastDownloads));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentChange(source.DownloadsChange));
                    alternate = !alternate;
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()} %").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()} Downloads").FontColor(Colors.White).Bold().FontSize(8);
                });

                // Rows
                bool alternate = false;
                foreach (var source in config.IOSMetrics.DownloadSources)
                {
                    var bgColor = alternate ? Colors.Grey.Lighten3 : Colors.White;
                    table.Cell().Background(bgColor).Padding(6).Text(source.Name);
                    table.Cell().Background(bgColor).Padding(6).Text(FormatPercentage(source.CurrentPercentage));
                    table.Cell().Background(bgColor).Padding(6).Text(FormatValue(source.CurrentDownloads));
                    alternate = !alternate;
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("% Change").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Total Installs", config.AndroidMetrics.TotalInstalls, config.AndroidMetrics.TotalInstallsLast, config.AndroidMetrics.TotalInstallsChange, config.IncludeLastPeriodData);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, config.AndroidMetrics.DailyDownloadsLast, config.AndroidMetrics.DailyDownloadsChange, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Crash Rate per Session", config.AndroidMetrics.CrashRatePerSession, config.AndroidMetrics.CrashRatePerSessionLast, config.AndroidMetrics.CrashRatePerSessionChange, config.IncludeLastPeriodData, true, true);
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"{config.GetFormattedReportDate()}").FontColor(Colors.White).Bold();
                });

                // Rows
                AddTableRow(table, "Total Installs", config.AndroidMetrics.TotalInstalls, null, null, config.IncludeLastPeriodData);
                AddTableRow(table, "Daily Downloads", config.AndroidMetrics.DailyDownloads, null, null, config.IncludeLastPeriodData, true);
                AddTableRow(table, "Crash Rate per Session", config.AndroidMetrics.CrashRatePerSession, null, null, config.IncludeLastPeriodData, true, true);
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
                    columns.RelativeColumn(1.5f);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1);
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Metric").FontColor(Colors.White).Bold();
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"iOS {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Android {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"iOS {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Android {config.GetFormattedLastReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("iOS % Change").FontColor(Colors.White).Bold().FontSize(8);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text("Android % Change").FontColor(Colors.White).Bold().FontSize(8);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads,
                    config.PlatformComparison.IOSTotalDownloadsLast, config.PlatformComparison.AndroidTotalDownloadsLast,
                    config.PlatformComparison.IOSTotalDownloadsChange, config.PlatformComparison.AndroidTotalDownloadsChange, config.IncludeLastPeriodData);

                AddPlatformComparisonRow(table, $"Download base of {config.CompanyName} All",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent,
                    config.PlatformComparison.IOSUserPercentLast, config.PlatformComparison.AndroidUserPercentLast,
                    config.PlatformComparison.IOSUserPercentChange, config.PlatformComparison.AndroidUserPercentChange, config.IncludeLastPeriodData, true, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads,
                    config.PlatformComparison.IOSDailyDownloadsLast, config.PlatformComparison.AndroidDailyDownloadsLast,
                    config.PlatformComparison.IOSDailyDownloadsChange, config.PlatformComparison.AndroidDailyDownloadsChange, config.IncludeLastPeriodData);

                AddPlatformComparisonRow(table, "Crash Rate",
                    config.PlatformComparison.IOSCrashRate, config.PlatformComparison.AndroidCrashRate,
                    config.PlatformComparison.IOSCrashRateLast, config.PlatformComparison.AndroidCrashRateLast,
                    config.PlatformComparison.IOSCrashRateChange, config.PlatformComparison.AndroidCrashRateChange, config.IncludeLastPeriodData, true, true);
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
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"iOS {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background(Colors.Blue.Darken2).Padding(8).Text($"Android {config.GetFormattedReportDate()}").FontColor(Colors.White).Bold().FontSize(9);
                });

                // Rows
                AddPlatformComparisonRow(table, "Total Downloads",
                    config.PlatformComparison.IOSTotalDownloads, config.PlatformComparison.AndroidTotalDownloads,
                    null, null, null, null, config.IncludeLastPeriodData);

                AddPlatformComparisonRow(table, $"User % of all {config.CompanyName} Apps",
                    config.PlatformComparison.IOSUserPercent, config.PlatformComparison.AndroidUserPercent,
                    null, null, null, null, config.IncludeLastPeriodData, true);

                AddPlatformComparisonRow(table, "Daily Downloads",
                    config.PlatformComparison.IOSDailyDownloads, config.PlatformComparison.AndroidDailyDownloads,
                    null, null, null, null, config.IncludeLastPeriodData);

                AddPlatformComparisonRow(table, "Crash Rate",
                    config.PlatformComparison.IOSCrashRate, config.PlatformComparison.AndroidCrashRate,
                    null, null, null, null, config.IncludeLastPeriodData, true);
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

    private void AddTableRow(TableDescriptor table, string metric, double? current, double? last, double? change, bool includeLastPeriod, bool alternate = false, bool isPercent = false)
    {
        var bgColor = alternate ? Colors.Grey.Lighten3 : Colors.White;
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
        double? iosChange, double? androidChange, bool includeLastPeriod, bool alternate = false, bool isPercent = false)
    {
        var bgColor = alternate ? Colors.Grey.Lighten3 : Colors.White;
        table.Cell().Background(bgColor).Padding(6).Text(metric);
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(iosCurrent) : FormatValue(iosCurrent));
        table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(androidCurrent) : FormatValue(androidCurrent));

        if (includeLastPeriod)
        {
            table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(iosLast) : FormatValue(iosLast));
            table.Cell().Background(bgColor).Padding(6).Text(isPercent ? FormatPercentage(androidLast) : FormatValue(androidLast));
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

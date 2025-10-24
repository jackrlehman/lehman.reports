namespace ReportBuilder.Models;

public class MobileAppReportConfig
{
    // Version
    public string Version { get; set; } = "1.01";

    // General Information
    public string CompanyName { get; set; } = "Your Company";
    public string ReportMonth { get; set; } = string.Empty;
    public int ReportDay { get; set; }
    public int ReportYear { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByTitle { get; set; } = string.Empty;

    // Last Report Date (Optional)
    public bool IncludeLastPeriodData { get; set; } = true;
    public string LastReportMonth { get; set; } = string.Empty;
    public int LastReportDay { get; set; }
    public int LastReportYear { get; set; }

    // Section Toggles
    public bool IncludeExecutiveSummary { get; set; } = true;
    public bool IncludeIOSSection { get; set; } = true;
    public bool IncludeAndroidSection { get; set; } = true;
    public bool IncludePlatformComparison { get; set; } = true;
    public bool IncludeTechnicalSpecifications { get; set; } = true;

    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;

    // iOS Platform Metrics
    public IOSMetrics IOSMetrics { get; set; } = new();

    // Android Platform Metrics
    public AndroidMetrics AndroidMetrics { get; set; } = new();

    // Platform Comparison
    public PlatformComparisonMetrics PlatformComparison { get; set; } = new();

    // Technical Specifications
    public string AppSize { get; set; } = string.Empty;

    // Method to auto-populate Platform Comparison from iOS and Android data
    public void SyncPlatformComparison()
    {
        // Total Downloads
        PlatformComparison.IOSTotalDownloads = IOSMetrics.TotalDownloads;
        PlatformComparison.AndroidTotalDownloads = AndroidMetrics.TotalInstalls;
        PlatformComparison.IOSTotalDownloadsLast = IOSMetrics.TotalDownloadsLast;
        PlatformComparison.AndroidTotalDownloadsLast = AndroidMetrics.TotalInstallsLast;

        // User % calculation (calculate as percentage of total across both platforms)
        var totalDownloads = (IOSMetrics.TotalDownloads ?? 0) + (AndroidMetrics.TotalInstalls ?? 0);
        if (totalDownloads > 0)
        {
            PlatformComparison.IOSUserPercent = Math.Round((IOSMetrics.TotalDownloads ?? 0) / totalDownloads * 100, 2);
            PlatformComparison.AndroidUserPercent = Math.Round((AndroidMetrics.TotalInstalls ?? 0) / totalDownloads * 100, 2);
        }

        var totalDownloadsLast = (IOSMetrics.TotalDownloadsLast ?? 0) + (AndroidMetrics.TotalInstallsLast ?? 0);
        if (totalDownloadsLast > 0)
        {
            PlatformComparison.IOSUserPercentLast = Math.Round((IOSMetrics.TotalDownloadsLast ?? 0) / totalDownloadsLast * 100, 2);
            PlatformComparison.AndroidUserPercentLast = Math.Round((AndroidMetrics.TotalInstallsLast ?? 0) / totalDownloadsLast * 100, 2);
        }

        // Daily Downloads
        PlatformComparison.IOSDailyDownloads = IOSMetrics.DailyDownloads;
        PlatformComparison.AndroidDailyDownloads = AndroidMetrics.DailyDownloads;
        PlatformComparison.IOSDailyDownloadsLast = IOSMetrics.DailyDownloadsLast;
        PlatformComparison.AndroidDailyDownloadsLast = AndroidMetrics.DailyDownloadsLast;

        // Crash Rate
        PlatformComparison.IOSCrashRate = IOSMetrics.CrashRatePerSession;
        PlatformComparison.AndroidCrashRate = AndroidMetrics.CrashRatePerSession;
        PlatformComparison.IOSCrashRateLast = IOSMetrics.CrashRatePerSessionLast;
        PlatformComparison.AndroidCrashRateLast = AndroidMetrics.CrashRatePerSessionLast;
    }
}

public class IOSMetrics
{
    // Key Metrics - Current
    public double? Impressions { get; set; }
    public double? ProductPageViews { get; set; }
    public double? ConversionRate { get; set; }
    public double? TotalDownloads { get; set; }
    public double? DailyDownloads { get; set; }
    public double? SessionsPerDevice { get; set; }
    public double? CrashRatePerSession { get; set; }
    public double? TotalCrashes { get; set; }

    // Key Metrics - Last Report (Optional)
    public double? ImpressionsLast { get; set; }
    public double? ProductPageViewsLast { get; set; }
    public double? ConversionRateLast { get; set; }
    public double? TotalDownloadsLast { get; set; }
    public double? DailyDownloadsLast { get; set; }
    public double? SessionsPerDeviceLast { get; set; }
    public double? CrashRatePerSessionLast { get; set; }
    public double? TotalCrashesLast { get; set; }

    // Computed % Change (Read-only)
    public double? ImpressionsChange => CalculatePercentChange(Impressions, ImpressionsLast);
    public double? ProductPageViewsChange => CalculatePercentChange(ProductPageViews, ProductPageViewsLast);
    public double? ConversionRateChange => CalculatePercentChange(ConversionRate, ConversionRateLast);
    public double? TotalDownloadsChange => CalculatePercentChange(TotalDownloads, TotalDownloadsLast);
    public double? DailyDownloadsChange => CalculatePercentChange(DailyDownloads, DailyDownloadsLast);
    public double? SessionsPerDeviceChange => CalculatePercentChange(SessionsPerDevice, SessionsPerDeviceLast);
    public double? CrashRatePerSessionChange => CalculatePercentChange(CrashRatePerSession, CrashRatePerSessionLast);
    public double? TotalCrashesChange => CalculatePercentChange(TotalCrashes, TotalCrashesLast);

    // Download Sources
    public List<DownloadSource> DownloadSources { get; set; } = new();

    // Method to auto-calculate download source percentages
    public void CalculateDownloadSourcePercentages()
    {
        // Calculate current percentages
        var totalCurrentDownloads = DownloadSources.Sum(s => s.CurrentDownloads ?? 0);
        if (totalCurrentDownloads > 0)
        {
            foreach (var source in DownloadSources)
            {
                if (source.CurrentDownloads.HasValue)
                {
                    source.CurrentPercentage = Math.Round((source.CurrentDownloads.Value / totalCurrentDownloads) * 100, 2);
                }
            }
        }

        // Calculate last period percentages
        var totalLastDownloads = DownloadSources.Sum(s => s.LastDownloads ?? 0);
        if (totalLastDownloads > 0)
        {
            foreach (var source in DownloadSources)
            {
                if (source.LastDownloads.HasValue)
                {
                    source.LastPercentage = Math.Round((source.LastDownloads.Value / totalLastDownloads) * 100, 2);
                }
            }
        }
    }

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}

public class DownloadSource
{
    public string Name { get; set; } = string.Empty;
    public double? CurrentPercentage { get; set; }
    public double? CurrentDownloads { get; set; }
    public double? LastPercentage { get; set; }
    public double? LastDownloads { get; set; }

    // Computed % Change
    public double? PercentageChange => CalculatePercentChange(CurrentPercentage, LastPercentage);
    public double? DownloadsChange => CalculatePercentChange(CurrentDownloads, LastDownloads);

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}

public class AndroidMetrics
{
    // Current Metrics
    public double? TotalInstalls { get; set; }
    public double? DailyDownloads { get; set; }
    public double? CrashRatePerSession { get; set; }

    // Last Report Metrics (Optional)
    public double? TotalInstallsLast { get; set; }
    public double? DailyDownloadsLast { get; set; }
    public double? CrashRatePerSessionLast { get; set; }

    // Computed % Change
    public double? TotalInstallsChange => CalculatePercentChange(TotalInstalls, TotalInstallsLast);
    public double? DailyDownloadsChange => CalculatePercentChange(DailyDownloads, DailyDownloadsLast);
    public double? CrashRatePerSessionChange => CalculatePercentChange(CrashRatePerSession, CrashRatePerSessionLast);

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}

public class PlatformComparisonMetrics
{
    // Total Downloads
    public double? IOSTotalDownloads { get; set; }
    public double? AndroidTotalDownloads { get; set; }
    public double? IOSTotalDownloadsLast { get; set; }
    public double? AndroidTotalDownloadsLast { get; set; }

    public double? IOSTotalDownloadsChange => CalculatePercentChange(IOSTotalDownloads, IOSTotalDownloadsLast);
    public double? AndroidTotalDownloadsChange => CalculatePercentChange(AndroidTotalDownloads, AndroidTotalDownloadsLast);

    // User % of all Apps
    public double? IOSUserPercent { get; set; }
    public double? AndroidUserPercent { get; set; }
    public double? IOSUserPercentLast { get; set; }
    public double? AndroidUserPercentLast { get; set; }

    public double? IOSUserPercentChange => CalculatePercentChange(IOSUserPercent, IOSUserPercentLast);
    public double? AndroidUserPercentChange => CalculatePercentChange(AndroidUserPercent, AndroidUserPercentLast);

    // Daily Downloads
    public double? IOSDailyDownloads { get; set; }
    public double? AndroidDailyDownloads { get; set; }
    public double? IOSDailyDownloadsLast { get; set; }
    public double? AndroidDailyDownloadsLast { get; set; }

    public double? IOSDailyDownloadsChange => CalculatePercentChange(IOSDailyDownloads, IOSDailyDownloadsLast);
    public double? AndroidDailyDownloadsChange => CalculatePercentChange(AndroidDailyDownloads, AndroidDailyDownloadsLast);

    // Crash Rate
    public double? IOSCrashRate { get; set; }
    public double? AndroidCrashRate { get; set; }
    public double? IOSCrashRateLast { get; set; }
    public double? AndroidCrashRateLast { get; set; }

    public double? IOSCrashRateChange => CalculatePercentChange(IOSCrashRate, IOSCrashRateLast);
    public double? AndroidCrashRateChange => CalculatePercentChange(AndroidCrashRate, AndroidCrashRateLast);

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}

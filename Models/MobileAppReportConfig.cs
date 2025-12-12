namespace ReportBuilder.Models;

public class MobileAppReportConfig
{
    // Version
    public string Version { get; set; } = "1.04";

    // General Information
    public string CompanyName { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonConverter(typeof(MonthJsonConverter))]
    public int? ReportMonth { get; set; }

    public int? ReportDay { get; set; }
    public int? ReportYear { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public string CreatedByTitle { get; set; } = string.Empty;

    // App Identifiers (for quick links to metrics)
    public string IOSAppIdentifier { get; set; } = string.Empty;
    public string AndroidAccountIdentifier { get; set; } = string.Empty;
    public string AndroidAppIdentifier { get; set; } = string.Empty;

    // Last Report Date (Optional)
    public bool IncludeLastPeriodData { get; set; } = true;

    [System.Text.Json.Serialization.JsonConverter(typeof(MonthJsonConverter))]
    public int? LastReportMonth { get; set; }

    public int? LastReportDay { get; set; }
    public int? LastReportYear { get; set; }

    // Helper method to format report date as MM/DD/YY
    public string GetFormattedReportDate()
    {
        if (ReportMonth.HasValue && ReportDay.HasValue && ReportYear.HasValue &&
            ReportMonth.Value > 0 && ReportDay.Value > 0 && ReportYear.Value > 0)
        {
            var year = ReportYear.Value % 100; // Get last 2 digits
            return $"{ReportMonth.Value}/{ReportDay.Value}/{year:00}";
        }
        return string.Empty;
    }

    // Helper method to format last report date as MM/DD/YY
    public string GetFormattedLastReportDate()
    {
        if (LastReportMonth.HasValue && LastReportDay.HasValue && LastReportYear.HasValue &&
            LastReportMonth.Value > 0 && LastReportDay.Value > 0 && LastReportYear.Value > 0)
        {
            var year = LastReportYear.Value % 100; // Get last 2 digits
            return $"{LastReportMonth.Value}/{LastReportDay.Value}/{year:00}";
        }
        return string.Empty;
    }

    // Section Toggles
    public bool IncludeExecutiveSummary { get; set; } = true;
    public bool IncludeIOSSection { get; set; } = true;
    public bool IncludeAndroidSection { get; set; } = true;
    public bool IncludePlatformComparison { get; set; } = true;
    public bool IncludeHighVarianceMetrics { get; set; } = true;
    public double HighVarianceThreshold { get; set; } = 50.0;
    
    // iOS Sub-section Toggles
    public bool IncludeIOSDailyActiveUsersByVersion { get; set; } = true;

    // Executive Summary
    public string ExecutiveSummary { get; set; } = string.Empty;

    // iOS Platform Metrics
    public IOSMetrics IOSMetrics { get; set; } = new();

    // Download Sources Display Options
    public bool ShowPercentBySource { get; set; } = true;

    // Android Platform Metrics
    public AndroidMetrics AndroidMetrics { get; set; } = new();

    // Platform Comparison
    public PlatformComparisonMetrics PlatformComparison { get; set; } = new();



    // Method to auto-populate Platform Comparison from iOS and Android data
    public void SyncPlatformComparison()
    {
        // Total Downloads
        PlatformComparison.IOSTotalDownloads = IOSMetrics.TotalDownloads;
        PlatformComparison.AndroidTotalDownloads = AndroidMetrics.TotalDownloads;
        PlatformComparison.IOSTotalDownloadsLast = IOSMetrics.TotalDownloadsLast;
        PlatformComparison.AndroidTotalDownloadsLast = AndroidMetrics.TotalDownloadsLast;

        // User % calculation (calculate as percentage of total across both platforms)
        var totalDownloads = (IOSMetrics.TotalDownloads ?? 0) + (AndroidMetrics.TotalDownloads ?? 0);
        if (totalDownloads > 0)
        {
            PlatformComparison.IOSUserPercent = Math.Round((IOSMetrics.TotalDownloads ?? 0) / totalDownloads * 100, 2);
            PlatformComparison.AndroidUserPercent = Math.Round((AndroidMetrics.TotalDownloads ?? 0) / totalDownloads * 100, 2);
        }

        var totalDownloadsLast = (IOSMetrics.TotalDownloadsLast ?? 0) + (AndroidMetrics.TotalDownloadsLast ?? 0);
        if (totalDownloadsLast > 0)
        {
            PlatformComparison.IOSUserPercentLast = Math.Round((IOSMetrics.TotalDownloadsLast ?? 0) / totalDownloadsLast * 100, 2);
            PlatformComparison.AndroidUserPercentLast = Math.Round((AndroidMetrics.TotalDownloadsLast ?? 0) / totalDownloadsLast * 100, 2);
        }

        // Daily Downloads
        PlatformComparison.IOSDailyDownloads = IOSMetrics.DailyDownloads;
        PlatformComparison.AndroidDailyDownloads = AndroidMetrics.DailyDownloads;
        PlatformComparison.IOSDailyDownloadsLast = IOSMetrics.DailyDownloadsLast;
        PlatformComparison.AndroidDailyDownloadsLast = AndroidMetrics.DailyDownloadsLast;

        // Total Crashes (iOS only)
        PlatformComparison.IOSTotalCrashes = IOSMetrics.TotalCrashes;
        PlatformComparison.IOSTotalCrashesLast = IOSMetrics.TotalCrashesLast;
    }
}

public class IOSMetrics
{
    // Key Metrics - Current
    public double? TotalDownloads { get; set; }
    public double? DailyDownloads { get; set; }
    public double? TotalCrashes { get; set; }
    public double? DevicesActiveWithin30Days { get; set; }
    public double? LifetimeDeletions { get; set; }
    public double? LifetimeReDownloads { get; set; }

    // Key Metrics - Last Report (Optional)
    public double? TotalDownloadsLast { get; set; }
    public double? DailyDownloadsLast { get; set; }
    public double? TotalCrashesLast { get; set; }
    public double? DevicesActiveWithin30DaysLast { get; set; }
    public double? LifetimeDeletionsLast { get; set; }
    public double? LifetimeReDownloadsLast { get; set; }

    // Computed % Change (Read-only)
    public double? TotalDownloadsChange => CalculatePercentChange(TotalDownloads, TotalDownloadsLast);
    public double? DailyDownloadsChange => CalculatePercentChange(DailyDownloads, DailyDownloadsLast);
    public double? TotalCrashesChange => CalculatePercentChange(TotalCrashes, TotalCrashesLast);
    public double? DevicesActiveWithin30DaysChange => CalculatePercentChange(DevicesActiveWithin30Days, DevicesActiveWithin30DaysLast);
    public double? LifetimeDeletionsChange => CalculatePercentChange(LifetimeDeletions, LifetimeDeletionsLast);
    public double? LifetimeReDownloadsChange => CalculatePercentChange(LifetimeReDownloads, LifetimeReDownloadsLast);

    // Daily Active Users broken down by individual app versions (optional breakdown of DevicesActiveWithin30Days)
    public List<VersionDAU> DailyActiveUsersByVersionBreakdown { get; set; } = new();
    
    // Daily Active Users by version breakdown - Last Report (Optional)
    public List<VersionDAU> DailyActiveUsersByVersionBreakdownLast { get; set; } = new();

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

public class VersionDAU
{
    public string VersionNumber { get; set; } = string.Empty;
    public double? DailyActiveUsers { get; set; }
}

public class AndroidMetrics
{
    // Current Metrics
    public double? TotalDownloads { get; set; }
    public double? DailyDownloads { get; set; }
    public double? CrashPercentPerSession { get; set; }

    // Last Report Metrics (Optional)
    public double? TotalDownloadsLast { get; set; }
    public double? DailyDownloadsLast { get; set; }
    public double? CrashPercentPerSessionLast { get; set; }

    // Computed % Change
    public double? TotalDownloadsChange => CalculatePercentChange(TotalDownloads, TotalDownloadsLast);
    public double? DailyDownloadsChange => CalculatePercentChange(DailyDownloads, DailyDownloadsLast);
    public double? CrashPercentPerSessionChange => CalculatePercentChange(CrashPercentPerSession, CrashPercentPerSessionLast);

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

    // User % of all Apps
    public double? IOSUserPercent { get; set; }
    public double? AndroidUserPercent { get; set; }
    public double? IOSUserPercentLast { get; set; }
    public double? AndroidUserPercentLast { get; set; }

    // Daily Downloads
    public double? IOSDailyDownloads { get; set; }
    public double? AndroidDailyDownloads { get; set; }
    public double? IOSDailyDownloadsLast { get; set; }
    public double? AndroidDailyDownloadsLast { get; set; }

    // Total Crashes (iOS only)
    public double? IOSTotalCrashes { get; set; }
    public double? IOSTotalCrashesLast { get; set; }

    private static double? CalculatePercentChange(double? current, double? last)
    {
        if (!current.HasValue || !last.HasValue || last.Value == 0)
            return null;

        return Math.Round(((current.Value - last.Value) / last.Value) * 100, 2);
    }
}


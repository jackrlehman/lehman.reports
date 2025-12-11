namespace ReportBuilder;

/// <summary>
/// Application-wide configuration constants.
/// </summary>
public static class AppConstants
{
    // Server Configuration
    public const string LocalhostUrl = "http://localhost:5000";
    public const int LocalhostPort = 5000;
    
    // File Upload Limits
    public const int MaxPdfFileSizeMB = 10;
    public const int MaxPdfFileSizeBytes = MaxPdfFileSizeMB * 1024 * 1024;
    
    // Report Names
    public const string MobileAppReportName = "Mobile App Store Performance Report";
    
    // Default Values
    public const string DefaultReportVersion = "1.04";
    
    // UI Messages
    public const string ErrorPdfLoadFailed = "Failed to load Blazor app. Make sure dotnet server is running on port 5000.";
    public const string ErrorPortInUse = "Port 5000 is already in use. Close other applications or change the port in Program.cs";
    
    // Time Formatting
    public const string DateFormatShort = "M/d/yy";
    public const string DateTimeFormat = "M/d/yy HH:mm";
}

/// <summary>
/// Logging event identifiers for structured logging.
/// </summary>
public static class LogEvents
{
    public const int AppStartup = 1000;
    public const int PdfGeneration = 2000;
    public const int PdfParsing = 2001;
    public const int FormValidation = 3000;
    public const int DataImport = 3001;
    public const int DataExport = 3002;
    public const int ServiceRegistration = 4000;
    public const int UnhandledException = 9000;
}

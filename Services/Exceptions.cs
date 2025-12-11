namespace ReportBuilder.Services;

/// <summary>
/// Thrown when PDF parsing or generation fails.
/// </summary>
public class PdfException : Exception
{
    public PdfException(string message) : base(message) { }
    public PdfException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when report configuration is invalid.
/// </summary>
public class InvalidReportConfigException : ArgumentException
{
    public InvalidReportConfigException(string message) : base(message) { }
    public InvalidReportConfigException(string message, string paramName) : base(message, paramName) { }
}

/// <summary>
/// Thrown when a required field is missing during report generation.
/// </summary>
public class MissingReportFieldException : InvalidReportConfigException
{
    public MissingReportFieldException(string fieldName)
        : base($"Required field '{fieldName}' is missing or invalid", fieldName) { }
}

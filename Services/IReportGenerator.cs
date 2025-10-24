namespace ReportBuilder.Services;

public interface IReportGenerator<TConfig>
{
    string ReportName { get; }
    byte[] GeneratePdf(TConfig config);
}

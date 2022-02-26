namespace Slox.Reporting;

public interface IOutputReporter
{
    void ReportResult(string result);
    void Print(string value);
}
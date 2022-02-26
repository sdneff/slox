namespace Slox.Reporting;

public class NullResultReporter : IResultReporter
{
    public void ReportResult(string result)
    {
        // no-op
    }
}
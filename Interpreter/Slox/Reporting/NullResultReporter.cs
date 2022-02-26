namespace Slox.Reporting;

public class NullOutputReporter : IOutputReporter
{
    public void ReportResult(string result)
    {
        // no-op
    }

    public void Print(string value)
    {
        // no-op
    }
}
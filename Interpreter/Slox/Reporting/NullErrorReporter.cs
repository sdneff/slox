namespace Slox.Reporting;

public class NullErrorReporter : IErrorReporter
{
    public void ReportError(int line, string message)
    {
        // no-op
    }
}
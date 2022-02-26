using Slox.Evaluation;
using Slox.Scanning;

namespace Slox.Reporting;

public class NullErrorReporter : IErrorReporter
{
    public void ReportError(int line, string message)
    {
        // no-op
    }

    public void ReportError(Token token, string message)
    {
        // no-op
    }

    public void ReportError(RuntimeError error)
    {
        // no-op
    }
}

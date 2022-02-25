using Slox.Scanning;

namespace Slox.Reporting;

public interface IErrorReporter
{
    void ReportError(int line, string message);
    void ReportError(Token token, string message);
}
namespace Slox.Reporting;

public interface IErrorReporter
{
    void ReportError(int line, string message);
}
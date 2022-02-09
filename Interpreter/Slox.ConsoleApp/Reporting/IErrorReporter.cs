namespace Slox.ConsoleApp.Reporting;

public interface IErrorReporter
{
    void ReportError(int line, int offset, string message);
}
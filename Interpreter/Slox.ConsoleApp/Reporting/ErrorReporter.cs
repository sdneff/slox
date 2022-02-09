namespace Slox.ConsoleApp.Reporting;

public class ErrorReporter : IErrorReporter
{
    public void ReportError(int line, int offset, string message)
    {
        Console.WriteLine($"[line {line}] Error : {message}.");
    }
}
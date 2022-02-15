using Slox.Reporting;

namespace Slox.ConsoleApp.Reporting;

public class SimpleErrorReporter : IErrorReporter
{
    public void ReportError(int line, string message)
    {
        Console.WriteLine($"[line {line}] Error : {message}.");
    }
}
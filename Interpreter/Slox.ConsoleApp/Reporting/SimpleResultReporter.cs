using Slox.Reporting;

namespace Slox.ConsoleApp.Reporting;

public class SimpleResultReporter : IResultReporter
{
    public void ReportResult(string result)
    {
        Console.WriteLine($"RESULT: {result}");
    }
}
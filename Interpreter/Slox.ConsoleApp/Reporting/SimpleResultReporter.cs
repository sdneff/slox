using Slox.Reporting;

namespace Slox.ConsoleApp.Reporting;

public class SimpleOutputReporter : IOutputReporter
{
    public void ReportResult(string result)
    {
        Console.WriteLine($"RESULT: {result}");
    }

    public void Print(string value)
    {
        Console.WriteLine(value);
    }
}
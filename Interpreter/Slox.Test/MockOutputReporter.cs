using System.Collections.Generic;
using Slox.Reporting;

public class MockOutputReporter : IOutputReporter
{
    private readonly List<string> _results = new();
    private readonly List<string> _print = new();

    public IReadOnlyList<string> Results => _results;
    public IReadOnlyList<string> PrintLines => _print;

    public void ReportResult(string result) => _results.Add(result);

    public void Print(string value) => _print.Add(value);
}
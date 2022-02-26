using Slox.Reporting;

namespace Slox;

public static class Slox
{
    public static IErrorReporter Error { get; set; } = new NullErrorReporter();
    public static IResultReporter Result { get; set; } = new NullResultReporter();
}
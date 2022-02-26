using Slox.Evaluation;
using Slox.Reporting;
using Slox.Scanning;

namespace Slox.ConsoleApp.Reporting;

public class SimpleErrorReporter : IErrorReporter
{
    public void ReportError(int line, string message)
    {
        Console.WriteLine($"[line {line}] Error : {message}.");
    }

    public void ReportError(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            ReportError(token.Line, $"at end. {message}");
        }
        else
        {
            ReportError(token.Line, $"at '{token.Lexeme}'. {message}");
        }
    }

    public void ReportError(RuntimeError error)
    {
        ReportError(error.Token.Line, error.Message);
    }
}
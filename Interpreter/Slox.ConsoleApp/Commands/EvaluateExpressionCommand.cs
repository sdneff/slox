using Slox.Evaluation;
using Slox.Parsing;
using Slox.Scanning;

namespace Slox.ConsoleApp.Commands;

public class EvaluateExpressionCommand : ICommand
{
    public void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expr = parser.ParseExpression();

        if (expr != null)
        {
            // evaluate
            var interpreter = new Interpreter();
            interpreter.Interpret(expr);
        }
    }
}

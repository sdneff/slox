using Slox.Parsing;
using Slox.Scanning;
using Slox.Syntax;

namespace Slox.ConsoleApp.Commands;

public class ParseTreeCommand : ICommand
{
    public string Name => "parse";

    public void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        var parser = new Parser(tokens);
        var expr = parser.Parse();

        // print
        if (expr != null)
        {
            var printer = new SExpressionAstPrinter();
            Console.WriteLine(printer.Print(expr));
        }
    }
}

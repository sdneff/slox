using Slox.Parsing;
using Slox.Scanning;
using Slox.Syntax;

namespace Slox.ConsoleApp.Commands;

public class ParseTreeCommand : ICommand
{
    private readonly Type _type;

    public ParseTreeCommand(Type type = Type.Program)
    {
        _type = type;
    }

    public void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var printer = new SExpressionAstPrinter();

        try
        {
            switch (_type)
            {
                case Type.Expression:
                    var expr = parser.ParseExpression();

                    if (expr != null)
                    {
                        Console.WriteLine(printer.Print(expr));
                    }
                    break;
                case Type.Statement:
                    var stmt = parser.Parse();

                    if (stmt.Any())
                    {
                        Console.WriteLine(printer.Print(stmt.Last()));
                    }
                    break;
                case Type.Program:
                    var prog = parser.Parse();

                    foreach (var st in prog)
                    {
                        Console.WriteLine(printer.Print(st));
                    }
                    break;
            }
        }
        catch (ParseError)
        {
            Console.WriteLine("PARSE ERROR");
        }
    }

    public enum Type
    {
        Expression,
        Statement,
        Program
    }
}

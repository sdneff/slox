using Slox.ConsoleApp.Commands;
using Slox.ConsoleApp.Interactive;
using Slox.ConsoleApp.Reporting;
using Slox.Evaluation;
using Slox.Parsing;
using Slox.Scanning;

namespace Slox.ConsoleApp;

class Program
{
    internal static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: slox [script]");
        }
        
        // set up environment
        Slox.Error = new SimpleErrorReporter();
        Slox.Out = new SimpleOutputReporter();

        if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        var interpreter = new Interpreter();

        var source = File.ReadAllText(path);
        InterpretSource(source, interpreter);
    }

    private static void RunPrompt()
    {
        var interpreter = new Interpreter();

        Repl.Run(source => {
            if (CommandReader.IsQuitCommand(source)) {
                return false;
            } else if (CommandReader.HandleInput(source, interpreter.Environment)) {
                return true;
            } else {
                InterpretSource(source, interpreter);
                return true;
            }
        });
    }

    private static void InterpretSource(string source, Interpreter interpreter)
    {
        try
        {
            var scanner = new Scanner(source);
            var tokens = scanner.ScanTokens();

            var parser = new Parser(tokens);
            var statements = parser.Parse();

            interpreter.Interpret(statements);
        }
        catch (ParseError)
        {
            Console.WriteLine("PARSE ERROR");
        }
        catch (RuntimeError)
        {
            Console.WriteLine("RUNTIME ERROR");
        }
    }
}

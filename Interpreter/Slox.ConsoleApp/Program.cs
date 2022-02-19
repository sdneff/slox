using Slox.Scanning;
using Slox.ConsoleApp.Reporting;
using System.Text;

namespace Slox.ConsoleApp;

class Program
{
    internal static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: slox [script]");
        }
        else if (args.Length == 1)
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
        var source = File.ReadAllText(path);
        Run(source);
    }

    private static void RunPrompt()
    {
        Console.WriteLine("slox interactive:");
        Console.WriteLine();

        var sb = new StringBuilder();

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
            else if (line.EndsWith(@"\"))
            {
                sb.AppendLine(line.Substring(0, line.Length - 1));
            }
            else
            {
                sb.AppendLine(line);
                Run(sb.ToString());
                sb.Clear();
            }
        }

        Console.WriteLine();
        Console.WriteLine("bye!");
    }

    private static void Run(string source)
    {
        // set up environment
        Slox.Error = new SimpleErrorReporter();

        Console.WriteLine($"running source...");

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}


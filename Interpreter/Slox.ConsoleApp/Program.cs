using Slox;

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

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == null) break;
            Run(line);
        }

        Console.WriteLine();
        Console.WriteLine("bye!");
    }

    private static void Run(string source)
    {
        Console.WriteLine($"running source...");
        Console.WriteLine(source);
    }
}


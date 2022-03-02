using System.Text;

namespace Slox.ConsoleApp.Interactive;

public static class Repl
{
    public static void Run(Func<string, bool> inputHandler)
    {
        Console.WriteLine("slox interactive:");
        Console.WriteLine();

        var sb = new StringBuilder();
        var multiline = false;

        while (true)
        {
            Console.Write(multiline ? "  " : "> ");
            var line = Console.ReadLine();
            if (line == null)
            {
                break;
            }
            else if (line.EndsWith(@"\"))
            {
                multiline = true;
                sb.AppendLine(line.Substring(0, line.Length - 1));
            }
            else
            {
                if (multiline) {
                    sb.AppendLine(line);
                    if (!inputHandler(sb.ToString())) break;
                    multiline = false;
                    sb.Clear();
                } else {
                    if (!inputHandler(line)) break;
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("bye!");
    }
}
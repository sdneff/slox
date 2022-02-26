using Slox.Scanning;

namespace Slox.ConsoleApp.Commands;

public class TokenizeCommand : ICommand
{
    public void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}

using static Slox.Evaluation.Values;
using Environment = Slox.Evaluation.Environment;

namespace Slox.ConsoleApp.Commands;

public class EnvironmentCommand : ICommand
{
    public void Run(string source) => Run(source, new Environment());

    public void Run(string source, Environment environment)
    {
        var count = environment.DefinedVariables.Count();
        var charLength = count > 0
            ? environment.DefinedVariables.Select(s => s.Length).Max()
            : 0;

        Console.WriteLine($"Current environment (depth={environment.Depth}): variable count={count}.");
        foreach (var @var in environment.DefinedVariables.OrderBy(s => s))
        {
            var value = Stringify(environment.GetValue(@var));
            Console.WriteLine($"{@var.PadRight(charLength)} : {value}");
        }
    }
}

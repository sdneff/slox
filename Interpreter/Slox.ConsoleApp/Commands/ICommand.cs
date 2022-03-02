using Environment = Slox.Evaluation.Environment;

namespace Slox.ConsoleApp.Commands;

public interface ICommand
{
    void Run(string source);
    void Run(string source, Environment environment) => Run(source);
}

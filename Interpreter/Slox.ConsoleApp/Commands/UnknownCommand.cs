namespace Slox.ConsoleApp.Commands;

public class UnknownCommand : ICommand
{
    private readonly string _name;

    public UnknownCommand(string name)
    {
        _name = name;
    }

    public void Run(string source)
    {
        Console.WriteLine($"Unrecognized command: {_name}");
    }
}
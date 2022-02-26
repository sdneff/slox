namespace Slox.ConsoleApp.Commands;

public static class CommandReader
{
    public static bool HandleInput(string sourceIn)
    {
        var (cmd, sourceOut) = ParseCommand(sourceIn);
        if (cmd == null) return false;
        
        ICommand command = cmd switch
        {
            "token" => new TokenizeCommand(),
            "parse" => new ParseTreeCommand(),
            _ => new UnknownCommand(cmd)
        };

        command.Run(sourceOut);
        return true;
    }

    private static (string?, string) ParseCommand(string source)
    {
        if (source.StartsWith(":"))
        {
            var i = 1;
            while (i < source.Length && !char.IsWhiteSpace(source[i])) ++i;

            return (source.Substring(1, i - 1), source.Substring(i).TrimStart());
        }
        else
        {
            return (null, source);
        }
    }
}
using Environment = Slox.Evaluation.Environment;

namespace Slox.ConsoleApp.Commands;

public static class CommandReader
{
    public static bool IsQuitCommand(string sourceIn)
    {
        var (cmd, _) = ParseCommand(sourceIn);
        return cmd switch
        {
            "quit" => true,
            "q" => true,
            _ => false
        };
    }

    public static bool HandleInput(string sourceIn, Environment environment)
    {
        var (cmd, sourceOut) = ParseCommand(sourceIn);
        if (cmd == null) return false;
        
        ICommand command = cmd switch
        {
            "token" => new TokenizeCommand(),
            "parse-e" => new ParseTreeCommand(ParseTreeCommand.Type.Expression),
            "parse-s" => new ParseTreeCommand(ParseTreeCommand.Type.Statement),
            "parse" => new ParseTreeCommand(),
            "eval"  => new EvaluateExpressionCommand(),
            "env" => new EnvironmentCommand(),
            _ => new UnknownCommand(cmd)
        };

        command.Run(sourceOut, environment);
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
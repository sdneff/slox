using Slox.Syntax;

namespace Slox.Evaluation;

public class Function : ICallable
{
    private readonly Stmt.Function _declaration;

    public Function(Stmt.Function declaration)
    {
        _declaration = declaration;
    }

    public int Arity => _declaration.Params.Count;

    public object? Call(Interpreter interpreter, IList<object?> arguments)
    {
        var environment = new Environment(interpreter.Globals); // todo: proper closure

        foreach (var (token, val) in _declaration.Params.Zip(arguments))
        {
            environment.Define(token.Lexeme, val);
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (Return returnVal)
        {
            return returnVal.Value;
        }

        return null;
    }
}
using Slox.Syntax;

namespace Slox.Evaluation;

public class Function : ICallable
{
    private readonly Stmt.Function _declaration;
    private readonly Environment _closure;

    public Function(Stmt.Function declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public int Arity => _declaration.Params.Count;

    public object? Call(Interpreter interpreter, IList<object?> arguments)
    {
        var environment = new Environment(_closure);

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
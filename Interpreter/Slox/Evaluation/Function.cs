using Slox.Syntax;

namespace Slox.Evaluation;

public class Function : ICallable
{
    private readonly string? _name;
    private readonly Expr.Function _function;
    private readonly Environment _closure;

    public Function(string? name, Expr.Function function, Environment closure)
    {
        _name = name;
        _function = function;
        _closure = closure;
    }

    public int Arity => _function.Params.Count;

    public object? Call(Interpreter interpreter, IList<object?> arguments)
    {
        var environment = new Environment(_closure);

        foreach (var (token, val) in _function.Params.Zip(arguments))
        {
            environment.Define(token.Lexeme, val);
        }

        try
        {
            interpreter.ExecuteBlock(_function.Body, environment);
        }
        catch (Return returnVal)
        {
            return returnVal.Value;
        }

        return null;
    }

    public override string ToString() => _name is null ? "<function>" : $"<function {_name}>";
}
namespace Slox.Evaluation;

public interface ICallable
{
    int Arity { get; }
    object? Call(Interpreter interpreter, IList<object?> arguments);
}
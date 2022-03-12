namespace Slox.Evaluation;

public static class NativeFunctions
{
    public static void AddTo(Environment globals)
    {
        globals.Define("clock", new Clock());
    }

    private abstract class NativeFunction : ICallable
    {
        public abstract int Arity { get; }

        public abstract object? Call(Interpreter interpreter, IList<object?> arguments);

        public override string ToString() => "<native function>";
    }

    private class Clock : NativeFunction
    {
        public override int Arity => 0;
        
        public override object? Call(Interpreter interpreter, IList<object?> arguments)
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1_000.0;
        }
    }
}


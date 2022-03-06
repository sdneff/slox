namespace Slox.Evaluation;

public static class NativeFunctions
{
    public static void AddTo(Environment globals)
    {
        globals.Define("clock", new Clock());
    }

    private class Clock : ICallable
    {
        public int Arity => 0;
        
        public object? Call(Interpreter interpreter, IList<object?> arguments)
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1_000.0;
        }
    }
}


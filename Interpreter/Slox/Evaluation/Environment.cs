using Slox.Scanning;

namespace Slox.Evaluation;

public class Environment
{
    private readonly Dictionary<string, object?> _values = new();

    public void Define(string name, object? value) => _values[name] = value;

    public IEnumerable<string> DefinedVariables => _values.Keys;

    public object? Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public object? GetValue(string name) => _values.GetValueOrDefault(name);
}
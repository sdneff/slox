using Slox.Scanning;

namespace Slox.Evaluation;

public class Environment
{
    private readonly Environment? _enclosing;
    private readonly Dictionary<string, object?> _values = new();

    public int Depth => _enclosing?.Depth + 1 ?? 0;

    public void Define(string name, object? value) => _values[name] = value;

    public IEnumerable<string> DefinedVariables => _values.Keys
        .Concat(_enclosing?.DefinedVariables ?? Enumerable.Empty<string>())
        .Distinct();

    public Environment()
    {
        _enclosing = null;
    }

    public Environment(Environment enclosing)
    {
        _enclosing = enclosing ?? throw new ArgumentNullException(nameof(enclosing));
    }

    public object? Get(Token name)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            return _values[name.Lexeme];
        }
        else if (_enclosing != null)
        {
            return _enclosing.Get(name);
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        else if (_enclosing != null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void AssignAt(int distance, Token token, object? value)
    {
        var environment = Ancestor(distance) ?? throw new RuntimeError(token, "Invalid environment resolution depth.");

        environment._values[token.Lexeme] = value;
    }

    public object? GetValue(string name) => _values.ContainsKey(name)
            ? _values.GetValueOrDefault(name)
            : _enclosing?.GetValue(name);

    public object? GetAt(int distance, Token name)
    {
        var environment = Ancestor(distance) ?? throw new RuntimeError(name, "Invalid environment resolution depth.");

        return environment._values[name.Lexeme];
    }

    private Environment? Ancestor(int distance)
    {
        var environment = this;
        for (var i = 0; i < distance; ++i)
        {
            environment = environment?._enclosing;
        }

        return environment;
    }
}
namespace Slox.Evaluation;

public static class Values
{
    public static bool IsTruthy(object? obj) => obj is bool b
        ? b
        : obj is not null;

    public static bool IsEqual(object? obj1, object? obj2) => obj1 is null
        ? obj2 is null
        : obj1.Equals(obj2);

    public static string Stringify(object? obj, bool quoteStrings = false)
    {
        if (obj is null) return "nil";

        var s = obj.ToString()!;
        
        if (obj is double && s.EndsWith(".0"))
        {
            return s.Substring(0, s.Length - 2);
        }

        if (quoteStrings && obj is string)
        {
            s = @$"""{s}""";
        }

        return s;
    }

    public static readonly Unit unit = Unit.Unit;
    public enum Unit
    {
        Unit
    }
}
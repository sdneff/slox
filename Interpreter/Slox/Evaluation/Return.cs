namespace Slox.Evaluation;

public class Return : RuntimeError
{
    public readonly object? Value;

    public Return(object? value)
        : base(null!, null!)
    {
        Value = value;
    }
}

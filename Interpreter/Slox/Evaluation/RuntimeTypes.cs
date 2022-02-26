using Slox.Scanning;

namespace Slox.Evaluation;

public static class RuntimeTypes
{
    public static double CheckNumberOperand(Token @operator, object? operand)
    {
        if (operand is double op)
        {
            return op;
        }
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    public static (double, double) CheckNumberOperands(Token @operator, object? left, object? right)
    {
        if (left is double l && right is double r)
        {
            return (l, r);
        }
        throw new RuntimeError(@operator, "Operands must be a numbers.");
    }
}

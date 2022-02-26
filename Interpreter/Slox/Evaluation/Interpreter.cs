using Slox.Syntax;
using static Slox.Scanning.TokenType;
using static Slox.Evaluation.ValueOps;
using static Slox.Evaluation.RuntimeTypes;

namespace Slox.Evaluation;

public class Interpreter : Expr.IVisitor<object?>
{
    public void Interpret(Expr expression)
    {
        try
        {
            var value = Evaluate(expression);
            Slox.Result.ReportResult(Stringify(value));
        }
        catch (RuntimeError err)
        {
            Slox.Error.ReportError(err);
        }
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case Plus when left is string lStr && right is string rStr:
                return lStr + rStr;
            case BangEqual:
                return !IsEqual(left, right);
            case EqualEqual:
                return IsEqual(left, right);
            default:
                var (l, r) = CheckNumberOperands(expr.Operator, left, right);
                return expr.Operator.Type switch
                {
                    Minus => l - r,
                    Plus => l + r,
                    Slash => l / r,
                    Star => l * r,
                    Greater => l > r,
                    GreaterEqual => l >= r,
                    Less => l < r,
                    LessEqual => l <= r,
                    _ => null as object
                };
        }
    }

    public object? VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.Expression);

    public object? VisitLiteralExpr(Expr.Literal expr) => expr.Value;

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        var right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case Bang:
                return !IsTruthy(right);
            case Minus:
                var r = CheckNumberOperand(expr.Operator, right);
                return -1 * r;
            default:
                return null as object; // unreachable
        };
    }

    private object? Evaluate(Expr expr) => expr.Accept(this);
}
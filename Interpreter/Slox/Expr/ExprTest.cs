namespace Slox.Expr;

public abstract record ExprTest
{
    public record RealExpr(string Left, string Right) : ExprTest();
}

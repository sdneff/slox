namespace Slox.Syntax;

public class SExpressionAstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>, IAstPrinter
{
    public string Print(Stmt stmt) => stmt.Accept(this);
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitExpressionStmt(Stmt.Expression stmt) => Print(stmt.Expr);

    public string VisitPrintStmt(Stmt.Print stmt) => Parenthesize("print", stmt.Expr);

    public string VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    public string VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.Expression);

    public string VisitLiteralExpr(Expr.Literal expr) => expr.Value?.ToString() ?? "nil";

    public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.Operator.Lexeme, expr.Right);

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var eVals = exprs.Select(e => " " + e.Accept(this));

        return $"({name}{string.Concat(eVals)})";
    }
}
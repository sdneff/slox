namespace Slox.Syntax;

public class SExpressionAstPrinter : Expr.IVisitor<string>, Stmt.IVisitor<string>, IAstPrinter
{
    public string Print(Stmt stmt) => stmt.Accept(this);
    public string Print(Expr expr) => expr.Accept(this);

    public string VisitBlockStmt(Stmt.Block stmt) => Lines(stmt.Statements.Select(Print).Prepend("{").Append("}"));

    public string VisitVarStmt(Stmt.Var stmt) => stmt.Initializer ==  null
        ? Parenthesize("declare " + stmt.Name.Lexeme)
        : Parenthesize("let " + stmt.Name.Lexeme, stmt.Initializer);
 
    public string VisitExpressionStmt(Stmt.Expression stmt) => Print(stmt.Expr);

    public string VisitIfStmt(Stmt.If stmt) => stmt.ElseBranch is null
        ? Parenthesize("if", Print(stmt.Condition), Print(stmt.ThenBranch))
        : Parenthesize("if", Print(stmt.Condition), Print(stmt.ThenBranch), Print(stmt.ElseBranch));

    public string VisitPrintStmt(Stmt.Print stmt) => Parenthesize("print", stmt.Expr);

    public string VisitWhileStmt(Stmt.While stmt) => Parenthesize("while", Print(stmt.Condition), Print(stmt.Body));

    public string VisitAssignExpr(Expr.Assign expr) => Parenthesize("set " + expr.Name.Lexeme, expr.Value);

    public string VisitBinaryExpr(Expr.Binary expr) => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    
    public string VisitGroupingExpr(Expr.Grouping expr) => Parenthesize("group", expr.Expression);

    public string VisitLiteralExpr(Expr.Literal expr) => expr.Value?.ToString() ?? "nil";

    public string VisitLogicalExpr(Expr.Logical expr) => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

    public string VisitUnaryExpr(Expr.Unary expr) => Parenthesize(expr.Operator.Lexeme, expr.Right);

    public string VisitVariableExpr(Expr.Variable expr) => expr.Name.ToString();

    private string Parenthesize(string name, params Expr[] exprs)
    {
        var eVals = exprs.Select(e => e.Accept(this));

        return Parenthesize(eVals.Prepend(name));
    }

    private string Parenthesize(IEnumerable<string> vals) => $"({string.Join(" ", vals)})";
    private string Parenthesize(params string[] vals) => Parenthesize(vals.AsEnumerable());

    private string Lines(IEnumerable<string> lines)
    {
        return string.Join(Environment.NewLine, lines);
    }
}
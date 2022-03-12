namespace Slox.Syntax;

public class ReversePolishNotationAstPrinter : IAstPrinter
{
    public string Print(Expr expr) => string.Join(" ", expr.Accept(new RPNVisitor()));

    private class RPNVisitor : Expr.IVisitor<IEnumerable<string>>
    {
        public IEnumerable<string> VisitAssignExpr(Expr.Assign expr) => AsStack(":= " + expr.Name.Lexeme, expr.Value);

        public IEnumerable<string> VisitBinaryExpr(Expr.Binary expr) => AsStack(expr.Operator.Lexeme, expr.Left, expr.Right);

        public IEnumerable<string> VisitCallExpr(Expr.Call expr) => AsStack(string.Concat(expr.Callee.Accept(this)), expr.Arguments.ToArray());

        public IEnumerable<string> VisitFunctionExpr(Expr.Function expr) => 
            new [] { "(", "lambda" }
                .Concat(expr.Params.Select(p => p.Lexeme))
                .Append(")")
                .Append("<body>")
                .Append(")");

        public IEnumerable<string> VisitGroupingExpr(Expr.Grouping expr) => AsStack("group", expr.Expression);

        public IEnumerable<string> VisitLiteralExpr(Expr.Literal expr)
        { 
            yield return expr.Value?.ToString() ?? "nil";
        }

        public IEnumerable<string> VisitLogicalExpr(Expr.Logical expr) => AsStack(expr.Operator.Lexeme, expr.Left, expr.Right);

        public IEnumerable<string> VisitUnaryExpr(Expr.Unary expr) => AsStack(expr.Operator.Lexeme, expr.Right);

        public IEnumerable<string> VisitVariableExpr(Expr.Variable expr)
        {
            yield return expr.Name.ToString();
        }

        private IEnumerable<string> AsStack(string name, params Expr[] exprs) => exprs.SelectMany(e => e.Accept(this)).Append(name);
    }
}
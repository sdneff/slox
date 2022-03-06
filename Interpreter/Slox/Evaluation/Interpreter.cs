using Slox.Syntax;
using static Slox.Scanning.TokenType;
using static Slox.Evaluation.Values;
using static Slox.Evaluation.RuntimeTypes;

namespace Slox.Evaluation;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<Unit>
{
    public readonly Environment Globals = new();
    public Environment Environment { get; private set; }

    public Interpreter()
    {
        Environment = Globals;
        NativeFunctions.AddTo(Globals);
    }

    public void Interpret(IEnumerable<Stmt> statements)
    {
        try
        {
            foreach (var stmt in statements)
            {
                Execute(stmt);
            }
        }
        catch (RuntimeError err)
        {
            Slox.Error.ReportError(err);
        }
    }

    public void Interpret(Expr expression)
    {
        try
        {
            var value = Evaluate(expression);
            Slox.Out.ReportResult(Stringify(value, true));
        }
        catch (RuntimeError err)
        {
            Slox.Error.ReportError(err);
        }
    }

    public Unit VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(Environment));

        return unit;
    }

    public Unit VisitVarStmt(Stmt.Var stmt)
    {
        Environment.Define(
            stmt.Name.Lexeme,
            stmt.Initializer != null
                ? Evaluate(stmt.Initializer)
                : null as object);

        return unit;
    }

    public Unit VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return unit;
    }

    public Unit VisitFunctionStmt(Stmt.Function stmt)
    {
        Environment.Define(stmt.Name.Lexeme, new Function(stmt));
        return unit;
    }

    public Unit VisitIfStmt(Stmt.If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.ThenBranch);
        } else if (stmt.ElseBranch != null) {
            Execute(stmt.ElseBranch);
        }
        return unit;
    }

    public Unit VisitPrintStmt(Stmt.Print stmt)
    {
        var value = Evaluate(stmt.Expr);
        Slox.Out.Print(Stringify(value));
        return unit;
    }

    public Unit VisitWhileStmt(Stmt.While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition))) {
            Execute(stmt.Body);
        }
        return unit;
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        var value = Evaluate(expr.Value);
        Environment.Assign(expr.Name, value);
        return value;
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

    public object? VisitCallExpr(Expr.Call expr)
    {
        var callee = Evaluate(expr.Callee);

        if (callee is ICallable function)
        {
            var arguments = expr.Arguments.Select(Evaluate).ToList();

            if (arguments.Count != function.Arity)
            {
                throw new RuntimeError(expr.Paren,
                    $"Expected {function.Arity} arguments but got {arguments.Count}.");
            }

            return function.Call(this, arguments);
        }
        else
        {
            throw new RuntimeError(expr.Paren, "Can only call functions and classes.");
        }
    }

    public object? VisitGroupingExpr(Expr.Grouping expr) => Evaluate(expr.Expression);

    public object? VisitLiteralExpr(Expr.Literal expr) => expr.Value;

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        var left = Evaluate(expr.Left);

        return expr.Operator.Type switch
        {
            Or when IsTruthy(left) => left,
            And when !IsTruthy(left) => left,
            _ => Evaluate(expr.Right)
        };
    }

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

    public object? VisitVariableExpr(Expr.Variable expr) => Environment.Get(expr.Name);

    public void ExecuteBlock(IEnumerable<Stmt> statements, Environment environment)
    {
        var outer = Environment;
        try
        {
            Environment = environment;

            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            Environment = outer;
        }
    }

    private object? Evaluate(Expr expr) => expr.Accept(this);
    private void Execute(Stmt stmt) => stmt.Accept(this);
}
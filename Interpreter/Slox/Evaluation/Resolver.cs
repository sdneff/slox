using Slox.Scanning;
using Slox.Syntax;
using static Slox.Evaluation.Values;

namespace Slox.Evaluation;

public class Resolver : Expr.IVisitor<Unit>, Stmt.IVisitor<Unit>
{
    private FunctionType _currentFunction = FunctionType.None;
    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new();

    public Resolver(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public void Resolve(IEnumerable<Stmt> statements) {
        foreach (var stmt in statements) {
            Resolve(stmt);
        }
    }

    public Unit VisitBlockStmt(Stmt.Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return unit;
    }

    public Unit VisitExpressionStmt(Stmt.Expression stmt)
    {
        Resolve(stmt.Expr);
        return unit;
    }

    public Unit VisitFunctionStmt(Stmt.Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        ResolveFunction(stmt.Func, FunctionType.Function);
        return unit;
    }   
    
    public Unit VisitIfStmt(Stmt.If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null) Resolve(stmt.ElseBranch);
        return unit;
    }

    public Unit VisitPrintStmt(Stmt.Print stmt)
    {
        Resolve(stmt.Expr);
        return unit;
    }

    public Unit VisitReturnStmt(Stmt.Return stmt)
    {
        if (_currentFunction == FunctionType.None)
        {
            throw new ResolutionError(stmt.Keyword, "Can't return from top-level code.");
        }

        if (stmt.Value != null) Resolve(stmt.Value);
        return unit;
    }

    public Unit VisitVarStmt(Stmt.Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        Define(stmt.Name);
        return unit;
    }

    public Unit VisitWhileStmt(Stmt.While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return unit;
    }

    public Unit VisitAssignExpr(Expr.Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return unit;
    }

    public Unit VisitBinaryExpr(Expr.Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return unit;
    }

    public Unit VisitCallExpr(Expr.Call expr)
    {
        Resolve(expr.Callee);

        foreach (var arg in expr.Arguments)
        {
            Resolve(arg);
        }

        return unit;
    }

    public Unit VisitFunctionExpr(Expr.Function expr)
    {
        ResolveFunction(expr, FunctionType.Function);
        return unit;
    }

    public Unit VisitGroupingExpr(Expr.Grouping expr)
    {
        Resolve(expr.Expression);
        return unit;
    }

    public Unit VisitLiteralExpr(Expr.Literal expr) => unit;

    public Unit VisitLogicalExpr(Expr.Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return unit;
    }

    public Unit VisitUnaryExpr(Expr.Unary expr)
    {
        Resolve(expr.Right);
        return unit;
    }

    public Unit VisitVariableExpr(Expr.Variable expr)
    {
        if (_scopes.Any() && _scopes.Peek().ContainsKey(expr.Name.Lexeme) && !_scopes.Peek()[expr.Name.Lexeme])
        {
            throw new ResolutionError(expr.Name, "Can't read local variable in its own initializer.");
        }

        ResolveLocal(expr, expr.Name);
        return unit;
    }

    private void BeginScope() => _scopes.Push(new());
    private void EndScope() => _scopes.Pop();

    private void Declare(Token name)
    {
        if (_scopes.Any())
        {
            var scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                throw new ResolutionError(name, "Already a variable with this name in this scope.");
            }
            scope[name.Lexeme] = false;
        }
    }
    private void Define(Token name)
    {
        if (_scopes.Any())
        {
            _scopes.Peek()[name.Lexeme] = true;
        }
    }

    private void Resolve(Stmt stmt) => stmt.Accept(this);
    private void Resolve(Expr expr) => expr.Accept(this);
    private void ResolveFunction(Expr.Function function, FunctionType type)
    {
        var enclosingFunction = _currentFunction;
        _currentFunction = type;
        BeginScope();
        foreach (var param in function.Params)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();
        _currentFunction = enclosingFunction;
    }
    private void ResolveLocal(Expr expr, Token name)
    {
        var depth = _scopes.Select((scope, depth) => new { scope, depth })
            .FirstOrDefault(s => s.scope.ContainsKey(name.Lexeme))
            ?.depth;

        if (depth.HasValue)
        {
            _interpreter.Resolve(expr, depth.Value);
        }
    }

    private enum FunctionType
    {
        None,
        Function
    }
}
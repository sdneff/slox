using Slox.Parsing;
using Slox.Scanning;
using Slox.Syntax;
using Xunit;

namespace Slox.Test;

public class ParserTest
{
    [Fact]
    public void TestExpression()
    {
        var source = "t = (13 + 4) - myFun(true, x < y and x >= -z)";

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);

        var expr = parser.ParseExpression();

        Assert.NotNull(expr);
        Assert.IsType<Expr.Assign>(expr);

        var assign = (Expr.Assign)expr!;

        Assert.Equal("t", assign.Name.Lexeme);
        Assert.IsType<Expr.Binary>(assign.Value);

        var minus = (Expr.Binary)assign.Value;

        Assert.Equal(TokenType.Minus, minus.Operator.Type);
        Assert.IsType<Expr.Grouping>(minus.Left);
        Assert.IsType<Expr.Call>(minus.Right);

        var group = (Expr.Grouping)minus.Left;
        var call = (Expr.Call)minus.Right;

        Assert.IsType<Expr.Binary>(group.Expression);

        var add = (Expr.Binary)group.Expression;

        Assert.Equal(TokenType.Plus, add.Operator.Type);
        Assert.IsType<Expr.Literal>(add.Left);
        Assert.IsType<Expr.Literal>(add.Right);

        var addLeft = (Expr.Literal)add.Left;
        var addRight = (Expr.Literal)add.Right;

        Assert.Equal(13d, addLeft.Value);
        Assert.Equal(4d, addRight.Value);

        Assert.IsType<Expr.Variable>(call.Callee);

        var callee = (Expr.Variable)call.Callee;

        Assert.Equal("myFun", callee.Name.Lexeme);

        Assert.Equal(2, call.Arguments.Count);
        Assert.IsType<Expr.Literal>(call.Arguments[0]);
        Assert.IsType<Expr.Logical>(call.Arguments[1]);

        var arg0 = (Expr.Literal)call.Arguments[0];
        var arg1 = (Expr.Logical)call.Arguments[1];

        Assert.Equal(true, arg0.Value);
    
        Assert.Equal(TokenType.And, arg1.Operator.Type);
        Assert.IsType<Expr.Binary>(arg1.Left);
        Assert.IsType<Expr.Binary>(arg1.Right);

        var arg1Left = (Expr.Binary)arg1.Left;
        var arg1Right = (Expr.Binary)arg1.Right;

        Assert.Equal(TokenType.Less, arg1Left.Operator.Type);
        Assert.IsType<Expr.Variable>(arg1Left.Left);
        Assert.IsType<Expr.Variable>(arg1Left.Right);

        var arg1LeftLeft = (Expr.Variable)arg1Left.Left;
        var arg1LeftRight = (Expr.Variable)arg1Left.Right;

        Assert.Equal("x", arg1LeftLeft.Name.Lexeme);
        Assert.Equal("y", arg1LeftRight.Name.Lexeme);

        Assert.Equal(TokenType.GreaterEqual, arg1Right.Operator.Type);
        Assert.IsType<Expr.Variable>(arg1Right.Left);
        Assert.IsType<Expr.Unary>(arg1Right.Right);

        var arg1RightLeft = (Expr.Variable)arg1Right.Left;
        var arg1RightRight = (Expr.Unary)arg1Right.Right;

        Assert.Equal("x", arg1LeftLeft.Name.Lexeme);

        Assert.Equal(TokenType.Minus, arg1RightRight.Operator.Type);
        Assert.IsType<Expr.Variable>(arg1RightRight.Right);

        var arg1RightRightRight = (Expr.Variable)arg1RightRight.Right;

        Assert.Equal("z", arg1RightRightRight.Name.Lexeme);
    }

    [Fact]
    public void TestVariableDeclaration()
    {
        var source = "var foo = \"bar\";";

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);

        var stmt = parser.Parse();        

        Assert.NotNull(stmt);
        Assert.Single(stmt);
        Assert.IsType<Stmt.Var>(stmt[0]);

        var varDecl = (Stmt.Var)stmt[0];

        Assert.Equal("foo", varDecl.Name.Lexeme);
        
        Assert.NotNull(varDecl.Initializer);
        Assert.IsType<Expr.Literal>(varDecl.Initializer);

        var value = (Expr.Literal)varDecl.Initializer!;

        Assert.Equal("bar", value.Value);
    }

    [Fact]
    public void TestFunctionDeclaration()
    {
        var source = @"fun incr(x) {
    return x + 1;
}";

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);

        var stmt = parser.Parse();        

        Assert.NotNull(stmt);
        Assert.Single(stmt);
        Assert.IsType<Stmt.Function>(stmt[0]);

        var funDecl = (Stmt.Function)stmt[0];

        Assert.Equal("incr", funDecl.Name.Lexeme);
        Assert.Single(funDecl.Func.Params);

        var param = funDecl.Func.Params[0];

        Assert.Equal("x", param.Lexeme);

        Assert.Single(funDecl.Func.Body);
        Assert.IsType<Stmt.Return>(funDecl.Func.Body[0]);

        var @return = (Stmt.Return)funDecl.Func.Body[0];

        Assert.NotNull(@return.Value);
        Assert.IsType<Expr.Binary>(@return.Value);

        var add = (Expr.Binary)@return.Value!;

        Assert.Equal(TokenType.Plus, add.Operator.Type);
        Assert.IsType<Expr.Variable>(add.Left);
        Assert.IsType<Expr.Literal>(add.Right);

        var left = (Expr.Variable)add.Left;
        var right = (Expr.Literal)add.Right;

        Assert.Equal("x", left.Name.Lexeme);
        Assert.Equal(1d, right.Value);
    }

    [Fact]
    public void TestAnonymousFunction()
    {
        var source = @"fun (x, y) {
    return x + y;
}";

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);

        var expr = parser.ParseExpression();        

        Assert.NotNull(expr);
        Assert.IsType<Expr.Function>(expr);

        var func = (Expr.Function)expr!;

        Assert.Equal(2, func.Params.Count);

        var param1 = func.Params[0];
        var param2 = func.Params[1];

        Assert.Equal("x", param1.Lexeme);
        Assert.Equal("y", param2.Lexeme);

        Assert.Single(func.Body);
        Assert.IsType<Stmt.Return>(func.Body[0]);

        var @return = (Stmt.Return)func.Body[0];

        Assert.NotNull(@return.Value);
        Assert.IsType<Expr.Binary>(@return.Value);

        var add = (Expr.Binary)@return.Value!;

        Assert.Equal(TokenType.Plus, add.Operator.Type);
        Assert.IsType<Expr.Variable>(add.Left);
        Assert.IsType<Expr.Variable>(add.Right);

        var left = (Expr.Variable)add.Left;
        var right = (Expr.Variable)add.Right;

        Assert.Equal("x", left.Name.Lexeme);
        Assert.Equal("y", right.Name.Lexeme);
    }
}
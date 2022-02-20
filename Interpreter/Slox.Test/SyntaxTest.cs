using Xunit;
using Slox.Scanning;
using Slox.Syntax;

namespace Slox.Test;

public class SyntaxTest
{
    [Fact]
    public void TestSExpressionAstPrinter()
    {
        var expr = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.Minus, "-", null, 1),
                new Expr.Literal(123)),
            new Token(TokenType.Star, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)));

        Assert.Equal("(* (- 123) (group 45.67))", new SExpressionAstPrinter().Print(expr));
    }

    [Fact]
    public void TestReversePolishNotationAstPrinter()
    {
        var expr = new Expr.Binary(
            new Expr.Binary(
                new Expr.Literal(1),
                new Token(TokenType.Plus, "+", null, 1),
                new Expr.Literal(2)),
            new Token(TokenType.Star, "*", null, 1),
            new Expr.Binary(
                new Expr.Literal(4),
                new Token(TokenType.Minus, "-", null, 1),
                new Expr.Literal(3)));

        Assert.Equal("1 2 + 4 3 - *", new ReversePolishNotationAstPrinter().Print(expr));
    }
}
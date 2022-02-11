using Xunit;
using Slox.Scanning;

namespace Slox.Test;

public class ScannerTest
{
    [Fact]
    public void TestScanningEmptyString()
    {
        var source = "";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.Single(tokens);

        // EOF
        Assert.Equal(TokenType.Eof, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
    }

    [Fact]
    public void TestScanningWhiteSpace()
    {
        var source = "      ";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.Single(tokens);

        // EOF
        Assert.Equal(TokenType.Eof, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
    }

    [Fact]
    public void TestScanningExpression()
    {
        var source = @"var language = ""slox"";";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.Equal(6, tokens.Count);

        // var
        Assert.Equal(TokenType.Var, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
        
        // language
        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal("language", tokens[1].Lexeme);
        Assert.Equal(1, tokens[1].Line);
        
        // =
        Assert.Equal(TokenType.Equal, tokens[2].Type);
        Assert.Equal(1, tokens[2].Line);
        
        // "slox"
        Assert.Equal(TokenType.String, tokens[3].Type);
        Assert.Equal(@"""slox""", tokens[3].Lexeme);
        Assert.Equal("slox", tokens[3].Literal);
        Assert.Equal(1, tokens[3].Line);
        
        // ;
        Assert.Equal(TokenType.Semicolon, tokens[4].Type);
        Assert.Equal(1, tokens[4].Line);
        
        // EOF
        Assert.Equal(TokenType.Eof, tokens[5].Type);
        Assert.Equal(1, tokens[5].Line);
    }

    [Fact]
    public void TestScanningComment()
    {
        var source = @"22 / 7 // approximation for pi";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.Equal(4, tokens.Count);

        // 22
        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(22d, tokens[0].Literal);
        Assert.Equal(1, tokens[0].Line);
        
        // /
        Assert.Equal(TokenType.Slash, tokens[1].Type);
        Assert.Equal(1, tokens[1].Line);
        
        // 7
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(7d, tokens[2].Literal);
        Assert.Equal(1, tokens[2].Line);

        // EOF
        Assert.Equal(TokenType.Eof, tokens[3].Type);
        Assert.Equal(1, tokens[3].Line);
    }

    [Fact]
    public void TestScanningMultipleLines()
    {
        var source = @"while (true) {
    print ""I'm stuck in a rut, stuck in a rut, stuck in a rut."";
}";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        Assert.Equal(10, tokens.Count);

        // while
        Assert.Equal(TokenType.While, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
        
        // (
        Assert.Equal(TokenType.LeftParen, tokens[1].Type);
        Assert.Equal(1, tokens[1].Line);
        
        // true
        Assert.Equal(TokenType.True, tokens[2].Type);
        Assert.Equal(1, tokens[2].Line);
        
        // )
        Assert.Equal(TokenType.RightParen, tokens[3].Type);
        Assert.Equal(1, tokens[3].Line);
        
        // {
        Assert.Equal(TokenType.LeftBrace, tokens[4].Type);
        Assert.Equal(1, tokens[4].Line);
        
        // print
        Assert.Equal(TokenType.Print, tokens[5].Type);
        Assert.Equal(2, tokens[5].Line);

        // "I'm stuck in a rut, stuck in a rut, stuck in a rut."
        Assert.Equal(TokenType.String, tokens[6].Type);
        Assert.Equal("I'm stuck in a rut, stuck in a rut, stuck in a rut.", tokens[6].Literal);
        Assert.Equal(2, tokens[6].Line);

        // ;
        Assert.Equal(TokenType.Semicolon, tokens[7].Type);
        Assert.Equal(2, tokens[7].Line);

        // }
        Assert.Equal(TokenType.RightBrace, tokens[8].Type);
        Assert.Equal(3, tokens[8].Line);

        // EOF
        Assert.Equal(TokenType.Eof, tokens[9].Type);
        Assert.Equal(3, tokens[9].Line);
    }
}
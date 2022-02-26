using Slox.Scanning;
using Slox.Syntax;

using static Slox.Scanning.TokenType;

namespace Slox.Parsing;

public class Parser
{
    private readonly IList<Token> _tokens;
    private int current = 0;

    private bool IsAtEnd => CurrentToken.Type == Eof;
    private Token CurrentToken => _tokens[current];
    private Token PreviousToken => _tokens[current - 1];

    public Parser(IEnumerable<Token> tokens)
    {
        _tokens = tokens.ToList();
    }

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParserError)
        {
            return null;
        }
    }

    // expression -> equality ;
    private Expr Expression() => Equality();

    // equality   -> comparison ( ( "!=" | "==" ) comparison )* ;
    private Expr Equality() => ParseBinary(Comparison, BangEqual, EqualEqual);

    // comparison -> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
    private Expr Comparison() => ParseBinary(Term, Greater, GreaterEqual, Less, LessEqual);

    // term       -> factor ( ( "-" | "+" ) factor)* ;
    private Expr Term() => ParseBinary(Factor, Minus, Plus);

    // factor     -> unary ( ( "/" | "*" ) unary)* ;
    private Expr Factor() => ParseBinary(Unary, Slash, Star);

    // unary      -> ( "!" | "-" ) unary | primary ;
    private Expr Unary()
    {
        if (Match(Bang, Minus)) {
            var @operator = PreviousToken;
            var right = Unary();
            return new Expr.Unary(@operator, right);
        }

        return Primary();
    }

    // primary    -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")"
    private Expr Primary()
    {
        if (Match(False)) return new Expr.Literal(false);
        if (Match(True)) return new Expr.Literal(true);
        if (Match(Nil)) return new Expr.Literal(null);

        if (Match(Number, TokenType.String)) return new Expr.Literal(PreviousToken.Literal);

        if (Match(LeftParen))
        {
            var expr = Expression();
            Consume(RightParen, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(CurrentToken, "Expect expression.");
    }

    private Expr ParseBinary(Func<Expr> operand, params TokenType[] operators)
    {
        var expr = operand();
        while (Match(operators))
        {
            var @operator = PreviousToken;
            var right = operand();
            expr = new Expr.Binary(expr, @operator, right);
        }

        return expr;
    }

    private bool Match(params TokenType[] types)
    {
        if (types.Any(Check))
        {
            Advance();
            return true;
        }
        return false;
    }

    private bool Check(TokenType type) => !IsAtEnd && CurrentToken.Type == type;

    private Token Advance() {
        if (!IsAtEnd) current++;
        return PreviousToken;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(CurrentToken, message);
    }

    private ParserError Error(Token token, string message)
    {
        Slox.Error.ReportError(token, message);
        return new ParserError();
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd)
        {
            if (PreviousToken.Type == Semicolon) return;

            if (CurrentToken.Type is
                Class or For or Fun or If or Print or Return or Var or While) return;

            Advance();
        }
    }
}
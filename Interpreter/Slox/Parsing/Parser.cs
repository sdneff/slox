using Slox.Scanning;
using Slox.Syntax;

using static Slox.Scanning.TokenType;

namespace Slox.Parsing;

public class Parser
{
    private readonly List<Token> _tokens = new();
    private int current = 0;

    private bool IsAtEnd => CurrentToken.Type == Eof;
    private Token CurrentToken => _tokens[current];
    private Token PreviousToken => _tokens[current - 1];

    public Parser(IEnumerable<Token> tokens)
    {
        _tokens.AddRange(tokens);
    }

    // program     -> statement* EOF ;
    public IList<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        while (!IsAtEnd)
        {
            var decl = Declaration();
            if (decl != null) statements.Add(decl);
        }

        return statements;
    }

    public Expr? ParseExpression()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    // declaration -> varDecl | statement ;
    private Stmt? Declaration()
    {
        try
        {
            if (Match(Var)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }


    // varDecl     -> "var" IDENTIFIER ( "=" expression )? ";" ;
    private Stmt VarDeclaration()
    {
        var name = Consume(Identifier, "Expcet variable name.");

        Expr? initializer = null;
        if (Match(Equal))
        {
            initializer = Expression();
        }
        Consume(Semicolon, "Expect ';' after expression.");
        return new Stmt.Var(name, initializer);
    }


    // statement   -> exprStmt | printStmt ;
    private Stmt Statement()
    {
        if (Match(Print)) return PrintStatement();

        return ExpressionStatement();
    }

    // exprStmt    -> expression ";" ;
    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(Semicolon, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    // printStmt   -> "print" expression ";" ;
    private Stmt PrintStatement()
    {
        var expr = Expression();
        Consume(Semicolon, "Expect ';' after value.");
        return new Stmt.Print(expr);
    }

    // expression  -> assignment ;
    private Expr Expression() => Assignment();
    
    // assignment  -> IDENTIFIER "=" assignment | equality ;
    private Expr Assignment() {
        var expr = Equality();

        if (Match(Equal))
        {
            var equals = PreviousToken;
            var value = Equality();

            if (expr is Expr.Variable @var)
            {
                var name = @var.Name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    // equality    -> comparison ( ( "!=" | "==" ) comparison )* ;
    private Expr Equality() => ParseBinary(Comparison, BangEqual, EqualEqual);

    // comparison  -> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
    private Expr Comparison() => ParseBinary(Term, Greater, GreaterEqual, Less, LessEqual);

    // term        -> factor ( ( "-" | "+" ) factor )* ;
    private Expr Term() => ParseBinary(Factor, Minus, Plus);

    // factor      -> unary ( ( "/" | "*" ) unary )* ;
    private Expr Factor() => ParseBinary(Unary, Slash, Star);

    // unary       -> ( "!" | "-" ) unary | primary ;
    private Expr Unary()
    {
        if (Match(Bang, Minus)) {
            var @operator = PreviousToken;
            var right = Unary();
            return new Expr.Unary(@operator, right);
        }

        return Primary();
    }

    // primary     -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | IDENTIFIER ;
    private Expr Primary()
    {
        if (Match(False)) return new Expr.Literal(false);
        if (Match(True)) return new Expr.Literal(true);
        if (Match(Nil)) return new Expr.Literal(null);

        if (Match(Number, TokenType.String)) return new Expr.Literal(PreviousToken.Literal);

        if (Match(Identifier)) return new Expr.Variable(PreviousToken);

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

    private ParseError Error(Token token, string message)
    {
        Slox.Error.ReportError(token, message);
        return new ParseError();
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
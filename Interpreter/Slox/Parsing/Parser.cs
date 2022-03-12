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
    private Token NextToken => _tokens[current + 1];
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

    // declaration -> funDecl | varDecl | statement ;
    private Stmt? Declaration()
    {
        try
        {
            if (Check(Fun) && CheckNext(Identifier))
            {
                Consume(Fun, "");
                return Function("function");
            }
            if (Match(Var)) return VarDeclaration();
            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    // funDecl     -> "fun" function ;
    // function    -> IDENTIFIER "(" parameters? ")" block ;
    // parameters  -> IDENTIFIER ( "," IDENTIFIER )* ;
    private Stmt Function(string kind)
    {
        var name = Consume(Identifier, $"Expect {kind} name.");
        var func = ParseFunction(kind);
        return new Stmt.Function(name, func);
    }

    // varDecl     -> "var" IDENTIFIER ( "=" expression )? ";" ;
    private Stmt VarDeclaration()
    {
        var name = Consume(Identifier, "Expect variable name.");

        Expr? initializer = null;
        if (Match(Equal))
        {
            initializer = Expression();
        }
        Consume(Semicolon, "Expect ';' after expression.");
        return new Stmt.Var(name, initializer);
    }

    // statement   -> exprStmt | forStmt | ifStmt | printStmt | returnStmt | whileStmt | block ;
    private Stmt Statement()
    {
        if (Match(For)) return ForStatement();
        if (Match(If)) return IfStatement();
        if (Match(Print)) return PrintStatement();
        if (Match(Return)) return ReturnStatement();
        if (Match(While)) return WhileStatement();
        if (Match(LeftBrace)) return new Stmt.Block(Block().ToList());

        return ExpressionStatement();
    }

    // exprStmt    -> expression ";" ;
    private Stmt ExpressionStatement()
    {
        var expr = Expression();
        Consume(Semicolon, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    // forStmt     -> "for" "(" ( varDecl | exprStmt | ";" ) expression? ";" expression? ")" statement ;
    private Stmt ForStatement()
    {
        Consume(LeftParen, "Expect '(' after 'for'.");

        var initializer = Match(Semicolon)
            ? null as Stmt
            : Match(Var)
                ? VarDeclaration()
                : ExpressionStatement();

        var condition = Check(Semicolon)
            ? null as Expr
            : Expression();

        Consume(Semicolon, "Expect ';' after loop condition.");

        var increment = Check(RightParen)
            ? null as Expr
            : Expression();

        Consume(RightParen, "Expect ')' after for clauses.");

        var body = Statement();

        // for (initializer condition increment) { body }
        // -- DESUGAR TO --
        // {
        //     initializer;
        //     while (condition) {
        //         body
        //         increment
        //     }
        // }

        if (increment != null)
        {
            body = new Stmt.Block(new List<Stmt> { body, new Stmt.Expression(increment) });
        }

        body = new Stmt.While(condition ?? new Expr.Literal(true), body);

        if (initializer != null)
        {
            body = new Stmt.Block(new List<Stmt> { initializer, body });
        }

        return body;
    }

    // ifStmt      -> "if" "(" expression ")" statement ( "else" statement )? ;
    private Stmt IfStatement()
    {
        Consume(LeftParen, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(RightParen, "Expect ')' after if condition.");

        var thenBranch = Statement();
        var elseBranch = Match(Else) ? Statement() : null as Stmt;
        return new Stmt.If(condition, thenBranch, elseBranch);
    }

    // printStmt   -> "print" expression ";" ;
    private Stmt PrintStatement()
    {
        var expr = Expression();
        Consume(Semicolon, "Expect ';' after value.");
        return new Stmt.Print(expr);
    }

    // returnStmt  -> "return" expression? ";" ;
    private Stmt ReturnStatement()
    {
        var keyword = PreviousToken;
        var value = Check(Semicolon)
            ? null as Expr
            : Expression();

        Consume(Semicolon, "Expect ';' after return value.");
        return new Stmt.Return(keyword, value);
    }

    // whileStmt   -> "while" "(" expression ")" body ;
    private Stmt WhileStatement()
    {
        Consume(LeftParen, "Expect '(' after 'while'.");
        var condition = Expression();
        Consume(RightParen, "Expect ')' after while condition.");
        var body = Statement();

        return new Stmt.While(condition, body);
    }

    // block       -> "{" declaration* "}" ;
    private IEnumerable<Stmt> Block()
    {
        while (!Check(RightBrace) && !IsAtEnd)
        {
            var decl = Declaration();
            if (decl != null) yield return decl;
        }
        Consume(RightBrace, "Expect '}' after block.");
    }

    // expression  -> assignment ;
    private Expr Expression() => Assignment();
    
    // assignment  -> IDENTIFIER "=" assignment | equality ;
    private Expr Assignment() {
        var expr = Or();

        if (Match(Equal))
        {
            var equals = PreviousToken;
            var value = Or();

            if (expr is Expr.Variable @var)
            {
                var name = @var.Name;
                return new Expr.Assign(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    // logic_or    -> logic_and ( "or" logic_and )* ;
    private Expr Or() => ParseLogical(And, TokenType.Or);

    // logic_and   -> equality ( "and" equality )* ;
    private Expr And() => ParseLogical(Equality, TokenType.And);

    // equality    -> comparison ( ( "!=" | "==" ) comparison )* ;
    private Expr Equality() => ParseBinary(Comparison, BangEqual, EqualEqual);

    // comparison  -> term ( ( ">" | ">=" | "<" | "<=" ) term )* ;
    private Expr Comparison() => ParseBinary(Term, Greater, GreaterEqual, Less, LessEqual);

    // term        -> factor ( ( "-" | "+" ) factor )* ;
    private Expr Term() => ParseBinary(Factor, Minus, Plus);

    // factor      -> unary ( ( "/" | "*" ) unary )* ;
    private Expr Factor() => ParseBinary(Unary, Slash, Star);

    // unary       -> ( "!" | "-" ) unary | call ;
    private Expr Unary()
    {
        if (Match(Bang, Minus)) {
            var @operator = PreviousToken;
            var right = Unary();
            return new Expr.Unary(@operator, right);
        }

        return Call();
    }

    // call        -> primary ( "(" arguments? ")" )* ;
    // arguments   -> expression ( "," expression )* ;
    private Expr Call()
    {
        var expr = Primary();

        while (true)
        {
            if (Match(LeftParen))
            {
                var arguments = new List<Expr>();
                if (!Check(RightParen))
                {
                    do
                    {
                        if (arguments.Count > 255)
                        {
                            Slox.Error.ReportError(CurrentToken, "Can't have more than 255 arguments.");
                        }
                        arguments.Add(Expression());
                    }
                    while (Match(Comma));
                }

                var paren = Consume(RightParen, "Expect ')' after arguments.");

                expr = new Expr.Call(expr, paren, arguments);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    // primary     -> NUMBER | STRING | "true" | "false" | "nil" | "(" expression ")" | IDENTIFIER | anon_func ;
    private Expr Primary()
    {
        if (Match(False)) return new Expr.Literal(false);
        if (Match(True)) return new Expr.Literal(true);
        if (Match(Nil)) return new Expr.Literal(null);
        if (Match(Fun)) return ParseFunction("anonymous function");

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
        => ParseBinaryImpl(operand, (l, op, r) => new Expr.Binary(l, op, r), operators);

    private Expr ParseLogical(Func<Expr> operand, params TokenType[] operators)
        => ParseBinaryImpl(operand, (l, op, r) => new Expr.Logical(l, op, r), operators);

    private Expr ParseBinaryImpl<T>(Func<Expr> operand, Func<Expr, Token, Expr, T> factory, params TokenType[] operators)
        where T : Expr 
    {
        var expr = operand();
        while (Match(operators))
        {
            var @operator = PreviousToken;
            var right = operand();
            expr = factory(expr, @operator, right);
        }

        return expr;
    }

    private Expr.Function ParseFunction(string kind)
    {
        Consume(LeftParen, $"Expect '(' after {kind} name.");
        var @params = new List<Token>();
        if (!Check(RightParen))
        {
            do
            {
                if (@params.Count > 255)
                {
                    Slox.Error.ReportError(CurrentToken, "Can't have more than 255 parameters.");
                }
                @params.Add(Consume(Identifier, "Expect parameter name."));
            }
            while (Match(Comma));
        }

        Consume(RightParen, "Expect ')' after parameters.");
        Consume(LeftBrace, $"Expect '{{' before {kind} body.");
        var body = Block().ToList();
        return new Expr.Function(@params, body);
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

    private bool CheckNext(TokenType type) => !IsAtEnd && NextToken.Type != Eof && NextToken.Type == type;

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
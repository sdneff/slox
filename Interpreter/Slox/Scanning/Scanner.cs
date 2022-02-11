using static Slox.Scanning.TokenType;

namespace Slox.Scanning;

public class Scanner
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        ["and"]     = And,
        ["class"]   = Class,
        ["else"]    = Else,
        ["false"]   = False,
        ["for"]     = For,
        ["fun"]     = Fun,
        ["if"]      = If,
        ["nil"]     = Nil,
        ["or"]      = Or,
        ["print"]   = Print,
        ["return"]  = Return,
        ["super"]   = Super,
        ["this"]    = This,
        ["true"]    = True,
        ["var"]     = Var,
        ["while"]   = While
    };

    private readonly string _source;
    private readonly List<Token> _tokens = new();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source) => _source = source;

    public IList<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        _tokens.Add(new Token(Eof, "", null, line));
        return _tokens;
    }

    private void ScanToken()
    {
        var c = Advance();
        switch (c)
        {
            case '(': AddToken(LeftParen); break;
            case ')': AddToken(RightParen); break;
            case '{': AddToken(LeftBrace); break;
            case '}': AddToken(RightBrace); break;
            case ',': AddToken(Comma); break;
            case '.': AddToken(Minus); break;
            case '+': AddToken(Plus); break;
            case ';': AddToken(Semicolon); break;
            case '*': AddToken(Star); break;

            case '!': AddToken(MatchNext('=') ? BangEqual : Bang); break;
            case '=': AddToken(MatchNext('=') ? EqualEqual : Equal); break;
            case '<': AddToken(MatchNext('=') ? LessEqual : Less); break;
            case '>': AddToken(MatchNext('=') ? GreaterEqual : Greater); break;

            case '/' when MatchNext('/'):
                // ignore line comment
                while (Peek() != '\n' && !IsAtEnd()) Advance();
                break;
            case '/': AddToken(Slash); break;

            case ' ':
            case '\t':
            case '\r':
                // ignore whitespace
                break;
            
            case '\n':
                line++;
                break;

            case '"':
                ScanString();
                break;

            default:
                if (IsDigit(c))
                {
                    ScanNumber();
                }
                else if (IsAlpha(c))
                {
                    ScanIdentifier();
                }
                else
                {
                    Console.WriteLine("ERROR"); break; // TODO: better error integration
                }
                break;
        }
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        _tokens.Add(new Token(type, CurrentText(), literal, line));
    }

    private bool IsAtEnd() => current >= _source.Length;
    private char Advance() => _source[current++];
    private char Peek() => IsAtEnd() ? default : _source[current];
    private char PeekNext() => current + 1 >= _source.Length ? default : _source[current + 1];
    private bool MatchNext(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[current] != expected) return false;

        current++;
        return true;
    }
    private string CurrentText() => _source.Substring(start, current - start);

    private void ScanString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Console.WriteLine("ERROR"); // TODO: better error integration <Unterminated string>
            return;
        }

        Advance(); // consume final '"'

        var value = _source.Substring(start + 1, current - start - 2); // trim quotes
        AddToken(TokenType.String, value);
    }

    private void ScanNumber()
    {
        while (IsDigit(Peek())) Advance();

        // handle fractional part
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance(); // consume '.'
    
            while (IsDigit(Peek())) Advance();
        }

        AddToken(Number, double.Parse(CurrentText()));
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();

        var type = Keywords.TryGetValue(CurrentText(), out var k) ? k : Identifier;
        AddToken(type);
    }

    private static bool IsDigit(char c) => char.IsDigit(c);
    private static bool IsAlpha(char c) => 
        (c >= 'a' && c <= 'z')
        || (c >= 'A' && c <= 'Z')
        || c == '_';
    private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
}
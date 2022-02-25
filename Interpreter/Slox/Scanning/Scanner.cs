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

    private bool IsAtEnd => current >= _source.Length;
    private char CurrentChar => IsAtEnd ? default : _source[current];
    private char NextChar => current + 1 >= _source.Length ? default : _source[current + 1];

    public Scanner(string source) => _source = source;

    public IList<Token> ScanTokens()
    {
        while (!IsAtEnd)
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
            case '.': AddToken(Dot); break;
            case '-': AddToken(Minus); break;
            case '+': AddToken(Plus); break;
            case ';': AddToken(Semicolon); break;
            case '*': AddToken(Star); break;

            case '!': AddToken(MatchNext('=') ? BangEqual : Bang); break;
            case '=': AddToken(MatchNext('=') ? EqualEqual : Equal); break;
            case '<': AddToken(MatchNext('=') ? LessEqual : Less); break;
            case '>': AddToken(MatchNext('=') ? GreaterEqual : Greater); break;

            case '/' when MatchNext('/'):
                // ignore line comment
                AdvanceWhile(c => c != '\n');
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
                    Slox.Error.ReportError(line, "Unexpected character.");
                }
                break;
        }
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        _tokens.Add(new Token(type, CurrentText(), literal, line));
    }

    private char Advance() => _source[current++];
    private void AdvanceWhile(Func<char, bool> match)
    {
        while (match(CurrentChar) && !IsAtEnd) Advance();
    }
    private bool MatchNext(char expected)
    {
        if (IsAtEnd) return false;
        if (_source[current] != expected) return false;

        current++;
        return true;
    }
    private string CurrentText() => _source.Substring(start, current - start);

    private void ScanString()
    {
        while (CurrentChar != '"' && !IsAtEnd)
        {
            if (CurrentChar == '\n') line++;
            Advance();
        }

        if (IsAtEnd)
        {
            Slox.Error.ReportError(line, "Unterminated string.");
            return;
        }

        Advance(); // consume final '"'

        var value = _source.Substring(start + 1, current - start - 2); // trim quotes
        AddToken(TokenType.String, value);
    }

    private void ScanNumber()
    {
        AdvanceWhile(IsDigit);

        // handle fractional part
        if (CurrentChar == '.' && IsDigit(NextChar))
        {
            Advance(); // consume '.'
    
            AdvanceWhile(IsDigit);
        }

        AddToken(Number, double.Parse(CurrentText()));
    }

    private void ScanIdentifier()
    {
        while (IsAlphaNumeric(CurrentChar)) Advance();

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
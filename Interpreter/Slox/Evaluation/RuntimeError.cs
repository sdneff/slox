using Slox.Scanning;

namespace Slox.Evaluation;

public class RuntimeError : ApplicationException
{
    public readonly Token Token;

    public RuntimeError(Token token, string message)
        : base(message)
    {
        Token = token;
    }
}
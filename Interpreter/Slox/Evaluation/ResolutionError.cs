using Slox.Scanning;

namespace Slox.Evaluation;

public class ResolutionError : ApplicationException
{
    public readonly Token Token;

    public ResolutionError(Token token, string message)
        : base(message)
    {
        Token = token;
    }
}

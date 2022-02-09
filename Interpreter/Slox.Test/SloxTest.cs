using Xunit;
using Slox.Scanning;

namespace Slox.Test;

public class SloxTest
{
    [Fact]
    public void TestTokenInstance()
    {
        var token = new Token(TokenType.LeftParen, "(", "(", 0);

        Assert.NotNull(token);
    }
}
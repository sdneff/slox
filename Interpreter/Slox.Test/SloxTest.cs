using Xunit;
using Slox;

namespace Slox.Test;

public class SloxTest
{
    [Fact]
    public void TestSloxLives()
    {
        var instance = new Slox.SloxStub();

        Assert.NotNull(instance);
    }
}
using Slox.Evaluation;
using Slox.Parsing;
using Slox.Scanning;
using Slox.Syntax;
using Xunit;

namespace Slox.Test;

public class ResolverTest
{
    [Fact]
    public void StaticScopeResolutionTest()
    {
        var source = @"var a = ""global"";
{
    fun showA() {
        print a;
    }

    showA();
    var a = ""block"";
    showA();
}";

        var reporter = new MockOutputReporter();

        var interpreter = new Interpreter(reporter);

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();

        var resolver = new Resolver(interpreter);
        resolver.Resolve(statements);

        interpreter.Interpret(statements);

        Assert.Equal(2, reporter.PrintLines.Count);
        Assert.Equal("global", reporter.PrintLines[0]);
        Assert.Equal("global", reporter.PrintLines[1]);
    }

    [Fact]
    public void VariableRedeclarationErrorTest()
    {
        var source = @"fun bad() {
    var a = ""first"";
    var a = ""second"";
}";

        var interpreter = new Interpreter();

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();

        var resolver = new Resolver(interpreter);

        var ex = Assert.Throws<ResolutionError>(() => resolver.Resolve(statements));

        Assert.Equal("Already a variable with this name in this scope.", ex.Message);
    }

    [Fact]
    public void TopLevelReturnErrorTest()
    {
        var source = @"return ""at top level"";";

        var interpreter = new Interpreter();

        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();

        var parser = new Parser(tokens);
        var statements = parser.Parse();

        var resolver = new Resolver(interpreter);

        var ex = Assert.Throws<ResolutionError>(() => resolver.Resolve(statements));

        Assert.Equal("Can't return from top-level code.", ex.Message);
    }
}
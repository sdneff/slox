namespace Slox.Syntax;

public interface IAstPrinter
{
    string Print(Expr expr);
}

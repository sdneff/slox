namespace Slox.Tool;

class Program
{
    internal static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: <outDir>");
            return;
        }

        var dir = new DirectoryInfo(args[0]);

        if (!dir.Exists)
        {
            Console.WriteLine($"Directory does not exist: {args[0]}.");
            return;
        }

        AstGenerator.DefineAst(dir, "Expr", new List<string>
        {
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Unary    : Token Operator, Expr Right"
        });

        Console.WriteLine($"AST code generated and written to: {dir.FullName}.");
    }
}
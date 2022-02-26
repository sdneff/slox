namespace Slox.Tool;

class Program
{
    internal static async Task Main(string[] args)
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

        await AstGenerator.DefineAst(dir, "Expr", new List<string>
        {
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Unary    : Token Operator, Expr Right"
        });

        await AstGenerator.DefineAst(dir, "Stmt", new List<string>
        {
            "Expression : Expr Expr",
            "Print      : Expr Expr"
        });

        Console.WriteLine($"AST code generated and written to: {dir.FullName}.");
    }
}
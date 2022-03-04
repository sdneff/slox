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
            "Assign   : Token Name, Expr Value",
            "Binary   : Expr Left, Token Operator, Expr Right",
            "Grouping : Expr Expression",
            "Literal  : object? Value",
            "Logical  : Expr Left, Token Operator, Expr Right",
            "Variable : Token Name",
            "Unary    : Token Operator, Expr Right"
        });

        await AstGenerator.DefineAst(dir, "Stmt", new List<string>
        {
            "Block      : List<Stmt> Statements",
            "Expression : Expr Expr",
            "If         : Expr Condition, Stmt ThenBranch, Stmt? ElseBranch",
            "Var        : Token Name, Expr? Initializer",
            "Print      : Expr Expr",
            "While      : Expr Condition, Stmt Body"
        });

        Console.WriteLine($"AST code generated and written to: {dir.FullName}.");
    }
}
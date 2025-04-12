namespace Stride.Compiler.Ast.Synthesis;

public static class AstNodeFactoryRepository
{
    private static readonly PackageNodeFactory PackageFactory = new();
    private static readonly ImportNodeFactory ImportFactory = new();
    private static readonly FunctionDeclarationNodeFactory FunctionDeclFactory = new();
    private static readonly ExpressionNodeFactory ExpressionFactory = new();


    public static readonly List<AbstractTreeNodeFactory> Factories = new()
    {
        PackageFactory,
        ImportFactory,
        FunctionDeclFactory,
        ExpressionFactory
    };
}
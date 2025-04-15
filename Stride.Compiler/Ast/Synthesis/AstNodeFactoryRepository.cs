namespace Stride.Compiler.Ast.Synthesis;

public static class AstNodeFactoryRepository
{
    private static readonly PackageNodeFactory PackageFactory = new();
    private static readonly ImportNodeFactory ImportFactory = new();
    private static readonly FunctionDeclarationNodeFactory FunctionDeclFactory = new();
    private static readonly ExpressionNodeFactory ExpressionFactory = new();
    private static readonly FunctionInvocationNodeFactory FunctionInvocationFactory = new();
    private static readonly AccessModifierFactoryDirector FactoryDirector = new(
        [FunctionDeclFactory]
        );

    public static readonly List<AbstractTreeNodeFactory> Factories = new()
    {
        PackageFactory,
        ImportFactory,
        FactoryDirector,
        ExpressionFactory,
        FunctionInvocationFactory,
    };
}
namespace Stride.Compiler.Ast.Synthesis;

public static class TreeNodeFactoryRepository
{
    public static readonly List<AbstractTreeNodeFactory> Factories = new()
    {
        new PackageNodeFactory(),
        new ImportNodeFactory(),
    };
}
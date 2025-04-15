using Stride.Compiler.Ast.Generic;

namespace Stride.Compiler.Ast.Nodes;

public class FunctionArgumentNode(string ArgName, InternalType ArgType) : AstNode
{
    public override string ToString()
    {
        return $"({ArgName}): {ArgType}";
    }
}
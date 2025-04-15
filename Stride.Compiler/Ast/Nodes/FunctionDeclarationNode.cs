using Stride.Compiler.Ast.Generic;
using Stride.Compiler.Ast.Synthesis;
using Stride.Compiler.Generic;

namespace Stride.Compiler.Ast.Nodes;

public class FunctionDeclarationNode(
    string functionName,
    List<FunctionArgumentNode> functionArguments,
    InternalType returnType,
    AccessModifier accessModifier = AccessModifier.Private
) : AstNode
{
    public override string ToString()
    {
        return $"Function {functionName} ({accessModifier}): Arguments[ {string.Join(", ", functionArguments.Select(arg => arg.ToString()))} ) -> {returnType}";
    }
}
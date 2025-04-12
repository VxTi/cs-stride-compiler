using Stride.Compiler.Ast.Generic;
using Stride.Compiler.Ast.Synthesis;
using Stride.Compiler.Generic;

namespace Stride.Compiler.Ast.Nodes;

public class FunctionDeclarationNode(
    string functionName,
    List<FunctionArgumentNode> functionArguments,
    InternalType returnType,
    Accessibility accessibility = Accessibility.Private
) : AstNode
{
    public override string ToString()
    {
        return $"Function {functionName} ({accessibility}): {string.Join(",", functionArguments.Select(arg => arg.ToString()))} -> {returnType}";
    }
}
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Nodes;

public class FunctionDeclarationNode(
    string functionName,
    List<FunctionDeclarationNode.FunctionArgument> functionArguments
) : AstNode
{
    public override void Validate()
    {
        throw new NotImplementedException();
    }

    public class FunctionArgument(
        string argName,
        TokenType argType,
        bool isArray = false)
    {
        public string ArgName { get; } = argName;

        public TokenType ArgType { get; } = argType switch
        {
            TokenType.Identifier => argType,
            TokenType.PrimitiveBool => TokenType.PrimitiveBool,
            TokenType.PrimitiveInt8 => TokenType.PrimitiveInt8,
            TokenType.PrimitiveInt16 => TokenType.PrimitiveInt16,
            TokenType.PrimitiveInt32 => TokenType.PrimitiveInt32,
            TokenType.PrimitiveInt64 => TokenType.PrimitiveInt64,
            TokenType.PrimitiveUInt8 => TokenType.PrimitiveUInt8,
            TokenType.PrimitiveUInt16 => TokenType.PrimitiveUInt16,
            TokenType.PrimitiveUInt32 => TokenType.PrimitiveUInt32,
            TokenType.PrimitiveUInt64 => TokenType.PrimitiveUInt64,
            TokenType.PrimitiveFloat32 => TokenType.PrimitiveFloat32,
            TokenType.PrimitiveFloat64 => TokenType.PrimitiveFloat64,
            TokenType.PrimitiveString => TokenType.PrimitiveString,
            TokenType.PrimitiveChar => TokenType.PrimitiveChar,
            TokenType.PrimitiveVoid => TokenType.PrimitiveVoid,
            _ => throw new IllegalTokenSequenceException("Invalid argument type")
        };

        public bool IsArray { get; } = isArray;
    }
    
    public class 
}
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Generic;

public static class Types
{
    public static bool IsValidType(TokenType type)
    {
        return IsPrimitive(type) || type == TokenType.Identifier;
    }

    public static bool IsPrimitive(TokenType type)
    {
        return type switch
        {
            TokenType.PrimitiveBool
                or TokenType.PrimitiveChar or TokenType.PrimitiveVoid
                or TokenType.PrimitiveString or TokenType.PrimitiveFloat32
                or TokenType.PrimitiveFloat64 or TokenType.PrimitiveInt8
                or TokenType.PrimitiveInt16 or TokenType.PrimitiveInt32
                or TokenType.PrimitiveInt64 or TokenType.PrimitiveUInt8
                or TokenType.PrimitiveUInt16 or TokenType.PrimitiveUInt32
                or TokenType.PrimitiveUInt64
                => true,
            _ => false
        };
    }
}
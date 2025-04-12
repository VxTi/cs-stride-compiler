using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Generic;

public class InternalType
{
    public readonly bool IsArray;

    private InternalType(bool isArray)
    {
        IsArray = isArray;
    }

    public static InternalType ExtractFromSet(TokenSet set)
    {
        var next = set.Next();
        if (next == null)
            throw new CompilationException("Failed to extract type: No tokens remaining.");
        
        if (!Types.IsValidType(next.Type))
            throw new CompilationException($"Failed to extract type: Invalid type {next.Type}");
        
        var isArray = false;

        if (set.PeekEqual(TokenType.LSquareBracket) &&
            set.PeekEqual(TokenType.RSquareBracket, 1))
        {
            set.Consume(TokenType.LSquareBracket);
            set.Consume(TokenType.RSquareBracket);
            isArray = true;
        }

        if (Types.IsPrimitive(next.Type))
            return new Primitive(next.Type, isArray);

        if (next.Type == TokenType.Identifier)
            return new Identifier(next.Value, isArray);

        throw new IllegalStateException(
            $"Unable to extract type: received token type {next.Type}");
    }

    public class Primitive : InternalType
    {
        public readonly TokenType Type;
        public Primitive(TokenType type, bool isArray) : base(isArray)
        {
            if (!Types.IsPrimitive(type))
                throw new IllegalArgumentException($"Cannot instantiate primitive type with token type {type}");
            
            Type = type;
        }
        public override string ToString()
        {
            return $"Type@{Type}" + (IsArray ? "[]" : "");
        }
    }

    public class Identifier(string ReferenceName, bool isArray) : InternalType(isArray)
    {
        public override string ToString()
        {
            return $"Type@{ReferenceName}" + (isArray ? "[]" : "");
        }
    }
}
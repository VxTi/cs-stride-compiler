using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Nodes;

public class AtomicValueNode : Expression
{
    public enum NumberType
    {
        I32,
        I64,
        F32,
        F64,
        None
    }
    
    public readonly dynamic Value;
    public readonly TokenType Type;
    public readonly NumberType NumericType = NumberType.None;

    public AtomicValueNode(string value)
    {
        Value = value;
        Type = TokenType.StringLiteral;
    }
    
    public AtomicValueNode(float floatValue)
    {
        Type = TokenType.NumberFloat;
        Value = floatValue;
        NumericType = NumberType.F32;
    }

    public AtomicValueNode(double doubleValue)
    {
        Type = TokenType.NumberFloat;
        Value = doubleValue;
        NumericType = NumberType.F64;
    }

    public AtomicValueNode(int intValue)
    {
        Type = TokenType.NumberInteger;
        Value = intValue;
        NumericType = NumberType.I32;
    }

    public AtomicValueNode(long intValue)
    {
        Type = TokenType.NumberInteger;
        Value = intValue;
        NumericType = NumberType.I64;
    }

    public bool IsNumeric()
    {
        return IsFloat() || IsInteger();
    }

    public bool IsInteger()
    {
        return Type == TokenType.NumberInteger;
    }

    public bool Is32BitInteger()
    {
        if (!IsInteger())
            return false;
        
        return (Value & ((1L << 16) - 1)) != 0;
    }

    public int? GetInt32()
    {
        if (!Is32BitInteger())
            return null;

        return Value;
    }

    public long? GetInt64()
    {
        if (!Is64BitInteger())
            return null;

        return Value;
    }

    public bool Is64BitInteger()
    {
        if (!IsInteger())
            return false;
        
        return (Value & ((1L << 32) - 1)) == 0;
    }

    public bool IsFloat()
    {
        return Type == TokenType.NumberFloat;
    }

    public bool IsString()
    {
        return Type == TokenType.StringLiteral;
    }
}
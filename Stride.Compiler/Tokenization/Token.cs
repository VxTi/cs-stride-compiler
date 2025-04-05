namespace Stride.Compiler.Tokenization;

public class Token(TokenType tokenType, string value)
{
    public readonly TokenType Type = tokenType;
    public readonly string Value = value;

    public string StringifyTokenType()
    {
        return Type switch
        {
            TokenType.KeywordFn => "Fn",
            TokenType.KeywordPub => "Pub",
            TokenType.KeywordPkg => "Module",
            TokenType.KeywordAnd => "And",
            TokenType.KeywordHas => "Has",
            TokenType.KeywordAs => "As",
            TokenType.KeywordLet => "Let",
            TokenType.KeywordReturn => "Return",
            TokenType.KeywordIf => "If",
            TokenType.KeywordElse => "Else",
            TokenType.KeywordClass => "Class",
            TokenType.KeywordConst => "Const",
            TokenType.KeywordAsync => "Async",
            TokenType.KeywordThis => "This",
            TokenType.KeywordEnum => "Enum",
            TokenType.KeywordSwitch => "Switch",
            TokenType.KeywordCase => "Case",
            TokenType.KeywordDefault => "Default",
            TokenType.KeywordBreak => "Break",
            TokenType.KeywordStruct => "Struct",
            TokenType.KeywordUse => "Import",
            TokenType.KeywordExternal => "External",
            TokenType.KeywordNull => "Null",
            TokenType.KeywordOverride => "Override",
            TokenType.KeywordDo => "Do",
            TokenType.KeywordWhile => "While",
            TokenType.KeywordFor => "For",
            TokenType.KeywordTry => "Try",
            TokenType.KeywordCatch => "Catch",
            TokenType.KeywordThrow => "Throw",
            TokenType.KeywordNew => "New",
            TokenType.PrimitiveBool => "Bool",
            TokenType.PrimitiveString => "String",
            TokenType.PrimitiveChar => "Char",
            TokenType.PrimitiveVoid => "Void",
            TokenType.PrimitiveVar => "Auto",
            TokenType.PrimitiveInt8 => "Int8",
            TokenType.PrimitiveInt16 => "Int16",
            TokenType.PrimitiveInt32 => "Int32",
            TokenType.PrimitiveInt64 => "Int64",
            TokenType.PrimitiveFloat32 => "Float32",
            TokenType.PrimitiveFloat64 => "Float64",
            TokenType.Identifier => "Identifier",
            TokenType.NumberInteger => "Integer",
            TokenType.BooleanLiteral => "Boolean",
            TokenType.NumberFloat => "Float",
            TokenType.StringLiteral => "String",
            TokenType.CharLiteral => "Char",
            _ => Value
        };
    }
    
    public override string ToString()
    {
        return StringifyTokenType() + ": " + Value;
    }
}
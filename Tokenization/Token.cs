namespace StrideCompiler.Tokenization;

public class Token(TokenType tokenType, string value)
{
    public readonly TokenType Type = tokenType;
    public readonly string Value = value;
}
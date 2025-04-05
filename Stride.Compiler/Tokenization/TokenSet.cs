namespace Stride.Compiler.Tokenization;

public class TokenSet(string sourceFilePath)
{
    public readonly string SourceFilePath = sourceFilePath;
    public List<Token> Tokens = new();

    public void Append(Token token)
    {
        Tokens.Add(token);
    }
}
namespace StrideCompiler.Tokenization;

public class TokenSet(string sourceFilePath)
{
    public readonly string SourceFilePath = sourceFilePath;
    protected List<Token> Tokens = new();

    public void Append(Token token)
    {
        Tokens.Add(token);
    }
}
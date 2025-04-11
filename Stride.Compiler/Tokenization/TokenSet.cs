using Stride.Compiler.Exceptions;

namespace Stride.Compiler.Tokenization;

public class TokenSet(string sourceFilePath)
{
    public readonly string SourceFilePath = sourceFilePath;
    public List<Token> Tokens = new();

    public int Index = 0;
    
    public void Append(Token token)
    {
        Tokens.Add(token);
    }
    
    public Token? Peek()
    {
        return Index >= Tokens.Count ? null : Tokens[Index];
    }
    
    public bool PeekEqual(TokenType tokenType)
    {
        if (Index >= Tokens.Count)
            return false;
        
        return Tokens[Index].Type == tokenType;
    }
    
    public void ConsumeOptional(TokenType tokenType)
    {
        if (Index >= Tokens.Count)
            return;
        
        if (Tokens[Index].Type != tokenType)
            return;
        
        Index++;
    }
    
    public void Consume(TokenType tokenType)
    {
        if (Index >= Tokens.Count)
            throw new CompilationException($"Expected token of type {tokenType}, but got {Tokens[Index].Type}");
        
        if (Tokens[Index].Type != tokenType)
            throw new CompilationException($"Expected token of type {tokenType}, but got {Tokens[Index].Type}");
        
        Index++;
    }
    
    public Token RequiresNext(TokenType type)
    {
        if (Index >= Tokens.Count)
            throw new CompilationException($"Expected token, but got end of token set");
        
        var next = Next();
        if (next == null)
            throw new CompilationException($"Expected token of type {type}, but got end of token set");
        
        if (next.Type != type)
            throw new CompilationException($"Expected token of type {type}, but got {next.Type}");
        
        return next;
    }
    
    public Token? Next()
    {
        if (Index >= Tokens.Count)
            return null;
        
        var token = Tokens[Index];
        Index++;
        return token;
    }
}
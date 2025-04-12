using Stride.Common.Logging;
using Stride.Compiler.Exceptions;

namespace Stride.Compiler.Tokenization;

public class TokenSet(string sourceFilePath, List<Token> tokens)
{
    public readonly string SourceFilePath = sourceFilePath;
    public readonly List<Token> Tokens = tokens;

    public int Index = 0;

    public TokenSet(string sourceFilePath) : this(sourceFilePath, [])
    {
    }


    public int Remaining()
    {
        return Tokens.Count - Index - 1;
    }
    
    public Token? Peek(int offset = 0)
    {
        Logger.Info($"Peaking: {(Index + offset >= Tokens.Count ? null : Tokens[Index + offset])} {Index + offset}");
        return Index + offset >= Tokens.Count ? null : Tokens[Index + offset];
    }

    public bool PeekEqual(TokenType tokenType, int offset = 0)
    {
        return Peek(offset)?.Type == tokenType;
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

    public TokenSet Clone()
    {
        return new TokenSet(SourceFilePath, Tokens);
    }
}
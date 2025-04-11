using Stride.Common.Logging;
using Stride.Compiler.Exceptions;

namespace Stride.Compiler.Tokenization;

public static class Tokenizer
{
    public static TokenSet Tokenize(string sourceFilePath, string sourceContent)
    {
        TokenSet tokenSet = new(sourceFilePath);
        Logger.Log(LogLevel.Debug, $"Tokenizing source {sourceFilePath}");

        for (var index = 0; index < sourceContent.Length;)
        {
            var token = TryMatch(sourceContent, index);

            if (token == null)
                throw new CompilationException($"Illegal token found: {sourceContent[index]}");
            
            index += token.Value.Length;
            Logger.Debug($"Skipping {token.Value.Length} ({token.Type})");
            if (token.Type == TokenType.IgnoreToken)
                continue;

            Logger.Log(LogLevel.Debug, $"Tokenized {token.Type} {token.Value}");

            tokenSet.Append(token);
        }

        return tokenSet;
    }

    private static Token? TryMatch(string source, int offset)
    {
        foreach (var (pattern, tokenType) in TokenPatterns.Patterns)
        {
            var match = pattern.Match(source, offset);

            if (!match.Success)
                continue;
            
            return new(tokenType, match.Value);
        }

        return null;
    }
}
using Stride.Common.Logging;
using Stride.Compiler.Exceptions;

namespace Stride.Compiler.Tokenization;

public static class Tokenizer
{
    public static TokenSet Tokenize(string sourceFilePath, string sourceContent)
    {
        List<Token> tokens = new();

        for (var index = 0; index < sourceContent.Length;)
        {
            var token = TryMatch(sourceContent, index);

            if (token == null)
            {
                IllegalToken(sourceContent, index);
                throw new CompilationException($"Illegal token found: {sourceContent[index]}");
            }

            index += token.Value.Length;

            if (token.Type == TokenType.IgnoreToken)
                continue;

            tokens.Add(token);
        }

        return new TokenSet(sourceFilePath, tokens);
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

    private static void IllegalToken(string source, int offset)
    {
        var before = source.Substring(0, offset);
        var faultyChar = source[offset].ToString();
        var after = source.Substring(offset + 1);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(before);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(faultyChar);

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(after);

        Console.ResetColor();
    }
}
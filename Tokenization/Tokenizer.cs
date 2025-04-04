using StrideCompiler.Exceptions;

namespace StrideCompiler.Tokenization;

using Logging;
using Project;

public static class Tokenizer
{
    public static List<TokenSet> StartTokenization(Project project)
    {
        List<TokenSet> tokenSets = new List<TokenSet>();
        Logger.Log(LogLevel.Debug, "Started tokenizing project");

        var mainFileContent = File.ReadAllText(project.ProjectConfig.MainFilePath);

        tokenSets.Add(Tokenize(project.ProjectConfig.MainFilePath, mainFileContent));

        return tokenSets;
    }

    private static TokenSet Tokenize(string sourceFilePath, string sourceContent)
    {
        TokenSet tokenSet = new(sourceFilePath);
        Logger.Log(LogLevel.Debug, $"Tokenizing source {sourceFilePath}");

        for (var index = 0; index < sourceContent.Length;)
        {
            var matched = false;
            foreach (var (pattern, tokenType) in TokenPatterns.Patterns)
            {
                var match = pattern.Match(sourceContent, index);

                if (!match.Success)
                    continue;

                index += match.Length;
                matched = true;

                // We won't want any comments or whitespace in our set
                if (tokenType == TokenType.IgnoreToken)
                    continue;

                Logger.Log(LogLevel.Debug, $"Tokenized [{index}]: [{tokenType}] {match.Value} ({match.Length})");

                tokenSet.Append(new(tokenType, match.Value));
                break;
            }

            if (!matched)
                throw new CompilationException(
                    $"Illegal token found: {sourceContent[index + (index + 1 >= sourceContent.Length ? 0 : 1)]}");
        }

        return tokenSet;
    }
}
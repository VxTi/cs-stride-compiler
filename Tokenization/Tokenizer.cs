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

        var mainFileContent = File.ReadAllLines(project.Config.MainFilePath);

        tokenSets.Add(Tokenize(project.Config.MainFilePath, mainFileContent));

        return tokenSets;
    }

    private static TokenSet Tokenize(string sourceFilePath, string[] sourceLines)
    {
        TokenSet tokenSet = new(sourceFilePath);
        Logger.Log(LogLevel.Debug, $"Tokenizing source {sourceFilePath}");

        int line, offset;
        bool fallthrough;

        for (line = 0; line < sourceLines.Length; line++)
        { 
            for (offset = 0; offset < sourceLines[line].Length; offset++)
            {
                fallthrough = false;
                foreach (var (pattern, tokenType) in TokenPatterns.Patterns)
                {
                    var match = pattern.Match(sourceLines[line], offset); 
                    if (match.Success)
                    {
                        tokenSet.Append(new(tokenType, match.Value));   
                        fallthrough = true;
                    }
                }

                if (!fallthrough)
                    throw new CompilationException("Error whilst tokenizing source", [
                        new ErrorFragment(sourceLines, line, offset, 1, "Illegal token")
                    ]);
            }
        }
        return tokenSet;
    }
}
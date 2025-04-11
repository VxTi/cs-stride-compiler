using Stride.Common;
using Stride.Common.Logging;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler;

public static class Compiler
{
    public static async Task CompileFile(string filePath, string[] packages)
    {
        if (!File.Exists(filePath))
            throw new Exception($"File not found: {filePath}");

        var fileContent = await File.ReadAllTextAsync(filePath);
        var tokenSet = Tokenizer.Tokenize(filePath, fileContent);

        foreach (var token in tokenSet.Tokens)
            Logger.Log(token.ToString());
    }
}
using Stride.Common;
using Stride.Common.Logging;
using Stride.Compiler.Ast.Synthesis;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler;

public static class Compiler
{
    public static async Task CompileFile(string filePath, string[] packages)
    {
        if (!File.Exists(filePath))
            throw new Exception($"File not found: {filePath}");

        var fileContent = await File.ReadAllTextAsync(filePath);

        CompileSource(filePath, fileContent, packages);
    }

    public static void CompileSource(string filePath, string source, string[] packages)
    {
        var tokenSet = Tokenizer.Tokenize(filePath, source);
        var ast = AstNodeFactory.GenerateAst(tokenSet);
        Logger.Info($"Generated AST: {ast}");
    }
}
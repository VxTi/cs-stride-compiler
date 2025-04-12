using Stride.Common.Logging;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public static class AstNodeFactory
{
    public static AstNode GenerateAst(TokenSet tokenSet)
    {
        var rootNode = new AstNode.RootNode();
        GenerateAst(rootNode, tokenSet);
        return rootNode;
    }

    public static void GenerateAst(AstNode rootNode, TokenSet set)
    {
        if (set.Remaining() == 0)
            return;
        
        Logger.Log(LogLevel.Debug,
            $"Generating AST for {set.SourceFilePath} with {set.Tokens.Count} tokens");

        while (set.Remaining() > 0)
        {
            try
            {
                foreach (var factory in AstNodeFactoryRepository.Factories)
                {
                    var token = set.Peek();
                    if (token == null)
                        throw new NullReferenceException("No token found");

                    if (!factory.CanConsumeToken(token, set))
                        continue;
                    
                    Logger.Info($"Factory: {factory.GetType().Name}");

                    factory.Synthesize(set, rootNode);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"An error occurred: {e.Message}");
                return;
            }
        }
    }
}
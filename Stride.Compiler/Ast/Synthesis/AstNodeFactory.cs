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

    public static void GenerateAst(AstNode rootNode, TokenSet tokenSet)
    {
        if (tokenSet.Remaining() == 0)
            return;
        
        Logger.Log(LogLevel.Debug,
            $"Generating AST for {tokenSet.SourceFilePath} with {tokenSet.Tokens.Count} tokens");

        var iterations = 0;
        while (tokenSet.Remaining() > 0)
        {
            try
            {
                foreach (var factory in AstNodeFactoryRepository.Factories)
                {
                    var token = tokenSet.Peek();
                    if (token == null)
                        throw new NullReferenceException("No token found");

                    if (!factory.CanConsumeToken(token))
                        continue;
                    
                    Logger.Info($"Factory: {factory.GetType().Name}");

                    factory.Synthesize(tokenSet, rootNode);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"An error occurred: {e.Message}");
                return;
            }

            iterations++;
        }

        Logger.Info($"Iterations: {iterations}");
    }
}
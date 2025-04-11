using Stride.Common.Logging;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public static class NodeFactory
{
    public static AstNode GenerateAst(TokenSet tokenSet)
    {
        Logger.Log(LogLevel.Debug, $"Generating AST for {tokenSet.SourceFilePath} with tokens: {tokenSet.Tokens.Count}");
        var rootNode = new AstNode.RootNode();
        TreeNodeFactoryRepository.Factories.ForEach(factory =>
        {
            var token = tokenSet.Peek();
            if (token == null)
                return;

            if (!factory.CanConsumeToken(token))
                return;
            
            var node = factory.Synthesize(tokenSet);
            if (node == null)
                return;
            
            Logger.Log(LogLevel.Debug, $"Generated node: {node}");

            rootNode.Children.Add(node);
        });
        
        rootNode.Validate();
        
        return rootNode;
    }
}
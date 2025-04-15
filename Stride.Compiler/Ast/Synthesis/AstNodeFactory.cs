using Stride.Common.Logging;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public static class AstNodeFactory
{
    public static AstNode GenerateAst(TokenSet tokenSet)
    {
        var contextMetadata = new ContextMetadata();
        var rootNode = new AstNode.RootNode();
        GenerateAst(rootNode, tokenSet, contextMetadata);
        return rootNode;
    }

    public static void GenerateAst(AstNode rootNode, TokenSet set, ContextMetadata ctx)
    {
        GenerateAstFromFactories(rootNode, set, AstNodeFactoryRepository.Factories, ctx);
    }

    public static void GenerateAstFromFactories(
        AstNode rootNode,
        TokenSet set,
        List<AbstractTreeNodeFactory> factories,
        ContextMetadata ctx
    )
    {
        if (set.Remaining() == 0)
            return;

        Logger.Debug($"Generating AST for {set.SourceFilePath} with {set.Tokens.Count} tokens");

        while (set.Remaining() > 0)
        {
            try
            {
                foreach (var factory in factories)
                {
                    var token = set.Peek();
                    if (token == null)
                        throw new NullReferenceException("No token found");

                    if (!factory.CanConsumeToken(token, set))
                        continue;

                    Logger.Info($"Factory: {factory.GetType().Name}");

                    factory.Synthesize(set, rootNode, ctx);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"An error occurred: {e.Message}");
                return;
            }
        }
    }
    
    public static void ParseBlock(TokenSet set, AstNode parent, ContextMetadata ctx)
    {
        if (ctx.CurrentScope == LexicalScope.Global)
            throw new CompilationException("Cannot parse block in global context");
        
        set.Consume(TokenType.LBrace);
        int indentLevel = 1, offset = 0;

        for (; set.Index + offset < set.Tokens.Count; offset++)
        {
            if (set.PeekEqual(TokenType.LBrace))
                indentLevel++;
            if (set.PeekEqual(TokenType.RBrace))
                indentLevel--;

            if (indentLevel <= 0)
                break;
        }
        var sublist = set.Tokens.Slice(set.Index, offset);
        Logger.Info($"Subset len: {sublist.Count}");
        var subset = new TokenSet(set.SourceFilePath, sublist);
        
        foreach (var token in sublist) Logger.Info($"{token}");

        var childContext = ctx.Create();
        childContext.CurrentScope = LexicalScope.Block;
        
        GenerateAst(parent, subset, childContext);

        set.Next(sublist.Count + 1); // Consumes another bracket
    }
}
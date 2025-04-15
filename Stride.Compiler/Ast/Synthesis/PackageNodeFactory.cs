using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class PackageNodeFactory : AbstractTreeNodeFactory
{
    
    public override void Synthesize(TokenSet set, AstNode rootNode, ContextMetadata? metadata)
    {
        set.Consume(TokenType.KeywordPackage);
        var packageSym = Symbol.FromTokenSet(set);
        set.ConsumeOptional(TokenType.Semicolon);
        
        rootNode.Children.Add(new PackageNode(packageSym));
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type == TokenType.KeywordPackage;
    }
}
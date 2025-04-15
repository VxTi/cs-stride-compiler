using Stride.Compiler.Exceptions;
using Stride.Compiler.Generic;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class AccessModifierFactoryDirector(List<AbstractTreeNodeFactory> factories)
    : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode, ContextMetadata metadata)
    {
        var next = set.Peek();
        if (next == null)
            throw new CompilationException("Cannot extract access modifier; end of set");

        var meta = metadata;
        
        if (Modifiers.IsAccessModifier(next.Type))
        {
            
            
            var accessModifierToken = set.Next();
            if (accessModifierToken == null)
                throw new CompilationException("Cannot extract access modifier; end of set");

            meta = metadata.Create();
            meta.Modifiers = ConsumeModifiers(set);
        }

        AstNodeFactory.GenerateAstFromFactories(rootNode, set, factories, meta);
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.Global;
    }

    private static int ConsumeModifiers(TokenSet set)
    {
        var modifiers = 0;

        for (var offset = 1; set.Index + offset < set.Tokens.Count && set.Remaining() > 0; offset++)
        {
            var peaked = set.Peek(offset);

            if (peaked == null)
                throw new CompilationException("Cannot extract access modifier; end of set");
            
            if (!Modifiers.IsModifier(peaked.Type))
                break;

            var modifier = Modifiers.FromToken(peaked.Type);

            if ((modifiers & ~modifier) != 0)
                throw new IllegalStateException("Found duplicate modifier");
            
            modifiers |= modifier;
        }
        
        return modifiers;
    }
    
    private static bool CanBeAnnotatedWithAccessor(TokenType type)
    {
        return type is TokenType.KeywordFn
            or TokenType.KeywordStruct
            or TokenType.KeywordClass
            or TokenType.KeywordAsync;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return Modifiers.IsAccessModifier(nextToken.Type)
               || CanBeAnnotatedWithAccessor(nextToken.Type);
    }
}
using Stride.Common.Logging;
using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class ImportNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode)
    {
        set.Consume(TokenType.KeywordImport);
        
        if (set.PeekEqual(TokenType.LParenthesis))
        {
            var imports = ParseSequentialImports(set);
            rootNode.Children.Add(new ImportNode(imports));
        }
        else
        {
            rootNode.Children.Add(new ImportNode([ParseImportEntry(set)]));
        }
         
        set.ConsumeOptional(TokenType.Semicolon);
    }

    private static List<ImportNode.Import> ParseSequentialImports(TokenSet set)
    {
        set.Consume(TokenType.LParenthesis);
        List<ImportNode.Import> imports = new();
        
        imports.Add(ParseImportEntry(set));

        do
        {
            set.Consume(TokenType.Comma);
            imports.Add(ParseImportEntry(set));
        } while (!set.PeekEqual(TokenType.RParenthesis) && set.Remaining() > 0);

        set.ConsumeOptional(TokenType.Comma); // Optional trailing comma
        set.Consume(TokenType.RParenthesis);

        return imports;
    }
    
    private static ImportNode.Import ParseImportEntry(TokenSet tokenSet)
    {
        var identifier = tokenSet.RequiresNext(TokenType.Identifier).Value;
        List<string> nsIdentifiers = [identifier];

        while (tokenSet.PeekEqual(TokenType.Dot) && tokenSet.Remaining() > 0)
        {
            tokenSet.Consume(TokenType.Dot);
            var subIdentifier = tokenSet.RequiresNext(TokenType.Identifier);
            nsIdentifiers.Add(subIdentifier.Value);
        }
        
        return new(new Symbol(nsIdentifiers));
    }

    public override PermittedLexicalScope GetLexicalScope()
    {
        return PermittedLexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type == TokenType.KeywordImport;
    }
}
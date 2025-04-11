using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class ImportNodeFactory : AbstractTreeNodeFactory
{
    public override AstNode? Synthesize(TokenSet tokens)
    {
        tokens.Consume(TokenType.KeywordImport);

        if (tokens.PeekEqual(TokenType.LParen))
        {
            var imports = ParseSequentialImports(tokens);
            return new ImportNode(imports);
        }
         
        var singularImport = ParseSingularImport(tokens);
        tokens.ConsumeOptional(TokenType.Semicolon); // Optional
        return new ImportNode([singularImport]);
    }

    private static List<ImportNode.Import> ParseSequentialImports(TokenSet tokenSet)
    {
        tokenSet.Consume(TokenType.LParen);
        List<ImportNode.Import> imports = new();

        do
        {
            var initial = tokenSet.RequiresNext(TokenType.Identifier);
            List<string> namespaceIdentifiers = [initial.Value];
            
            while (tokenSet.PeekEqual(TokenType.Dot))
            {
                tokenSet.Consume(TokenType.Dot);
                var nextToken = tokenSet.RequiresNext(TokenType.Identifier);
                namespaceIdentifiers.Add(nextToken.Value);
                tokenSet.ConsumeOptional(TokenType.Comma);
            }

            imports.Add(new(namespaceIdentifiers));

        } while (!tokenSet.PeekEqual(TokenType.RParen));

        tokenSet.Consume(TokenType.RParen);

        return imports;
    }
    
    private static ImportNode.Import ParseSingularImport(TokenSet tokenSet)
    {
        var nextToken = tokenSet.Next();
        
        if (nextToken is not { Type: TokenType.Identifier })
            throw new IllegalTokenSequenceException("Expected import name, but got " + nextToken?.Type);
        
        return new([nextToken.Value]);
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken)
    {
        return nextToken.Type == TokenType.KeywordImport;
    }
}
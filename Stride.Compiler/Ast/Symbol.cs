using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast;

public class Symbol(List<string> accessors) : AstNode
{
    public override string ToString()
    {
        return string.Join(", ", accessors);
    }

    public static Symbol FromTokenSet(TokenSet set)
    {
        var identifier = set.RequiresNext(TokenType.Identifier).Value;
        List<string> nsIdentifiers = [identifier];

        while (set.PeekEqual(TokenType.Dot) && set.Remaining() > 0)
        {
            set.Consume(TokenType.Dot);
            var subIdentifier = set.RequiresNext(TokenType.Identifier);
            nsIdentifiers.Add(subIdentifier.Value);
        }

        return new(nsIdentifiers);
    }
}
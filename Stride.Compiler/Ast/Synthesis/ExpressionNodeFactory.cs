using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class ExpressionNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode)
    {
        throw new NotImplementedException();
    }

    public override PermittedLexicalScope GetLexicalScope()
    {
        return PermittedLexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken)
    {
        return true;
    }
}
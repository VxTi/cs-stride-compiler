using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public abstract class AbstractTreeNodeFactory
{
    /**
     * Synthesize a node from the given token set.
     * This method should be implemented by subclasses to create specific node types.
     * <param name="tokens">The token set to synthesize from.</param>
     * <returns>The synthesized node, or null if the synthesis failed.</returns>
     */
    public abstract AstNode? Synthesize(TokenSet tokens);

    /**
     * The type of node this factory can synthesize.
     * This is used to determine which factory to use when synthesizing a node.
     */
    public abstract LexicalScope GetLexicalScope();

    /**
     * Abstraction for checking whether the next token can be consumed by this factory.
     * This is used to determine which factory to use when synthesizing a node.
     * <param name="nextToken">The next token to check.</param>
     */
    public abstract bool CanConsumeToken(Token nextToken);
}
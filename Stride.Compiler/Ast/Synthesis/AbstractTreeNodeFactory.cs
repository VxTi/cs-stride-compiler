using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public abstract class AbstractTreeNodeFactory
{
    /**
     * Synthesize a node from the given token set.
     * This method should be implemented by subclasses to create specific node types.
     * <param name="set">The token set to synthesize from.</param>
     * <param name="rootNode">The root node of the AST</param>
     * <param name="metadata">Additional metadata provided by previous AST factories</param>
     * <returns>The synthesized node, or null if the synthesis failed.</returns>
     */
    public abstract void Synthesize(TokenSet set, AstNode rootNode, ContextMetadata metadata);

    /**
     * The type of node this factory can synthesize.
     * This is used to determine which factory to use when synthesizing a node.
     */
    public abstract LexicalScope GetLexicalScope();

    /**
     * Abstraction for checking whether the next token can be consumed by this factory.
     * This is used to determine which factory to use when synthesizing a node.
     * <param name="nextToken">The next token to check.</param>
     * <param name="set">The current token set. This can be used for further token checking,
     * if the sequence is more complex.</param>
     */
    public abstract bool CanConsumeToken(Token nextToken, TokenSet set);
}
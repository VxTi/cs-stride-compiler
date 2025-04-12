using Stride.Common.Logging;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class LexicalScope
{
    public static TokenSet CreateSubsetForBlockScope(TokenSet set)
    {
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
        foreach (var subset in sublist)
            Logger.Info($"{subset}");
        return new TokenSet(set.SourceFilePath, sublist);
    }
}
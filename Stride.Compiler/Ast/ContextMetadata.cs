using Stride.Compiler.Generic;

namespace Stride.Compiler.Ast;

public class ContextMetadata
{
    public int Modifiers = 0;
    public LexicalScope CurrentScope = LexicalScope.Global;

    public ContextMetadata? ParentMeta;

    public ContextMetadata Create()
    {
        return new ContextMetadata()
        {
            ParentMeta = this
        };
    }
}
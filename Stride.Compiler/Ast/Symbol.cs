using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast;

public class Symbol(List<string> accessors)
{
    public override string ToString()
    {
        return string.Join(", ", accessors);
    }
}
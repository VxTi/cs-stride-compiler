namespace Stride.Compiler.Ast;

public interface IReducible<out TOut>
{
    TOut Reduce();
    
    bool IsReducible();
}
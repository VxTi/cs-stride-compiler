namespace Stride.Compiler.Exceptions;

public class IllegalTokenSequenceException : CompilationException
{
    public IllegalTokenSequenceException(Exception root) : base(root)
    {
    }

    public IllegalTokenSequenceException(string message) : base(message)
    {
    }

    public IllegalTokenSequenceException(string message, ErrorFragment[] fragments) : base(message, fragments)
    {
    }
}
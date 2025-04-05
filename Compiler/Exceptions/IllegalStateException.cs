namespace Compiler.Exceptions;

public class IllegalStateException(string error) : Exception(error)
{
}
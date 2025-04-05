namespace Stride.Compiler.Io;

public class IllegalFileTypeException(string fileName) : Exception(fileName);
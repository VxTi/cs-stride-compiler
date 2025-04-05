namespace Compiler.Io;

public class IllegalFileTypeException(string fileName) : Exception(fileName);
namespace StrideCompiler.Io;

public class IllegalFileTypeException(string fileName) : Exception(fileName);
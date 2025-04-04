namespace StrideCompiler.Exceptions;

public class ErrorFragment(
    string[] sourceLines,
    int sourceLineIndexOffset,
    int sourceIndexOffset,
    int sourceErrorLength,
    string message
)
{
    public string[] SourceLines = sourceLines;
    public int SourceLineIndexOffset = sourceLineIndexOffset;
    public int SourceIndexOffset = sourceIndexOffset;
    public int SourceErrorLength = sourceErrorLength;
    public string Message = message;
}
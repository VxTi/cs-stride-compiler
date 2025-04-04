namespace StrideCompiler.Exceptions;

using Logging;

public class CompilationException : Exception
{
    private readonly ErrorFragment[] _fragments = [];

    public CompilationException(Exception root) : base(root.Message, root.InnerException)
    {
    }

    public CompilationException(string message) : base(message)
    {
    }

    public CompilationException(string message, ErrorFragment[] fragments) : base(message)
    {
        _fragments = fragments;
    }

    private string ComposeErrorFragments()
    {
        if (_fragments.Length == 0)
            return "";

        const string Reset = "\u001b[0m";
        const string Red = "\u001b[31m";
        const string Yellow = "\u001b[33m";

        var result = new System.Text.StringBuilder();

        foreach (var fragment in _fragments)
        {
            result.Append(Red);
            result.AppendLine(fragment.Message);

            for (int i = 0; i < fragment.SourceLines.Length; i++)
            {
                var line = fragment.SourceLines[i];
                var startIdx = (i == 0) ? fragment.SourceIndexOffset : 0;
                var endIdx = (i == fragment.SourceLines.Length - 1)
                    ? startIdx + fragment.SourceErrorLength
                    : line.Length;

                // Append the line before the error in yellow
                result.Append(Yellow);
                result.Append(line[..startIdx]);

                // Append the error part in red
                result.Append(Red);
                result.Append(line.AsSpan(startIdx, endIdx - startIdx));

                result.Append(Yellow);
                result.Append(line.AsSpan(endIdx));

                result.Append(Reset);
                result.AppendLine();
            }

            result.Append(Reset);
        }

        return result.ToString();
    }

    public void Log()
    {
        if (_fragments.Length > 0)
            Logger.Log(ComposeErrorFragments());
        else Logger.Log($"\e[31m{Message}");
    }
}
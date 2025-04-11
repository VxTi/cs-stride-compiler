using System.CommandLine;

namespace Stride.CLI;

public interface ICommandFactory
{
    public void AppendCommandToParent(RootCommand command);
}
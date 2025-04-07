using System.CommandLine;

namespace Stride.CLI;

public abstract class AbstractCommandFactory
{
    public abstract Command CreateCommand();
}
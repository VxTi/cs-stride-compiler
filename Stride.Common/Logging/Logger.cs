namespace Stride.Common.Logging;

public static class Logger
{
    public static void Log(string message)
    {
        Console.WriteLine(message);
    }

    public static void Info(string message)
    {
        Log(LogLevel.Info, message);
    }

    public static void Warn(string message)
    {
        Log(LogLevel.Warn, message);
    }

    public static void Error(string message)
    {
        Log(LogLevel.Error, message);
    }

    public static void Debug(string message)
    {
        Log(LogLevel.Debug, message);
    }

    public static void Log(LogLevel level, string message, string prefix = "", string suffix = "")
    {
        var ansiPrefix = level switch
        {
            LogLevel.Debug => "\e[34m",
            LogLevel.Info => "",
            LogLevel.Warn => "\e[33m",
            LogLevel.Error => "\e[32m",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
        };

        Log($"{prefix}\e[34mStride \u25fe {ansiPrefix}{message}\e[0m{suffix}");
    }

    private const string Blue = "\u001b[34m";
    private const string Reset = "\u001b[0m";

    public static void Table(string[][] items)
    {
        if (items.Length == 0)
        {
            Console.WriteLine("No data to display.");
            return;
        }

        int colCount = items.Max(row => row.Length);

        // Compute column widths, with extra space for padding
        int[] colWidths = new int[colCount];
        for (int col = 0; col < colCount; col++)
        {
            colWidths[col] = items.Max(row =>
                row.Length > col ? (row[col] ?? string.Empty).Length : 0) + 2; // +2 for padding (1 space on each side)
        }

        Console.Write(Blue);

        // Top border
        Console.Write("┌");
        for (int col = 0; col < colCount; col++)
        {
            Console.Write(new string('─', colWidths[col]));
            Console.Write(col < colCount - 1 ? "┬" : "┐");
        }
        Console.WriteLine();

        for (int rowIndex = 0; rowIndex < items.Length; rowIndex++)
        {
            var row = items[rowIndex];

            // Table row
            Console.Write("│");
            for (int col = 0; col < colCount; col++)
            {
                string cell = col < row.Length ? row[col] ?? string.Empty : string.Empty;
                string padded = $" {cell} ".PadRight(colWidths[col]);
                Console.Write(padded);
                Console.Write("│");
            }
            Console.WriteLine();

            // Header separator
            if (rowIndex == 0)
            {
                Console.Write("├");
                for (int col = 0; col < colCount; col++)
                {
                    Console.Write(new string('─', colWidths[col]));
                    Console.Write(col < colCount - 1 ? "┼" : "┤");
                }
                Console.WriteLine();
            }
        }

        // Bottom border
        Console.Write("└");
        for (int col = 0; col < colCount; col++)
        {
            Console.Write(new string('─', colWidths[col]));
            Console.Write(col < colCount - 1 ? "┴" : "┘");
        }
        Console.WriteLine(Reset);
    }
}
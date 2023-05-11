using System.Diagnostics;

public class Logger
{
    public enum LogLevel
    {
        TRACE,
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }
    private static char[] DELIMITERS = { '[', ']' };

    private static string GetLevelName(LogLevel level)
    {
        switch (level)
        {
            case LogLevel.TRACE:
                return "TRACE";
            case LogLevel.DEBUG:
                return "DEBUG";
            case LogLevel.INFO:
                return "INFO";
            case LogLevel.WARNING:
                return "WARNING";
            case LogLevel.ERROR:
                return "ERROR";
            default:
                return "UNKNOWN";
        }
    }

    private static string GetHeader(LogLevel level)
    {
        var level_name = GetLevelName(level);
        var t = Process.GetCurrentProcess().Threads[0].TotalProcessorTime;

        StackTrace stackTrace = new StackTrace();
        var stackFrame = stackTrace.GetFrame(3)!;
        var methodName = stackFrame.GetMethod()?.Name;
        var className = stackFrame.GetMethod()?.DeclaringType?.Name;
        return $"{t.ToString()} " +
            $"{DELIMITERS[0]}{level_name}{DELIMITERS[1]}" +
            $"{DELIMITERS[0]}{className}.{methodName}{DELIMITERS[1]}";
    }

    public static void log(LogLevel level, string message)
    {
        Godot.GD.Print($"{GetHeader(level)} {message}");
    }

    public static void trace(string message)
    {
        log(LogLevel.TRACE, message);
    }

    public static void debug(string message)
    {
        log(LogLevel.DEBUG, message);
    }

    public static void info(string message)
    {
        log(LogLevel.INFO, message);
    }

    public static void warning(string message)
    {
        log(LogLevel.WARNING, message);
    }

    public static void error(string message)
    {
        log(LogLevel.ERROR, message);
    }
}
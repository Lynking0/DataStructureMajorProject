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

    private static string GetHeader(LogLevel level, object logee)
    {
        var level_name = GetLevelName(level);
        var t = Process.GetCurrentProcess().TotalProcessorTime;

#if DEBUG
        StackTrace stackTrace = new StackTrace();
        var stackFrame = stackTrace.GetFrame(3)!;
        var methodName = stackFrame.GetMethod()?.Name;
        var className = stackFrame.GetMethod()?.DeclaringType?.Name;
        return $"{t.ToString()} " +
            $"{DELIMITERS[0]}{level_name}{DELIMITERS[1]}" +
            $"{DELIMITERS[0]}{className}.{methodName}{DELIMITERS[1]}";

#else
        return $"{t.ToString()} " +
            $"{DELIMITERS[0]}{level_name}{DELIMITERS[1]}" +
            $"{DELIMITERS[0]}{(string)logee.GetType()}{DELIMITERS[1]}";
#endif

    }

    public static void log(object logee, LogLevel level, string message)
    {
        Godot.GD.Print($"{GetHeader(level, logee)} {message}");
    }

    public static void trace(object logee, string message)
    {
        log(logee, LogLevel.TRACE, message);
    }

    public static void debug(object logee, string message)
    {
        log(logee, LogLevel.DEBUG, message);
    }

    public static void info(object logee, string message)
    {
        log(logee, LogLevel.INFO, message);
    }

    public static void warning(object logee, string message)
    {
        log(logee, LogLevel.WARNING, message);
    }

    public static void error(object logee, string message)
    {
        log(logee, LogLevel.ERROR, message);
    }
}
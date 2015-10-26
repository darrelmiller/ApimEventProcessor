using System;

namespace ApimEventProcessor
{
    public enum LogLevel { Debug, Info, Warning, Error };

    public class ConsoleLogger : ILogger
    {
        private LogLevel _LogLevel;

        public ConsoleLogger(LogLevel logLevel = LogLevel.Info)
        {
            _LogLevel = logLevel;
        }
        public void LogDebug(string message, params object[] parameters)
        {
            if (_LogLevel > LogLevel.Debug) return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogInfo(string message, params object[] parameters)
        {
            if (_LogLevel > LogLevel.Info) return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogWarning(string message, params object[] parameters)
        {
            if (_LogLevel > LogLevel.Warning) return;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogError(string message, params object[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(string.Format(message, parameters));
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
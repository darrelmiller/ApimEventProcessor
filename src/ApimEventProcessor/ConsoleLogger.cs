using System;

namespace ApimEventProcessor
{
    public enum LogLevel { Debug, Info, Warning, Error };

    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel logLevel;
        private readonly object writerLock = new object();

        public ConsoleLogger(LogLevel logLevel = LogLevel.Info)
        {
            this.logLevel = logLevel;
        }

        public void LogDebug(string message, params object[] parameters)
        {
            if (logLevel > LogLevel.Debug) return;
            WriteLine(ConsoleColor.Green, message, parameters);
        }
     
        public void LogInfo(string message, params object[] parameters)
        {
            if (logLevel > LogLevel.Info) return;
            WriteLine(ConsoleColor.Yellow, message, parameters);
        }

        public void LogWarning(string message, params object[] parameters)
        {
            if (logLevel > LogLevel.Warning) return;
            WriteLine(ConsoleColor.Blue, message, parameters);
        }

        public void LogError(string message, params object[] parameters)
        {
            WriteLine(ConsoleColor.Magenta, message, parameters);
        }

        private void WriteLine(ConsoleColor color, string message, object[] parameters)
        {
            lock (writerLock)
            {
                var currentColor = Console.ForegroundColor;
                try
                {
                    currentColor = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.WriteLine(string.Format(message, parameters));
                }
                finally
                {
                    Console.ForegroundColor = currentColor;
                }
            }
        }

    }
}
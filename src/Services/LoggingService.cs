using System;
using System.IO;

namespace RitualOS.Services
{
    /// <summary>
    /// Simple logging service for writing timestamped messages to the
    /// <c>logs</c> directory under the application folder.
    /// </summary>
    public static class LoggingService
    {
        private static readonly object _lock = new();
        private static string LogDirectory => Path.Combine(AppContext.BaseDirectory, "logs");
        private static string LogFilePath => Path.Combine(LogDirectory, "app.log");

        /// <summary>
        /// Log an informational message.
        /// </summary>
        public static void Info(string message) => Write("INFO", message);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        public static void Warn(string message) => Write("WARN", message);

        /// <summary>
        /// Log an error message with optional exception details.
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            var detail = ex != null ? $"{message} :: {ex.Message}" : message;
            Write("ERROR", detail);
        }

        private static void Write(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    Directory.CreateDirectory(LogDirectory);
                    var line = $"{DateTime.Now:u} [{level}] {message}";
                    File.AppendAllLines(LogFilePath, new[] { line });
                }
            }
            catch
            {
                // swallow logging failures
            }
        }
    }
}

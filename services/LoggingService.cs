using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RitualOS.Services
{
    /// <summary>
    /// Simple logging service for writing timestamped messages to the <c>logs</c> directory under the application folder.
    /// </summary>
    public static class LoggingService
    {
        private static readonly object _lock = new();
        private static readonly SemaphoreSlim _sem = new(1, 1);
        private static string LogDirectory => Path.Combine(AppContext.BaseDirectory, "logs");
        private static string LogFilePath => Path.Combine(LogDirectory, "app.log");

        /// <summary>
        /// Log an informational message.
        /// </summary>
        public static void Info(string message) => Write("INFO", message);

        /// <summary>
        /// Log an informational message asynchronously. ✅
        /// </summary>
        public static Task InfoAsync(string message) => WriteAsync("INFO", message);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        public static void Warn(string message) => Write("WARN", message);

        /// <summary>
        /// Log a warning message asynchronously. ✅
        /// </summary>
        public static Task WarnAsync(string message) => WriteAsync("WARN", message);

        /// <summary>
        /// Log an error message with optional exception details.
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            var detail = ex != null ? $"{message} :: {ex.Message}" : message;
            Write("ERROR", detail);
        }

        /// <summary>
        /// Log an error message asynchronously. ✅
        /// </summary>
        public static Task ErrorAsync(string message, Exception? ex = null)
        {
            var detail = ex != null ? $"{message} :: {ex.Message}" : message;
            return WriteAsync("ERROR", detail);
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

        private static async Task WriteAsync(string level, string message)
        {
            try
            {
                await _sem.WaitAsync();
                Directory.CreateDirectory(LogDirectory);
                var line = $"{DateTime.Now:u} [{level}] {message}";
                await File.AppendAllLinesAsync(LogFilePath, new[] { line });
            }
            catch
            {
                // swallow logging failures
            }
            finally
            {
                _sem.Release();
            }
        }
    }
}
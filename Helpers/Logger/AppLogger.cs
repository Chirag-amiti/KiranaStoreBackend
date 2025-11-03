using System;
using System.IO;

namespace KiranaStore.Helpers.Logger
{
    public static class AppLogger
    {
        private static readonly string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "app.log");

        static AppLogger()
        {
            // Ensure Logs folder exists
            var logDir = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }

        // Info level logging
        public static void Info(string message)
        {
            Log("INFO", message);
        }

        // Warning level logging
        public static void Warning(string message)
        {
            Log("WARNING", message);
        }

        // Error level logging
        public static void Error(string message)
        {
            Log("ERROR", message);
        }

        // Core log function
        private static void Log(string level, string message)
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            Console.WriteLine(logMessage); // log to console
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine); // log to file
        }
    }
}

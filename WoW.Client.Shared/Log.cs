using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    /// <summary>
    /// Handles detailed logging for all WPP projects.
    /// </summary>
    public static class Log
    {
        private static bool _isLoggingDebug = true;
        private static bool _isLoggingToFile = true;
        private static bool _isLoggingToConsole = true;

        public static void Print(object obj, LogType _logType)
        {
            StringBuilder fullLog = new StringBuilder();

            fullLog.Append("Log: ");
            fullLog.Append(PrintTime());

            string logType = "";
            switch (_logType)
            {
                case LogType.Debug: logType = "Debug | "; break;
                case LogType.Error: logType = "Error | "; break;
                case LogType.Warning: logType = "Warning | "; break;
                case LogType.Exception: logType = "Exception | "; break;
                case LogType.Process: logType = $"Process | "; break;
                case LogType.Network: logType = $"Network | "; break;
            }

            fullLog.Append(logType);
            fullLog.Append(obj.ToString());

            Console.WriteLine(fullLog.ToString());
        }

        private static string PrintTime() => $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | ";

        public static void SetLoggingDebug(bool isLoggingDebug) => _isLoggingDebug = isLoggingDebug;
        public static void SetLoggingToFile(bool isLoggingToFile) => _isLoggingToFile = isLoggingToFile;
        public static void SetLoggingToConsole(bool isLoggingToConsole) => _isLoggingToFile = isLoggingToConsole;
    }

    public enum LogType
    {
        Debug,
        Error,
        Warning,
        Exception,
        Network,
        Process
    }
}

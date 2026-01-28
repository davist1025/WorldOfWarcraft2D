using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WoW.Framework.Utils;

namespace WoW.Framework.Logging
{
    public static class Logger
    {
        private static bool _isLoggingDebug = true;
        private static bool _isLoggingToFile = true;
        private static bool _isLoggingToConsole = true;

        public static void Print(object obj, LogEntryType _logType)
        {
            StringBuilder fullLog = new StringBuilder();

            fullLog.Append("Log: ");
            fullLog.Append(PrintTime());

            string logType = "";
            switch (_logType)
            {
                case LogEntryType.Debug: logType = "Debug | "; break;
                case LogEntryType.Error: logType = "Error | "; break;
                case LogEntryType.Warning: logType = "Warning | "; break;
                case LogEntryType.Fatal: logType = "Exception | "; break;
                case LogEntryType.Process: logType = $"Process | "; break;
                case LogEntryType.Network: logType = $"Network | "; break;
            }

            fullLog.Append(logType);
            fullLog.Append(obj.ToString());

            Console.WriteLine(fullLog.ToString());
        }

        private static string PrintTime() => $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} | ";

    }
}

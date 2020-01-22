using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    public class ConsoleLogger : IStatusLogger
    {
        public ConsoleLogger(int level)
        {
            LogLevel = level;
        }

        public int LogLevel { get; set; }

        public void Line(int level)
        {
            Log(level, "");
        }

        public void Log(int level, string status, params object[] args)
        {
            if (LogLevel >= level)
            {
                Console.WriteLine(status, args);
            }
        }

        public void LogTimed(int level, string status, params object[] args)
        {
            Log(level, $"{DateTime.Now:yyyy-MM-dd}: {status}", args);
        }
    }
}

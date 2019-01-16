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
    }
}

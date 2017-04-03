using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

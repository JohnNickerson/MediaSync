using System;
using System.Runtime.Remoting.Channels;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    [Obsolete("Use plain trace and debug instead.")]
    public interface IStatusLogger
    {
        int LogLevel { get; set; }

        void Log(int level, string status, params object[] args);

        void LogTimed(int level, string status, params object[] args);

        /// <summary>
        /// Shortcut for logging a newline, in whatever form that takes.
        /// </summary>
        /// <param name="level">The logging level at which this should display.</param>
        /// <remarks>Should be equivalent to 'Log(level, "")'.</remarks>
        void Line(int level);
    }
}

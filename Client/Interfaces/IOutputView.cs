using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;

namespace AssimilationSoftware.MediaSync.Interfaces
{
    /// <summary>
    /// A generic interface for sync operation output.
    /// </summary>
    public interface IOutputView
    {
        void WriteLine(string format, params object[] args);

        void Report(SyncOperation op);

        string Status
        {
            set;
        }
    }
}

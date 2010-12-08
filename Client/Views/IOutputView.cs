using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Views
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

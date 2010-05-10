using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    /// <summary>
    /// A generic interface for sync operation output.
    /// </summary>
    public interface IOutputView
    {
        void WriteLine(string format, params object[] args);
    }
}

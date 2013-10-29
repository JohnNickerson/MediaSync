using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IFileHashProvider
    {
        string ComputeHash(string filename);
    }
}

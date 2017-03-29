using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.FileManagement.Hashing
{
    /// <summary>
    /// http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
    /// </summary>
    public class MurMur3Hahser : IFileHashProvider
    {
        public string ComputeHash(string filename)
        {
            throw new NotImplementedException();
        }
    }
}

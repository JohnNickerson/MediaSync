using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileRevision
    {

        /// <summary>
        /// The size of the file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// A hash of the contents, to compare quickly with others.
        /// </summary>
        public string ContentsHash { get; set; }

        /// <summary>
        /// A version number. Helps track conflicting edits.
        /// </summary>
        public int Revision { get; set; }
    }
}

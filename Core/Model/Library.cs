using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Library : EntityBase
    {
        public string Name { get; set; }
        public int LibraryId { get; set; }
        public FileIndex PrimaryIndex { get; set; }
        public ulong MaxSharedSize { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileIndex
    {
        public FileIndex()
        {
            Files = new List<FileHeader>();
        }

        public int Id { get; set; }
        public string ProfileName { get; set; }
        public Repository Participant { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<FileHeader> Files { get; set; }
    }
}

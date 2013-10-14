using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Model
{
    public class FileIndex
    {
        public FileIndex()
        {
            Files = new List<FileHeader>();
        }

        public string MachineName { get; set; }
        public string ProfileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string LocalBasePath { get; set; }
        public List<FileHeader> Files { get; set; }
    }
}

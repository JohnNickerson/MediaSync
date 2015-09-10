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

        public string MachineName { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<FileHeader> Files { get; set; }
        public string LocalPath { get; set; }
        public string SharedPath { get; set; }
        public bool IsPull { get; set; }
        public bool IsPush { get; set; }
        public string ProfileName { get; internal set; }
        public bool IsMaster { get; set; }
    }
}

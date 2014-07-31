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

        public int Id { get; set; }
        public SyncProfile Profile { get; set; }
        public ProfileParticipant Participant { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<FileHeader> Files { get; set; }
    }
}

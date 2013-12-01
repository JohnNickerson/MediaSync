using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    abstract class FileAction
    {
        public Guid ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string MachineName { get; set; }
        public string ProfileName { get; set; }

        public abstract void Replay();
    }
}

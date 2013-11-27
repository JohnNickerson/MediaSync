using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public abstract class FileAction
    {
        public FileAction(MediaSync.Model.SyncProfile profile)
        {
            ProfileName = profile.ProfileName;
            MachineName = Settings.Default.MachineName;
            Timestamp = DateTime.Now;
            ID = Guid.NewGuid();
        }

        public Guid ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string MachineName { get; set; }
        public string ProfileName { get; set; }

        public abstract void Replay();
    }
}

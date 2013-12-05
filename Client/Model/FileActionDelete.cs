using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    /// <summary>
    /// Prunes a file from shared storage after it has served its purpose.
    /// </summary>
    public class FileActionDelete : FileAction
    {
        public string SharedPath { get; set; }
        public string FileName { get; set; }

        public FileActionDelete(SyncProfile profile, string filename) : base(profile)
        {
            SharedPath = profile.GetParticipant(Settings.Default.MachineName).SharedPath;
            FileName = filename;
        }

        public override void Replay()
        {
            File.Delete(Path.Combine(SharedPath, FileName));
        }
    }
}

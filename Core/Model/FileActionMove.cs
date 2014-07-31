using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileActionMove : FileAction
    {
        public string FileName { get; set; }
        private string sharedpath;
        private string localpath;

        public FileActionMove(SyncProfile profile, string filename) : base(profile)
        {
            sharedpath = profile.GetParticipant(MachineName).SharedPath;
            localpath = profile.GetParticipant(MachineName).LocalPath;
            FileName = filename;
        }

        public override void Replay()
        {
            string sharedfilename = Path.Combine(sharedpath, FileName);
            string localfilename = Path.Combine(localpath, FileName);

            File.Copy(sharedfilename, localfilename);
        }
    }
}

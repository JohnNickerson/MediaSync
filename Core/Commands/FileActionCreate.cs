﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    class FileActionCreate : FileAction
    {
        public string FileName { get; set; }
        private string sharedpath;
        private string localpath;

        public FileActionCreate(SyncProfile profile, string filename, string machine)
        {
            sharedpath = profile.GetParticipant(machine).SharedPath;
            localpath = profile.GetParticipant(machine).LocalPath;
            FileName = filename;
        }

        public void Replay()
        {
            string sharedfilename = Path.Combine(sharedpath, FileName);
            string localfilename = Path.Combine(localpath, FileName);

            System.IO.File.Copy(sharedfilename, localfilename);
        }
    }
}

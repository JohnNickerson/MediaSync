using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileActionUpdate : FileAction
    {
        public FileActionUpdate(SyncProfile profile)
            : base(profile)
        {
        }

        public override void Replay()
        {
            throw new NotImplementedException();
        }
    }
}

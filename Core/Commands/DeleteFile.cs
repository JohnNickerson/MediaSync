using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class DeleteFile : FileCommand
    {
        public string Path;

        public DeleteFile(string dir)
        {
            this.Path = dir;
        }

        public override void Replay()
        {
            if (Directory.Exists(Path))
            {
                // We are dealing with a directory.
                Directory.Delete(Path);
            }
            else if (File.Exists(Path))
            {
                // Just a file.
                File.Delete(Path);
            }
        }
    }
}

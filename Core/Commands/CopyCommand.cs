using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class CopyCommand : FileCommand
    {
        public string Source;
        public string Target;

        public CopyCommand(string source, string target)
        {
            this.Source = source;
            this.Target = target;
        }

        public override void Replay()
        {
            // Ensure the target folder exists.
            var targetfolder = new FileInfo(Target).Directory.FullName;
            if (!Directory.Exists(targetfolder))
            {
                Directory.CreateDirectory(targetfolder);
            }
            File.Copy(Source, Target, true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class MoveCommand : FileCommand
    {
        public string Source;
        public string Target;

        public MoveCommand(string source, string target)
        {
            this.Source = source;
            this.Target = target;
        }

        public override void Replay()
        {
            File.Move(Source, Target);
        }
    }
}

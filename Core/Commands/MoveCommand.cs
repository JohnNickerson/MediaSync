using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class MoveCommand : FileCommand
    {
        public string Source;
        public string Target;

        public MoveCommand(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public override void Replay()
        {
            File.Move(Source, Target);
        }
    }
}

using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class CopyCommand : FileCommand
    {
        public string Source;
        public string Target;

        public CopyCommand(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public override void Replay()
        {
            // Ensure the target folder exists.
            var directoryInfo = new FileInfo(Target).Directory;
            if (directoryInfo != null)
            {
                var targetfolder = directoryInfo.FullName;
                if (!Directory.Exists(targetfolder))
                {
                    Directory.CreateDirectory(targetfolder);
                }
            }

            File.Copy(Source, Target, true);
        }
    }
}

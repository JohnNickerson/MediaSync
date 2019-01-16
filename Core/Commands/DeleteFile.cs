using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Commands
{
    public class DeleteFile : FileCommand
    {
        public string Path;

        public DeleteFile(string dir)
        {
            Path = dir;
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
                try
                {
                    File.Delete(Path);
                }
                catch (IOException)
                {
                    // TODO: Report failure somehow.
                }
            }
        }
    }
}

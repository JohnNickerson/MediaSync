using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class SplitPath
    {
        public string BasePath { get; set; }
        public string RelativePath { get; set; }

        public string FileName => new FileInfo(Path.Combine(BasePath, RelativePath)).Name;
    }
}

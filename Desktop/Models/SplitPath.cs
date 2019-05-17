using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class SplitPath
    {
        public SplitPath(string basePath, string relativePath)
        {
            BasePath = basePath;
            RelativePath = relativePath;
        }

        public SplitPath(string fullPath, int basePathLength)
        {
            BasePath = fullPath.Substring(0, basePathLength);
            RelativePath = fullPath.Substring(basePathLength);

            if (BasePath.EndsWith(Path.PathSeparator.ToString()))
            {
                BasePath = BasePath.Substring(0, BasePath.Length - 1);
            }

            if (RelativePath.StartsWith(Path.PathSeparator.ToString()))
            {
                RelativePath = RelativePath.Substring(1);
            }
        }

        public string BasePath { get; set; }
        public string RelativePath { get; set; }
        public string FullPath => Path.Combine(BasePath, RelativePath);
    }
}

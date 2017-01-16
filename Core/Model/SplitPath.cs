using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class SplitPath
    {
        public string BasePath { get; set; }
        public string RelativePath { get; set; }

        public string FileName
        {
            get
            {
                return new FileInfo(Path.Combine(BasePath, RelativePath)).Name;
            }
        }
    }
}

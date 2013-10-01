using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Model
{
    public class FileHeader
    {
        public string RelativePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentsHash { get; set; }
    }
}

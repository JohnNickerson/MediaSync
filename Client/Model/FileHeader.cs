using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AssimilationSoftware.MediaSync.Model
{
    public class FileHeader
    {
        public FileHeader(string filename)
        {
            var fileinfo = new FileInfo(filename);

            this.FileName = fileinfo.Name;
            this.FileSize = fileinfo.Length;
        }

        public string RelativePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentsHash { get; set; }
    }
}

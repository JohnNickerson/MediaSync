﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AssimilationSoftware.MediaSync.Model
{
    public class FileHeader
    {
        public FileHeader(string filename, string basepath)
        {
            var fileinfo = new FileInfo(Path.Combine(basepath, filename));

            this.FileName = fileinfo.Name;
            this.FileSize = fileinfo.Length;
            this.RelativePath = filename.Substring(0, filename.Length - fileinfo.Name.Length);
        }

        public string RelativePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentsHash { get; set; }
    }
}

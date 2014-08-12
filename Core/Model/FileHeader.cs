using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileHeader
    {
        #region Constructors
        public FileHeader(string filename, string basepath, IFileHashProvider hasher)
        {
            var fileinfo = new FileInfo(Path.Combine(basepath, filename));

            this.FileName = fileinfo.Name;
            this.FileSize = fileinfo.Length;
            this.RelativePath = filename.Substring(0, filename.Length - fileinfo.Name.Length);
            this.ContentsHash = hasher.ComputeHash(fileinfo.FullName);
        }

        public FileHeader()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// File ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The path of the file, relative to the local base path.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The size of the file.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// A hash of the contents, to compare quickly with others.
        /// </summary>
        public string ContentsHash { get; set; }
        #endregion
    }
}

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

        /// <summary>
        /// A version number. Helps track conflicting edits.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// A flag to indicate that the file was deleted.
        /// </summary>
        /// <remarks>
        /// Used only in the master index. Records with this flag that do not exist in any satellite index should be removed.
        /// </remarks>
        public bool IsDeleted { get; set; }
        #endregion
    }
}

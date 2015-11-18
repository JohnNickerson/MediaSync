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
        /// A flag to indicate that the file was deleted.
        /// </summary>
        /// <remarks>
        /// Used only in the master index. Records with this flag that do not exist in any satellite index should be removed.
        /// </remarks>
        public bool IsDeleted { get; set; }

        public long Size { get; set; }

        public string ContentsHash { get; set; }

        public DateTime LastModified { get; set; }

        internal FileHeader Clone()
        {
            return new FileHeader
            {
                ContentsHash = ContentsHash,
                FileName = FileName,
                IsDeleted = IsDeleted,
                LastModified = LastModified,
                RelativePath = RelativePath,
                Size = Size
            };
        }
        #endregion
    }
}

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

        /// <summary>
        /// The size of the file, in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// A hash of the file contents, used to determine if changes have been made.
        /// </summary>
        public string ContentsHash { get; set; }

        /// <summary>
        /// The date and time when the file was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets a copy of the file's metadata in a new object.
        /// </summary>
        /// <returns>A FileHeader just like this one.</returns>
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

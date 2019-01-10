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
        #region Fields
        private readonly IFileHashProvider _hasher;
        private string _contentsHash;
        #endregion

        #region Constructors
        public FileHeader(IFileHashProvider hasher = null)
        {
            _hasher = hasher;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The local base path, where the file is stored.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// The path of the file, relative to the local base path.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName => new FileInfo(Path.Combine(BasePath, RelativePath)).Name;

        /// <summary>
        /// A flag to indicate that the file was deleted.
        /// </summary>
        /// <remarks>
        /// Used only in the master index. Records with this flag that do not exist in any satellite index should be removed.
        /// </remarks>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Indicates whether this is a folder.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// The size of the file, in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// A hash of the file contents, used to determine if changes have been made.
        /// </summary>
        public string ContentsHash
        {
            get => _contentsHash ?? (_contentsHash = _hasher?.ComputeHash(Path.Combine(BasePath, RelativePath)));
            set => _contentsHash = value;
        }

        /// <summary>
        /// A synchronisation state, used for the master index.
        /// </summary>
        public FileSyncState State { get; set; }

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
                IsDeleted = IsDeleted,
                LastModified = LastModified,
                RelativePath = RelativePath,
                Size = Size,
                BasePath = BasePath,
                State = State,
                IsFolder = IsFolder
            };
        }
        #endregion
    }

    /// <summary>
    /// Possible file synchronisation states.
    /// </summary>
    public enum FileSyncState
    {
        Synchronised,
        Transit,
        Expiring,
        Destroyed
    }
}

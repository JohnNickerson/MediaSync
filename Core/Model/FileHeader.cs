using System;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileHeader : FileSystemEntry
    {
        #region Fields
        private readonly IFileHashProvider _hasher;

        #endregion

        #region Constructors
        public FileHeader() : this(null) { }

        public FileHeader(IFileHashProvider hasher)
        {
            _hasher = hasher;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName => new FileInfo(Path.Combine(BasePath, RelativePath)).Name;

        /// <summary>
        /// The date and time when the file was last modified.
        /// </summary>
        public DateTime LastWriteTime { get; set; }
        #endregion

        #region Methods

        public override bool Matches(FileSystemEntry shareFileHead)
        {
            if (!(shareFileHead is FileHeader otherFile)) return false;
            return base.Matches(otherFile);
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

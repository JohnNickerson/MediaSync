using System.IO;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class FileHeader : Maroon.Model.ModelObject
    {
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
        /// Used only in the primary index. Records with this flag that do not exist in any satellite index should be removed.
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
        public string ContentsHash { get; set; }

        /// <summary>
        /// A synchronisation state, used for the primary index.
        /// </summary>
        public FileSyncState State { get; set; }

        /// <summary>
        /// Gets a copy of the file's metadata in a new object.
        /// </summary>
        /// <returns>A FileHeader just like this one.</returns>
        public override object Clone()
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
                IsFolder = IsFolder,
                ID = ID,
                RevisionGuid = RevisionGuid,
                Revision = Revision
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

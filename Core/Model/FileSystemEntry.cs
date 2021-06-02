using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public abstract class FileSystemEntry : Maroon.Model.ModelObject
    {
        public Guid IndexId { get; set; }

        /// <summary>
        /// The path of the file, relative to the local base path.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Indicates the state of the file on disk.
        /// </summary>
        /// <remarks>
        /// Not to be confused with the "IsDeleted" property that refers instead to the deleted state of this record.
        /// </remarks>
        public FileSyncState State { get; set; }

        /// <summary>
        /// A hash of the file contents, used to determine if changes have been made.
        /// </summary>
        public string ContentsHash { get; set; }

        /// <summary>
        /// The size of the file, in bytes.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Compares this file system entry to another, and returns true if they seem to represent copies of each other.
        /// </summary>
        /// <param name="otherFile">The file to compare to.</param>
        /// <returns></returns>
        public virtual bool Matches(FileSystemEntry otherFile)
        {
            return Size == otherFile.Size && ContentsHash == otherFile.ContentsHash;
        }
    }
}

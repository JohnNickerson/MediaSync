using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileIndex
    {
        public FileIndex()
        {
            Files = new List<FileHeader>();
        }

        /// <summary>
        /// The name of the machine to which this index belongs, if any.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// The date and time at which the index was last updated.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The actual files that make up the index.
        /// </summary>
        public List<FileHeader> Files { get; set; }

        /// <summary>
        /// The path on the local machine where the repository is stored.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// The path, on the local machine, where shared storage for file transfers is accessed.
        /// </summary>
        public string SharedPath { get; set; }

        /// <summary>
        /// Indicates whether this machine, for this index, replicates file changes from remote repositories.
        /// </summary>
        public bool IsPull { get; set; }

        /// <summary>
        /// Indeicates whether this machine, for this index, replicates its own changes for other repositories to copy.
        /// </summary>
        public bool IsPush { get; set; }

        internal FileHeader GetFile(string relativePath)
        {
            return Files.FirstOrDefault(f => f.RelativePath.ToLower() == relativePath.ToLower());
        }

        internal void UpdateFile(FileHeader fileHeader)
        {
            if (fileHeader != null)
            {
                Files.RemoveAll(f => f.RelativePath.ToLower() == fileHeader.RelativePath.ToLower());
                Files.Add(fileHeader);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileIndex
    {
        private Dictionary<string, FileHeader> _files;

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
        public Dictionary<string, FileHeader> Files
        {
            get => _files ?? (_files = new Dictionary<string, FileHeader>(StringComparer.CurrentCultureIgnoreCase));
            set => _files = value;
        }

        /// <summary>
        /// The path on the local machine where the repository is stored.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// The path, on the local machine, where shared storage for file transfers is accessed.
        /// </summary>
        public string SharedPath { get; set; }

        public FileHeader GetFile(string relativePath)
        {
            return Files.ContainsKey(relativePath) ? Files[relativePath] : null;
        }

        public void UpdateFile(FileHeader fileHeader)
        {
            if (fileHeader == null) return;
            Files[fileHeader.RelativePath] = fileHeader.Clone();
        }

        public void Remove(FileHeader localIndexFile)
        {
            if (Exists(localIndexFile.RelativePath))
            {
                Files.Remove(localIndexFile.RelativePath);
            }
        }

        public bool Exists(string relativepath)
        {
            return Files.ContainsKey(relativepath);
        }

        public bool MatchesFile(FileHeader file)
        {
            if (file == null)
                return false;

            if (!Exists(file.RelativePath))
                return false;

            var indexfile = GetFile(file.RelativePath);
            return file.IsFolder == indexfile.IsFolder && file.Size == indexfile.Size && file.ContentsHash == indexfile.ContentsHash;
        }
    }
}

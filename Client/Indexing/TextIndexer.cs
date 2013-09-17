using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Indexing
{
    /// <summary>
    /// An index class that writes to a text file.
    /// </summary>
    public class TextIndexer : IIndexService
    {
        #region Fields
        /// <summary>
        /// The sync profile to work from.
        /// </summary>
        private SyncProfile _options;

        /// <summary>
        /// The current list of files.
        /// </summary>
        private List<string> _fileList;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new text indexer for a given sync profile.
        /// </summary>
        /// <param name="_options">The sync profile to work from.</param>
        public TextIndexer(SyncProfile _options)
        {
            this._options = _options;
            _fileList = new List<string>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a file to the index.
        /// </summary>
        /// <param name="trunc_file">The file name to add to the index.</param>
        void IIndexService.Add(string trunc_file)
        {
            _fileList.Add(trunc_file);
        }

        /// <summary>
        /// Writes the index out to a file.
        /// </summary>
        void IIndexService.WriteIndex()
        {
            File.WriteAllLines(IndexFileName, _fileList.ToArray());
        }

        /// <summary>
        /// Compares this index to all the other indices on disk.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        Dictionary<string, int> IIndexService.CompareCounts()
        {
            var FileCounts = new Dictionary<string, int>();
            foreach (string otherindex in Directory.GetFiles(_options.SharedPath, "*_index.txt"))
            {
                if (!otherindex.Equals(IndexFileName))
                {
                    foreach (string idxfilename in File.ReadAllLines(otherindex))
                    {
                        if (FileCounts.ContainsKey(idxfilename))
                        {
                            FileCounts[idxfilename]++;
                        }
                        else
                        {
                            FileCounts[idxfilename] = 1;
                        }
                    }
                }
            }
            return FileCounts;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of peers participating in this sync profile, based on index files on disk.
        /// </summary>
        int IIndexService.PeerCount
        {
            get
            {
                return Directory.GetFiles(_options.SharedPath, "*_index.txt").Length;
            }
        }

        /// <summary>
        /// Gets the name of the file to which the index will be written.
        /// </summary>
        private string IndexFileName
        {
            get
            {
                string indexfile = Path.Combine(_options.SharedPath, string.Format("{0}_index.txt", Settings.Default.MachineName));
                return indexfile;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Indexing
{
    public interface IIndexService
    {
        /// <summary>
        /// Adds a file to the index.
        /// </summary>
        /// <param name="trunc_file">The file name to add to the index.</param>
        void Add(string trunc_file);

        /// <summary>
        /// Persists the index in storage.
        /// </summary>
        void WriteIndex();

        /// <summary>
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        Dictionary<string, int> CompareCounts();

        void CreateIndex(IFileManager file_manager);

        /// <summary>
        /// Gets the number of peers participating in this sync profile.
        /// </summary>
        int PeerCount { get; }
    }
}

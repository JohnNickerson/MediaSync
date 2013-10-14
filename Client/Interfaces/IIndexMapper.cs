﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;

namespace AssimilationSoftware.MediaSync.Interfaces
{
    public interface IIndexMapper
    {
        void Save(FileIndex index);

        FileIndex LoadLatest(string machine, string profile);

        /// <summary>
        /// Adds a file to the index.
        /// </summary>
        /// <param name="filename">The file name to add to the index.</param>
        void Add(string trunc_file);

        /// <summary>
        /// Persists the index in storage.
        /// </summary>
        [Obsolete("Use Save() instead.")]
        void WriteIndex();

        /// <summary>
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        Dictionary<string, int> CompareCounts();

        void CreateIndex(IFileManager file_manager);
    }
}

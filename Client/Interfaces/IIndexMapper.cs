using System;
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
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        Dictionary<string, int> CompareCounts();

        /// <summary>
        /// Loads all indexes for all profiles and all machines.
        /// </summary>
        /// <returns></returns>
        List<FileIndex> LoadAll();

        List<FileIndex> Load(SyncProfile profile);

        List<FileIndex> Load(string machine);

        List<FileIndex> Load(string machine, SyncProfile profile);
    }
}

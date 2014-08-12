using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IIndexMapper
    {
        void Save(FileIndex index);

        FileIndex LoadLatest(string machine, string profile);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace AssimilationSoftware.MediaSync.Mappers.Mock
{
    public class MockIndexMapper : IIndexMapper
    {
        Dictionary<string, int> IIndexMapper.CompareCounts(SyncProfile profile)
        {
            return new Dictionary<string, int>();
        }

        public void CreateIndex(IFileManager file_manager)
        {
        }

        void IIndexMapper.Save(FileIndex index)
        {
            throw new NotImplementedException();
        }

        FileIndex IIndexMapper.LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }


        public List<FileIndex> LoadAll()
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}

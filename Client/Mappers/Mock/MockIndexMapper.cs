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
        void IIndexMapper.Add(string trunc_file)
        {
        }

        void IIndexMapper.WriteIndex()
        {
        }

        Dictionary<string, int> IIndexMapper.CompareCounts()
        {
            return new Dictionary<string, int>();
        }

        public void CreateIndex(IFileManager file_manager)
        {
        }

        int IIndexMapper.PeerCount
        {
            get
            {
                return 1;
            }
        }

        void IIndexMapper.Save(FileIndex index)
        {
            throw new NotImplementedException();
        }

        FileIndex IIndexMapper.LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }

        int IIndexMapper.NumPeers(string profile)
        {
            throw new NotImplementedException();
        }

        void IIndexMapper.CreateIndex(IFileManager file_manager)
        {
            throw new NotImplementedException();
        }
    }
}

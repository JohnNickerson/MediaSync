using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace UnitTests.Mocks
{
    class ConsoleIndexer : IIndexMapper
    {
        private List<string> _files = new List<string>();

        void IIndexMapper.Add(string trunc_file)
        {
            _files.Add(trunc_file);
        }

        void IIndexMapper.WriteIndex()
        {
            foreach (string file in _files)
            {
                Console.WriteLine(file);
            }
        }

        int IIndexMapper.PeerCount
        {
            get { return 5; }
        }

        Dictionary<string, int> IIndexMapper.CompareCounts()
        {
            var result = new Dictionary<string, int>();

            for (int x = 1; x <= 10; x++)
            {
                result[string.Format("file{0}", x)] = x / 2;
            }

            return result;
        }

        void IIndexMapper.CreateIndex(IFileManager file_manager)
        {
            throw new NotImplementedException();
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
    }
}

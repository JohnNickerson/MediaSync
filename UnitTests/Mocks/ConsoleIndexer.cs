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

        Dictionary<string, int> IIndexMapper.CompareCounts(SyncProfile profile)
        {
            var result = new Dictionary<string, int>();

            for (int x = 1; x <= 10; x++)
            {
                result[string.Format("file{0}", x)] = x / 2;
            }

            return result;
        }

        void IIndexMapper.Save(FileIndex index)
        {
            foreach (FileHeader file in index.Files)
            {
                Console.WriteLine(file.FileName);
            }
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

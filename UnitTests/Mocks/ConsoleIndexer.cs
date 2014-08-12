using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.UnitTests.Mocks
{
    class ConsoleIndexer : IIndexMapper
    {
        private List<string> _files = new List<string>();

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core.Indexing;

namespace UnitTests.Mocks
{
    class ConsoleIndexer : IIndexService
    {
        private List<string> _files = new List<string>();

        void IIndexService.Add(string trunc_file)
        {
            _files.Add(trunc_file);
        }

        void IIndexService.WriteIndex()
        {
            foreach (string file in _files)
            {
                Console.WriteLine(file);
            }
        }

        int IIndexService.PeerCount
        {
            get { return 5; }
        }

        Dictionary<string, int> IIndexService.CompareCounts()
        {
            var result = new Dictionary<string, int>();

            for (int x = 1; x <= 10; x++)
            {
                result[string.Format("file{0}", x)] = x / 2;
            }

            return result;
        }
    }
}

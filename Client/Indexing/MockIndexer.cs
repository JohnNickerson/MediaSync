using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Indexing
{
    public class MockIndexer : IIndexService
    {
        void IIndexService.Add(string trunc_file)
        {
        }

        void IIndexService.WriteIndex()
        {
        }

        int IIndexService.PeerCount
        {
            get
            {
                return 1;
            }
        }

        Dictionary<string, int> IIndexService.CompareCounts()
        {
            return new Dictionary<string, int>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    interface IIndexService
    {
        void Add(string trunc_file);

        void WriteIndex();

        int PeerCount { get; }

        Dictionary<string, int> CompareCounts();
    }
}

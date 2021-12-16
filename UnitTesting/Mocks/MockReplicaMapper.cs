using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockReplicaMapper : IDiskMapper<Replica>
    {
        private List<Replica> _items;

        public IEnumerable<Replica> Read(params string[] fileNames)
        {
            return _items ?? (_items = new List<Replica>());
        }

        public void Write(IEnumerable<Replica> items, string filename)
        {
            _items = items.ToList();
        }

        public void Delete(string filename)
        {
            _items = new List<Replica>();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockIndexMapper : IDiskMapper<FileIndex>
    {
        private List<FileIndex> _items;

        public IEnumerable<FileIndex> Read(params string[] fileNames)
        {
            return _items ?? (_items = new List<FileIndex>());
        }

        public void Write(IEnumerable<FileIndex> items, string filename)
        {
            _items = items.ToList();
        }

        public void Delete(string filename)
        {
            _items = new List<FileIndex>();
        }
    }
}
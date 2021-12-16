using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockLibraryMapper : IDiskMapper<Library>
    {
        private List<Library> _items;

        public IEnumerable<Library> Read(params string[] fileNames)
        {
            return _items ?? (_items = new List<Library>());
        }

        public void Write(IEnumerable<Library> items, string filename)
        {
            _items = items.ToList();
        }

        public void Delete(string filename)
        {
            _items = new List<Library>();
        }
    }
}
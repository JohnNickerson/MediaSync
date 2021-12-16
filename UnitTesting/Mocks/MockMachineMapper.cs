using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockMachineMapper : IDiskMapper<Machine>
    {
        private List<Machine> _items;

        public IEnumerable<Machine> Read(params string[] fileNames)
        {
            return _items ?? (_items = new List<Machine>());
        }

        public void Write(IEnumerable<Machine> items, string filename)
        {
            _items = items.ToList();
        }

        public void Delete(string filename)
        {
            _items = new List<Machine>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockFileMapper : IDiskMapper<FileSystemEntry>
    {
        private List<FileSystemEntry> _items;

        public IEnumerable<FileSystemEntry> Read(params string[] fileNames)
        {
            return _items ?? (_items = new List<FileSystemEntry>());
        }

        public void Write(IEnumerable<FileSystemEntry> items, string filename)
        {
            _items = items.ToList();
        }

        public void Delete(string filename)
        {
            _items = new List<FileSystemEntry>();
        }
    }
}
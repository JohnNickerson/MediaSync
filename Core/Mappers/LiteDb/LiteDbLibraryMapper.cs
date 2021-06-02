using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using LiteDB;

namespace AssimilationSoftware.MediaSync.Core.Mappers.LiteDb
{
    public class LiteDbLibraryMapper : IMapper<Library> 
    {
        private LiteDatabase _database;
        private LiteCollection<Library> _data;

        public LiteDbLibraryMapper(string filename)
        {
            _database = new LiteDatabase(filename);
            _data = _database.GetCollection<Library>("libraries");
            _data.EnsureIndex(s => s.Name);
        }

        IEnumerable<Library> IMapper<Library>.LoadAll()
        {
            return _data.FindAll().ToList();
        }

        public void Save(Library item)
        {
            if (_data.Exists(n => n.Name.Equals(item.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                _data.Update(item);
            }
            else
            {
                _data.Insert(item);
            }
        }

        public void SaveAll(IEnumerable<Library> libraries)
        {
            foreach (var s in libraries)
            {
                Save(s);
            }
        }

        public void Delete(Library item)
        {
            _data.Delete(i => i.Name == item.Name);
        }

        public Library Load(string name)
        {
            return _data.FindOne(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}

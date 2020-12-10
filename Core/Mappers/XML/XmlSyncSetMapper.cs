using AssimilationSoftware.MediaSync.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Model;
using Polenter.Serialization;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.XML
{
    public class XmlLibraryMapper : IMapper<Library>
    {
        private readonly string _filename;
        private readonly SharpSerializer _serialiser;

        public XmlLibraryMapper(string filename)
        {
            _serialiser = new SharpSerializer();
            _filename = filename;
        }

        public Library Read(string name)
        {
            var allsyncsets = LoadAll();
            if (allsyncsets.Any(ss => ss.Name == name))
            {
                return allsyncsets.First(ss => ss.Name == name);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Library> LoadAll()
        {
            if (File.Exists(_filename))
            {
                using (var fileStream = new FileStream(_filename, FileMode.Open))
                {
                    return (List<Library>)_serialiser.Deserialize(fileStream);
                }
            }
            else
            {
                return new List<Library>();
            }
        }

        public void Save(Library item)
        {
            var allsyncsets = LoadAll().ToList();
            if (allsyncsets.Any(ss => ss.Name == item.Name))
            {
                allsyncsets.RemoveAll(ss => ss.Name == item.Name);
            }
            allsyncsets.Add(item);
            SaveAll(allsyncsets.ToList());
        }

        public void SaveAll(IEnumerable<Library> libraries)
        {
            using (var fileStream = new FileStream(_filename, FileMode.OpenOrCreate))
            {
                _serialiser.Serialize(libraries, fileStream);
            }
        }

        public void Delete(Library item)
        {
            var allData = LoadAll().Where(i => i.Name != item.Name);
            SaveAll(allData);
        }
    }
}

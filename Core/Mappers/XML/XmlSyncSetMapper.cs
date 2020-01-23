using AssimilationSoftware.MediaSync.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Model;
using Polenter.Serialization;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.XML
{
    public class XmlSyncSetMapper : ISyncSetMapper
    {
        private readonly string _filename;
        private readonly SharpSerializer _serialiser;

        public XmlSyncSetMapper(string filename)
        {
            _serialiser = new SharpSerializer();
            _filename = filename;
        }

        public SyncSet Read(string name)
        {
            var allsyncsets = ReadAll();
            if (allsyncsets.Any(ss => ss.Name == name))
            {
                return allsyncsets.First(ss => ss.Name == name);
            }
            else
            {
                return null;
            }
        }

        public List<SyncSet> ReadAll()
        {
            if (File.Exists(_filename))
            {
                using (var fileStream = new FileStream(_filename, FileMode.Open))
                {
                    return (List<SyncSet>)_serialiser.Deserialize(fileStream);
                }
            }
            else
            {
                return new List<SyncSet>();
            }
        }

        public void Update(SyncSet syncSet)
        {
            var allsyncsets = ReadAll();
            if (allsyncsets.Any(ss => ss.Name == syncSet.Name))
            {
                allsyncsets.RemoveAll(ss => ss.Name == syncSet.Name);
            }
            allsyncsets.Add(syncSet);
            UpdateAll(allsyncsets.ToList());
        }

        public void UpdateAll(List<SyncSet> syncSets)
        {
            using (var fileStream = new FileStream(_filename, FileMode.OpenOrCreate))
            {
                _serialiser.Serialize(syncSets, fileStream);
            }
        }
    }
}

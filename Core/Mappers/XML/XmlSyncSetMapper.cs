using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;
using Polenter.Serialization;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.XML
{
    public class XmlSyncSetMapper : ISyncSetMapper
    {
        private string _filename;
        private SharpSerializer _serialiser;

        public XmlSyncSetMapper(string filename)
        {
            _serialiser = new SharpSerializer();
            _filename = filename;
        }

        public void Delete(string name)
        {
            var allsyncsets = ReadAll();
            UpdateAll(allsyncsets.Where(ss => ss.Name != name).ToList());
        }

        public void Delete(SyncSet syncset)
        {
            Delete(syncset.Name);
        }

        public SyncSet Read(string name)
        {
            var allsyncsets = ReadAll();
            if (allsyncsets.Any(ss => ss.Name == name))
            {
                return allsyncsets.Where(ss => ss.Name == name).First();
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
                return (List<SyncSet>)_serialiser.Deserialize(_filename);
            }
            else
            {
                return new List<SyncSet>();
            }
        }

        public void Update(SyncSet syncset)
        {
            var allsyncsets = ReadAll();
            if (allsyncsets.Any(ss => ss.Name == syncset.Name))
            {
                allsyncsets.RemoveAll(ss => ss.Name == syncset.Name);
            }
            allsyncsets.Add(syncset);
            UpdateAll(allsyncsets.ToList());
        }

        public void UpdateAll(List<SyncSet> syncsets)
        {
            _serialiser.Serialize(syncsets, _filename);
        }
    }
}

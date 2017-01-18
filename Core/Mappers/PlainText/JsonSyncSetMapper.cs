using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;
using System.IO;
using UniversalSerializerLib3;

namespace AssimilationSoftware.MediaSync.Core.Mappers.PlainText
{
    public class JsonSyncSetMapper : ISyncSetMapper
    {
        private readonly string filename = "SyncSets.json";

        public JsonSyncSetMapper(string v)
        {
            this.filename = v;
        }

        public void Delete(string name)
        {
            var allsets = ReadAll();
            allsets.RemoveAll(ss => ss.Name == name);
            UpdateAll(allsets);
        }

        public void Delete(SyncSet syncset)
        {
            Delete(syncset.Name);
        }

        public SyncSet Read(string name)
        {
            var all = ReadAll().Where(ss=> ss.Name == name);
            if (all.Any())
            {
                return all.First();
            }
            else
            {
                return null;
            }
        }

        public List<SyncSet> ReadAll()
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                var parameters = new Parameters()
                {
                    Stream = fs,
                    SerializerFormatter = SerializerFormatters.JSONSerializationFormatter
                };
                var ser = new UniversalSerializer(parameters);

                List<SyncSet> deserialized = null;
                try
                {
                    deserialized = ser.Deserialize<List<SyncSet>>();
                }
                catch { }
                return deserialized ?? new List<SyncSet>();
            }
        }

        public void Update(SyncSet syncset)
        {
            var allsets = ReadAll();
            allsets.RemoveAll(ss => ss.Name == syncset.Name);
            allsets.Add(syncset);
            UpdateAll(allsets);
        }

        public void UpdateAll(List<SyncSet> syncsets)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                Parameters parameters = new Parameters()
                {
                    Stream = fs,
                    SerializerFormatter = SerializerFormatters.JSONSerializationFormatter
                };
                UniversalSerializer ser = new UniversalSerializer(parameters);

                ser.Serialize(syncsets);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Binary
{
    public class BinarySyncSetMapper : Interfaces.ISyncSetMapper
    {
        private Dictionary<string, SyncSet> allsets = new Dictionary<string, SyncSet>();
        private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        public void Delete(string name)
        {
            if (allsets.ContainsKey(name))
            {
                allsets.Remove(name);
            }
        }

        public void Delete(SyncSet syncset)
        {
            Delete(syncset.Name);
        }

        public SyncSet Read(string name)
        {
            ReadAll();
            if (allsets.ContainsKey(name))
            {
                return allsets[name];
            }
            else
            {
                return null;
            }
        }

        public List<SyncSet> ReadAll()
        {
            if (File.Exists("SyncSets.bin"))
            {
                var stm = new System.IO.FileStream("SyncSets.bin", System.IO.FileMode.OpenOrCreate);
                allsets = (Dictionary<string, SyncSet>)formatter.Deserialize(stm);
                stm.Close();
            }
            else
            {
                allsets = new Dictionary<string, Model.SyncSet>();
            }
            return allsets.Values.ToList();
        }

        public void Update(SyncSet syncset)
        {
            ReadAll();
            allsets[syncset.Name] = syncset;
            var stm = new System.IO.FileStream("SyncSets.bin", System.IO.FileMode.Create);
            formatter.Serialize(stm, allsets.Values.ToList());
            stm.Close();
        }

        public void UpdateAll(List<SyncSet> syncsets)
        {
            allsets = new Dictionary<string, SyncSet>();
            foreach (var sync in syncsets)
            {
                allsets[sync.Name] = sync;
            }
            var stm = new System.IO.FileStream("SyncSets.bin", System.IO.FileMode.Create);
            formatter.Serialize(stm, allsets.Values.ToList());
            stm.Close();
        }
    }
}

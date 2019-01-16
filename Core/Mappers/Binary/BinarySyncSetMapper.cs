using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Model;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Binary
{
    public class BinarySyncSetMapper : Interfaces.ISyncSetMapper
    {
        private Dictionary<string, SyncSet> allSets = new Dictionary<string, SyncSet>();
        private readonly System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        public void Delete(string name)
        {
            if (allSets.ContainsKey(name))
            {
                allSets.Remove(name);
            }
        }

        public void Delete(SyncSet syncSet)
        {
            Delete(syncSet.Name);
        }

        public SyncSet Read(string name)
        {
            ReadAll();
            if (allSets.ContainsKey(name))
            {
                return allSets[name];
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
                var stm = new FileStream("SyncSets.bin", FileMode.OpenOrCreate);
                allSets = (Dictionary<string, SyncSet>)formatter.Deserialize(stm);
                stm.Close();
            }
            else
            {
                allSets = new Dictionary<string, SyncSet>();
            }
            return allSets.Values.ToList();
        }

        public void Update(SyncSet syncSet)
        {
            ReadAll();
            allSets[syncSet.Name] = syncSet;
            var stm = new FileStream("SyncSets.bin", FileMode.Create);
            formatter.Serialize(stm, allSets.Values.ToList());
            stm.Close();
        }

        public void UpdateAll(List<SyncSet> syncSets)
        {
            allSets = new Dictionary<string, SyncSet>();
            foreach (var sync in syncSets)
            {
                allSets[sync.Name] = sync;
            }
            var stm = new FileStream("SyncSets.bin", FileMode.Create);
            formatter.Serialize(stm, allSets.Values.ToList());
            stm.Close();
        }
    }
}

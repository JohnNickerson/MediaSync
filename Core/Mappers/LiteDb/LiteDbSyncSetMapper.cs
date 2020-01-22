using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using LiteDB;

namespace AssimilationSoftware.MediaSync.Core.Mappers.LiteDb
{
    public class LiteDbSyncSetMapper : ISyncSetMapper
    {
        private LiteDatabase _database;
        private LiteCollection<SyncSet> _syncSets;

        public LiteDbSyncSetMapper(string filename)
        {
            _database = new LiteDatabase(filename);
            _syncSets = _database.GetCollection<SyncSet>("syncSets");
            _syncSets.EnsureIndex(s => s.Id);
        }

        public SyncSet Read(string name)
        {
            return _syncSets.FindOne(s => s.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<SyncSet> ReadAll()
        {
            return _syncSets.FindAll().ToList();
        }

        public void UpdateAll(List<SyncSet> syncSets)
        {
            foreach (var s in syncSets)
            {
                if (_syncSets.Exists(n => n.Name.Equals(s.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    _syncSets.Update(s);
                }
                else
                {
                    _syncSets.Insert(s);
                }
            }
        }
    }
}

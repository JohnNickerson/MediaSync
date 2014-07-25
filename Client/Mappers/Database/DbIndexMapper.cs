using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Database;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    /// <summary>
    /// Stores file indexes in a database.
    /// </summary>
    class DbIndexMapper : IIndexMapper
    {
        private List<string> contents = new List<string>();
        private SyncProfile _options;
        private ProfileParticipant _localSettings;

        public DbIndexMapper(SyncProfile options, string machine)
        {
            this._options = options;
            this._localSettings = options.GetParticipant(machine);
        }

        public void Save(FileIndex index)
        {
            DatabaseContext.Default.FileIndexes.Add(index);
            DatabaseContext.Default.SaveChanges();
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            return DatabaseContext.Default.FileIndexes.Where(i => i.MachineName.ToLower() == machine.ToLower() && i.ProfileName.ToLower() == profile.ToLower()).OrderBy(i => i.TimeStamp).LastOrDefault();
        }


        public List<FileIndex> LoadAll()
        {
            return DatabaseContext.Default.FileIndexes.ToList();
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            return DatabaseContext.Default.FileIndexes.Where(i => i.ProfileName.ToLower() == profile.Name.ToLower()).ToList();
        }

        public List<FileIndex> Load(string machine)
        {
            return DatabaseContext.Default.FileIndexes.Where(i => i.MachineName.ToLower() == machine.ToLower()).ToList();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            return DatabaseContext.Default.FileIndexes.Where(i => i.MachineName.ToLower() == machine.ToLower() && i.ProfileName.ToLower() == profile.Name.ToLower()).ToList();
        }
    }
}

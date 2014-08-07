using AssimilationSoftware.MediaSync.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    public class DbSyncProfileMapper : IProfileMapper
    {
        public void Save(string machineName, MediaSync.Model.SyncProfile saveobject)
        {
            throw new NotImplementedException();
        }

        public MediaSync.Model.SyncProfile Load(string machineName, string profile)
        {
            throw new NotImplementedException();
        }

        public MediaSync.Model.SyncProfile[] Load(string machineName)
        {
            throw new NotImplementedException();
        }

        public List<MediaSync.Model.SyncProfile> Load()
        {
            return DatabaseContext.Default.SyncProfiles.ToList();
        }

        public void Save(List<MediaSync.Model.SyncProfile> profiles)
        {
            throw new NotImplementedException();
        }

        public void Save(MediaSync.Model.SyncProfile p)
        {
            DatabaseContext.Default.SyncProfiles.Add(p);
        }
    }
}

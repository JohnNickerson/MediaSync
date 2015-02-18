using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    [Obsolete("To be removed in version 1.2")]
    public class DbSyncProfileMapper : IProfileMapper
    {
        public void Save(string machineName, SyncProfile saveobject)
        {
            throw new NotImplementedException();
        }

        public SyncProfile Load(string machineName, string profile)
        {
            throw new NotImplementedException();
        }

        public SyncProfile[] Load(string machineName)
        {
            throw new NotImplementedException();
        }

        public List<SyncProfile> Load()
        {
            return DatabaseContext.Default.SyncProfiles.ToList();
        }

        // TODO: Remove this. I don't like the bulk-save process. It doesn't sit well with me.
        public void Save(List<SyncProfile> profiles)
        {
            foreach (var p in profiles)
            {
                if (!DatabaseContext.Default.SyncProfiles.Select(j => j.Id).Contains(p.Id))
                {
                    DatabaseContext.Default.SyncProfiles.Add(p);
                }
            }
            foreach (var p in DatabaseContext.Default.SyncProfiles)
            {
                if (!profiles.Select(j => j.Id).Contains(p.Id))
                {
                    DatabaseContext.Default.SyncProfiles.Remove(p);
                }
            }
            DatabaseContext.Default.SaveChanges();
        }

        public void Save(SyncProfile p)
        {
            DatabaseContext.Default.SyncProfiles.Add(p);
            DatabaseContext.Default.SaveChanges();
        }

        public SyncProfile Load(int id)
        {
            return DatabaseContext.Default.SyncProfiles.Find(id);
        }

        public void Delete(SyncProfile p)
        {
            DatabaseContext.Default.SyncProfiles.Remove(p);
            DatabaseContext.Default.SaveChanges();
        }
    }
}

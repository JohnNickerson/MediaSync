using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace AssimilationSoftware.MediaSync.Mappers.Database
{
    /// <summary>
    /// A sync profile manager that stores its data in a database.
    /// </summary>
    public class DbProfileMapper : IProfileMapper
    {
        private string _connString;

        public DbProfileMapper(string conn)
        {
            _connString = conn;
        }

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
            throw new NotImplementedException();
        }

        public void Save(List<SyncProfile> profiles)
        {
            throw new NotImplementedException();
        }
    }
}

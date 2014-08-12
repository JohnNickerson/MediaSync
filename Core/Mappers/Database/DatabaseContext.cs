﻿using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    internal class DatabaseContext : DbContext
    {
        static DatabaseContext()
        {
            Default = new DatabaseContext();
        }

        public static DatabaseContext Default { get; set; }

        public DbSet<FileIndex> FileIndexes { get; set; }
        public DbSet<FileHeader> FileHeaders { get; set; }
        public DbSet<ProfileParticipant> ProfileParticipants { get; set; }
        public DbSet<SyncProfile> SyncProfiles { get; set; }
    }
}

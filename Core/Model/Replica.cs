using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Replica : EntityBase
    {
        public int HostId { get; set; }
        public Machine Host { get; set; }
        public int IndexId { get; set; }
        public FileIndex Index { get; set; }

        /// <summary>
        /// The path on the local machine where the replica is stored.
        /// </summary>
        public string LocalPath { get; set; }

        public int LibraryId { get; set; }
        public Library Library { get; set; }
    }
}

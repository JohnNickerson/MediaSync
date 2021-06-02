using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Replica : Maroon.Model.ModelObject
    {
        public Guid MachineId { get; set; }
        public Guid IndexId { get; set; }

        /// <summary>
        /// The path on the local machine where the replica is stored.
        /// </summary>
        public string LocalPath { get; set; }

        public Guid LibraryId { get; set; }
    }
}

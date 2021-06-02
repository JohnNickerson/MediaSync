using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileIndex : Maroon.Model.ModelObject
    {
        public Guid LibraryId { get; set; }
        public Guid? ReplicaId { get; set; }

        /// <summary>
        /// The date and time at which the index was last updated.
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}

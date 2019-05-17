using System;
using System.Collections.Generic;
using System.Linq;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class FileIndex : Maroon.Model.ModelObject
    {
        /// <summary>
        /// The name of the machine to which this index belongs, if any.
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// The date and time at which the index was last updated.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The actual files that make up the index.
        /// </summary>
        public List<Guid> FileIds { get; set; } = new List<Guid>();

        /// <summary>
        /// The path on the local machine where the repository is stored.
        /// </summary>
        public string LocalPath { get; set; }

        public override object Clone()
        {
            return new FileIndex
            {
                LocalPath = LocalPath,
                MachineId = MachineId,
                TimeStamp = TimeStamp,
                FileIds = FileIds,
                ID = ID,
                RevisionGuid = RevisionGuid,
                Revision = Revision,
                LastModified = LastModified
            };
        }
    }
}

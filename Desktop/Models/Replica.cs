using System;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class Replica : Maroon.Model.ModelObject
    {
        public override object Clone()
        {
            return new Replica
            {
                BasePath = BasePath,
                LibraryId = LibraryId,
                LocalIndexId = LocalIndexId,
                MachineId = MachineId,
                LastModified = LastModified,
                ID = ID,
                RevisionGuid = RevisionGuid,
                Revision = Revision
            };
        }

        public Guid MachineId { get; set; }
        public string BasePath { get; set; }
        public Guid LocalIndexId { get; set; }
        public Guid LibraryId { get; set; }

    }
}

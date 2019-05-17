using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class Library : Maroon.Model.ModelObject
    {
        public override object Clone()
        {
            return new Library
            {
                LastModified = LastModified,
                ID = ID,
                Revision = Revision,
                RevisionGuid = RevisionGuid,
                Name = Name,
                MasterIndexId = MasterIndexId
            };
        }

        public string Name { get; set; }
        public Guid MasterIndexId { get; set; }
    }
}

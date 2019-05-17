using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Desktop.Models
{
    public class FlashDrive : Maroon.Model.ModelObject
    {
        public Guid Id { get; set; }
        public string DriveLetter { get; set; }
        public long ReserveSpace { get; set; }
        public FlashDriveState Status { get; set; }
        public DateTime LastSeenDate { get; set; }
        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public enum FlashDriveState
    {
        Full,
        InUse,
        Missing,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Library : Maroon.Model.ModelObject
    {
        public string Name { get; set; }
        public Guid PrimaryIndexId { get; set; }
        public ulong MaxSharedSize { get; set; }
    }
}

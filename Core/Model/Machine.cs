using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Machine : Maroon.Model.ModelObject
    {
        public string Name { get; set; }

        /// <summary>
        /// The path, on the local machine, where shared storage for file transfers is accessed.
        /// </summary>
        public string SharedPath { get; set; }
    }
}

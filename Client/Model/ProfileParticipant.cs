using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Model
{
    public class ProfileParticipant
    {
        #region Properties
        /// <summary>
        /// The path on the local machine for the file collection.
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// The path on the local machine for the shared storage.
        /// </summary>
        public string SharedPath { get; set; }

        /// <summary>
        /// The name of the machine participant.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// True if this machine contributes files to this profile.
        /// </summary>
        public bool Contributor { get; set; }

        /// <summary>
        /// True if this machine receives files for this profile.
        /// </summary>
        public bool Consumer { get; set; }
        #endregion
    }
}

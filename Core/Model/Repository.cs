﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class Repository
    {
        #region Properties
        /// <summary>
        /// Participant ID.
        /// </summary>
        public int Id { get; set; }

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

        public Machine Machine { get; set; }

        public SyncProfile Profile { get; set; }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Model
{
    public class ProfileParticipant
    {
        #region Properties
        public string LocalPath { get; set; }
        public string SharedPath { get; set; }
        public string MachineName { get; set; }
        public bool Contributor { get; set; }
        public bool Consumer { get; set; }
        #endregion
    }
}

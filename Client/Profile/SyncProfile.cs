using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;

namespace AssimilationSoftware.MediaSync.Core
{
    public class SyncProfile
    {
        #region Properties
        public string LocalPath { get; set; }
        public string SharedPath { get; set; }
        public bool Simulate { get; set; }
        public bool Contributor { get; set; }
        public bool Consumer { get; set; }
        public ulong ReserveSpace { get; set; }

        /// <summary>
        /// A search pattern for files to include.
        /// </summary>
        public string SearchPattern { get; set; }
        public string ProfileName { get; set; }
        #endregion

		#region Constructors
		public SyncProfile()
		{
		}

		public SyncProfile(DataRow row)
		{
            this.SearchPattern = (string)row["SearchPattern"];
			this.ReserveSpace = (ulong)(long)row["SharedSpace"];
			this.SharedPath = (string)row["SharedPath"];
			this.Simulate = false;
			this.Consumer = (bool)row["Consumer"];
			this.Contributor = (bool)row["Contributor"];
			this.LocalPath = (string)row["MediaPath"];
			this.ProfileName = (string)row["Profile"];
		}
		#endregion
    }
}

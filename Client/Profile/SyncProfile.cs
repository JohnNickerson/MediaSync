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
    [Serializable]
    public class SyncProfile
    {
        #region Fields
        public string SourcePath;
        public string SharedPath;
        public bool Simulate;
		public bool Contributor;
		public bool Consumer;
        public ulong ReserveSpace;
        public string[] ExcludePatterns;
		public string ProfileName;
        #endregion

		#region Constructors
		public SyncProfile()
		{
		}

		public SyncProfile(DataRow row)
		{
			this.ExcludePatterns = new string[] { @"Thumbs\.db", @"desktop\.ini", @".*_index\.txt" };
			this.ReserveSpace = (ulong)(long)row["SharedSpace"];
			this.SharedPath = (string)row["SharedPath"];
			this.Simulate = false;
			this.Consumer = (bool)row["Consumer"];
			this.Contributor = (bool)row["Contributor"];
			this.SourcePath = (string)row["MediaPath"];
			this.ProfileName = (string)row["Profile"];
		}
		#endregion

		#region Methods
		//TODO: public static void Save(SyncProfile saveObject)
        //TODO: move SyncProfile load/save methods to a ProfileManager class.
        #endregion
    }
}

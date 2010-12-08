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
		/// <summary>
        /// Serialises a SyncProfile object to a named file.
        /// </summary>
        /// <param name="filename">The name of the file to save to.</param>
        /// <param name="saveobject">The sync options to save.</param>
		public static void Save(string filename, SyncProfile saveobject)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SyncProfile));
            Stream stream = new FileStream(filename,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveobject);
            stream.Close();
        }

		/// <summary>
		/// Returns a list of all profiles in the database for a given machine name.
		/// </summary>
		/// <param name="machineName">The name of the machine whose profiles to load.</param>
		/// <returns>An array of profiles.</returns>
		public static SyncProfile[] Load(string machineName)
		{
			SqlCeConnection connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			List<SyncProfile> result = new List<SyncProfile>();
			foreach (DataRow r in data.Tables[0].Select(string.Format("Machine = '{0}'", machineName)))
			{
				SyncProfile opts = new SyncProfile(r);
				result.Add(opts);
			}
			return result.ToArray();
		}

		/// <summary>
		/// Loads a profile for a given machine and profile name.
		/// </summary>
		/// <param name="machineName">The machine name whose profile to load.</param>
		/// <param name="profile">The name of the profile to load.</param>
		/// <returns>The profile options for the given machine name and profile name if any.</returns>
		public static SyncProfile Load(string machineName, string profile)
		{
			SqlCeConnection connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			SyncProfile result = new SyncProfile();
			DataRow r = data.Tables[0].Select(string.Format("Machine = '{0}' And Profile = '{1}'", machineName, profile))[0];
			SyncProfile opts = new SyncProfile(r);
			result = opts;
			return result;
		}
		//TODO: public static void Save(SyncProfile saveObject)
        //TODO: move SyncProfile load/save methods to a ProfileManager class.
        #endregion
    }
}

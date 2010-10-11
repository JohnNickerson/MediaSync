using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;

namespace Client
{
    [Serializable]
    public class SyncOptions
    {
        #region Fields
        public string SourcePath;
        public string SharedPath;
        public bool Simulate;
		public bool Contributor;
		public bool Consumer;
        public ulong ReserveSpace;
        public string[] ExcludePatterns;
        #endregion

        #region Methods
        /// <summary>
        /// Serialises a SyncOptions object to a named file.
        /// </summary>
        /// <param name="filename">The name of the file to save to.</param>
        /// <param name="saveobject">The sync options to save.</param>
		[Obsolete("Sync options are now stored in a database.")]
		public static void Save(string filename, SyncOptions saveobject)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SyncOptions));
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
		public static SyncOptions[] Load(string machineName)
		{
			SqlCeConnection connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			List<SyncOptions> result = new List<SyncOptions>();
			foreach (DataRow r in data.Tables[0].Select(string.Format("Machine = '{0}'", machineName)))
			{
				SyncOptions opts = new SyncOptions();
				opts.ExcludePatterns = new string[] { @"Thumbs\.db", @"desktop\.ini", @".*_index\.txt" };
				opts.ReserveSpace = (ulong)(long)r["SharedSpace"];
				opts.SharedPath = (string)r["SharedPath"];
				opts.Simulate = false;
				opts.Consumer = (bool)r["Consumer"];
				opts.Contributor = (bool)r["Contributor"];
				opts.SourcePath = (string)r["MediaPath"];
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
		public static SyncOptions Load(string machineName, string profile)
		{
			SqlCeConnection connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			SyncOptions result = new SyncOptions();
			DataRow r = data.Tables[0].Select(string.Format("Machine = '{0}' And Profile = '{1}'", machineName, profile))[0];
			SyncOptions opts = new SyncOptions();
			opts.ExcludePatterns = new string[] { @"Thumbs\.db", @"desktop\.ini", @".*_index\.txt" };
			opts.ReserveSpace = (ulong)(long)r["SharedSpace"];
			opts.SharedPath = (string)r["SharedPath"];
			opts.Simulate = false;
			opts.Consumer = (bool)r["Consumer"];
			opts.Contributor = (bool)r["Contributor"];
			opts.SourcePath = (string)r["MediaPath"];
			result = opts;
			return result;
		}
		//TODO: public static void Save(SyncOptions saveObject)
        #endregion
    }
}

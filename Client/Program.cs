using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Data;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Views;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Mappers.PlainText;
using AssimilationSoftware.MediaSync.Mappers.Xml;

namespace AssimilationSoftware.MediaSync.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configuration
            if (args.Contains("reconfigure"))
            {
                Settings.Default.Configured = false;
            }
            if (!Settings.Default.Configured)
            {
                Settings.Default.MachineName = ConfigureString(Settings.Default.MachineName, "Machine name");
                Settings.Default.ProfilesLocation = ConfigurePath(Settings.Default.ProfilesLocation, "Profiles list");
                Settings.Default.Configured = true;

                Settings.Default.Save();
            }
            #endregion

            IProfileMapper profileManager = new XmlProfileMapper(Settings.Default.ProfilesLocation);
            if (args.Contains("addprofile"))
            {
                var profiles = profileManager.Load();

                var profile = new SyncProfile();
                profile.ProfileName = ConfigureString("NewProfile", "Profile name");
                profile.LocalPath = ConfigurePath(@"D:\Src\MediaSync\TestData\Pictures", "Local path");
                profile.SharedPath = ConfigurePath(@"D:\Src\MediaSync\TestData\SharedSpace", "Path to shared space");
                profile.ReserveSpace = ConfigureInt(500, "Reserve space (MB)") * (ulong)Math.Pow(10, 6);
                profile.Consumer = true;
                profile.Contributor = true;
                profile.Simulate = false;
                profile.SearchPattern = ConfigureString("*.jpg", "File search pattern");

                profiles.Add(profile);
                profileManager.Save(profiles);
            }
            else
            {
                IOutputView view = new ConsoleView();
                try
                {
                    foreach (SyncProfile opts in profileManager.Load())
                    {
                        view.WriteLine(string.Empty);
                        view.WriteLine(string.Format("Processing profile {0}", opts.ProfileName));

                        IIndexMapper indexer = new TextIndexMapper(opts);
                        IFileManager copier = new QueuedDiskCopier(opts, indexer);
                        SyncService s = new SyncService(opts, view, indexer, copier);
                        s.Sync();
                    }
                }
                catch (Exception e)
                {
                    view.WriteLine("Could not sync.");
                    view.WriteLine(e.Message);
                    File.WriteAllText("error.log", e.StackTrace);
                }
            }

            Console.WriteLine("Finished. Press a key to exit.");
            Console.ReadKey();
        }

        private static ulong ConfigureInt(ulong value, string prompt)
        {
            ulong configval = value;
            Console.WriteLine("Configure value for {0}:", prompt);
            Console.WriteLine("Type correct value or [Enter] to accept default.");
            Console.WriteLine(value);
            var response = Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                if (ulong.TryParse(response, out configval))
                {
                    // Everything is fine.
                }
                else
                {
                    Console.WriteLine("Could not parse value. Using default.");
                }
            }
            return configval;
        }

		public void ConnectListAndSaveSQLCompactExample()
		{
			// Create a connection to the file datafile.sdf in the program folder
			string dbfile = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\index.sdf";
			SqlCeConnection connection = new SqlCeConnection("datasource=" + dbfile);

			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			// Add a row to the test_table (assume that table consists of a text column)
			DataRow r = data.Tables[0].NewRow();
			r["ID"] = 1;
			r["Machine"] = Environment.MachineName;
			r["Profile"] = "Music";
			r["MediaPath"] = Environment.SpecialFolder.MyMusic;
			r["SharedPath"] = "J:\\Music";
			r["SharedSpace"] = 4000000000;
			r["Contributor"] = true;
			r["Consumer"] = true;
			data.Tables[0].Rows.Add(r);
			//data.Tables[0].Rows.Add(new object[] { "New row added by code" });

			// Save data back to the database
			//adapter.Update(data);
			adapter.InsertCommand = new SqlCeCommand("Insert Into Profiles (Machine, Profile, MediaPath, SharedPath, SharedSpace, Contributor, Consumer) Values (@Machine, @Profile, @MediaPath, @SharedPath, @SharedSpace, @Contributor, @Consumer)", connection);
			foreach (DataColumn col in r.Table.Columns)
			{
				adapter.InsertCommand.Parameters.AddWithValue(string.Format("@{0}", col.ColumnName), r[col]);
			}
			connection.Open();
			adapter.InsertCommand.ExecuteNonQuery();

			// Close 
			connection.Close();
		}

        /// <summary>
        /// Prompts to configure a path based on an existing value.
        /// </summary>
        /// <param name="path">The path as it exists. May include "{MyDocs}" as a placeholder.</param>
        /// <param name="prompt">The human-friendly name of the folder to be used as a cue.</param>
        /// <returns>The correct path as provided by the user.</returns>
        private static string ConfigurePath(string path, string prompt)
        {
            // Special folder replacements.
            path = path.Replace("{MyDocs}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = path.Replace("{MyPictures}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = path.Replace("{MachineName}", Settings.Default.MachineName);

            Console.WriteLine("Configure path to {0}:", prompt);
            Console.WriteLine("Type correct value or [Enter] to accept default.");
            Console.WriteLine(path);
            var response = Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                path = response;
                Console.WriteLine();
            }
            return path;
        }

        /// <summary>
        /// Prompts to configure a string value, or accept a default.
        /// </summary>
        /// <param name="value">The initial default value.</param>
        /// <param name="prompt">A prompt for the user.</param>
        /// <returns>The configured value as entered or accepted by the user.</returns>
        private static string ConfigureString(string value, string prompt)
        {
            value = value.Replace("{MachineName}", Environment.MachineName);
            
            Console.WriteLine("Configure string value for {0}:", prompt);
            Console.WriteLine("Type correct value or [Enter] to accept default.");
            Console.WriteLine(value);
            var response = Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                value = response;
                Console.WriteLine();
            }
            return value;
        }
    }
}

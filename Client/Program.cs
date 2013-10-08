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
            IOutputView view = new ConsoleView();
            IInputView configurator = (IInputView)view;

            #region Configuration
            if (args.Contains("reconfigure"))
            {
                Settings.Default.Configured = false;
            }
            if (!Settings.Default.Configured)
            {
                Settings.Default.MachineName = configurator.ConfigureString(Settings.Default.MachineName, "Machine name");
                Settings.Default.ProfilesLocation = configurator.ConfigurePath(Settings.Default.ProfilesLocation, "Profiles list");
                Settings.Default.Configured = true;

                Settings.Default.Save();
            }
            #endregion

            IProfileMapper profileManager = new XmlProfileMapper(Settings.Default.ProfilesLocation);
            if (args.Contains("addprofile"))
            {
                var profiles = profileManager.Load();

                var profile = new SyncProfile();
                profile.ProfileName = configurator.ConfigureString("NewProfile", "Profile name");
                var participant = new ProfileParticipant();
                participant.LocalPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\Pictures", "Local path");
                participant.SharedPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\SharedSpace", "Path to shared space");
                profile.ReserveSpace = configurator.ConfigureULong(500, "Reserve space (MB)") * (ulong)Math.Pow(10, 6);
                participant.Consumer = true;
                participant.Contributor = true;
                participant.MachineName = Settings.Default.MachineName;
                profile.SearchPatterns.Add(configurator.ConfigureString("*.jpg", "File search pattern"));

                profile.Participants.Add(participant);
                profiles.Add(profile);
                profileManager.Save(profiles);
            }
            else if (args.Contains("joinprofile"))
            {
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    view.WriteLine(p.ProfileName);
                }
                string profilename = configurator.ConfigureString("", "Profile to join");
                if ((from p in profiles select p.ProfileName.ToLower()).Contains(profilename.ToLower()))
                {
                    var profile = (from p in profiles select p).First();
                    var participant = new ProfileParticipant();
                    participant.LocalPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\Pictures", "Local path");
                    participant.SharedPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\SharedSpace", "Path to shared space");
                    participant.Consumer = true;
                    participant.Contributor = true;
                    participant.MachineName = Settings.Default.MachineName;
                    profile.Participants.Add(participant);
                    // TODO: Test that the updated profile is updated in the collection.
                    profileManager.Save(profiles);
                }
            }
            else
            {
                try
                {
                    foreach (SyncProfile opts in profileManager.Load())
                    {
                        if (opts.ContainsParticipant(Settings.Default.MachineName))
                        {
                            view.WriteLine(string.Empty);
                            view.WriteLine(string.Format("Processing profile {0}", opts.ProfileName));

                            IIndexMapper indexer = new TextIndexMapper(opts);
                            IFileManager copier = new QueuedDiskCopier(opts, indexer);
                            SyncService s = new SyncService(opts, view, indexer, copier, false);
                            s.Sync();
                        }
                        else
                        {
                            view.WriteLine(string.Empty);
                            view.WriteLine("Not participating in profile {0}", opts.ProfileName);
                        }
                    }
                }
                catch (Exception e)
                {
                    view.WriteLine("Could not sync.");
                    view.WriteLine(e.Message);
                    File.WriteAllText("error.log", e.StackTrace);
                }
            }

            view.WriteLine("Finished. Press a key to exit.");
            configurator.WaitForKey();
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
    }
}

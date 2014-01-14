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
using System.Diagnostics;

namespace AssimilationSoftware.MediaSync.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            IOutputView view = new ConsoleView();
            IInputView configurator = (IInputView)view;
            Debug.Listeners.Add(new TextWriterTraceListener("error.log"));

            #region Configuration
            if (args.Contains("reconfigure"))
            {
                Settings.Default.Configured = false;
            }
            if (!Settings.Default.Configured)
            {
                Settings.Default.MachineName = configurator.ConfigureString(Settings.Default.MachineName, "Machine name");
                Settings.Default.MetadataFolder = configurator.ConfigurePath(Settings.Default.MetadataFolder, "Metadata folder location");
                Settings.Default.Configured = true;

                Settings.Default.Save();
            }
            #endregion

            IProfileMapper profileManager = new XmlProfileMapper(Path.Combine(Settings.Default.MetadataFolder, "Profiles.xml"));
            int pulled = 0, pushed = 0, purged = 0, errors = 0;
            if (args.Contains("/?"))
            {
                #region Help text
                // Display help text.
                view.WriteLine();
                view.WriteLine("Usage:");
                view.WriteLine("client.exe [addprofile|joinprofile|leaveprofile|list|reconfigure] [/d] [/?]");
                view.WriteLine();
                view.WriteLine("\taddprofile\tAdd a new sync profile. Also adds this machine");
                view.WriteLine("\t\t\tas a participant.");
                view.WriteLine("\tjoinprofile\tJoins an existing profile as a participant.");
                view.WriteLine("\tleaveprofile\tStops participating in an existing profile.");
                view.WriteLine("\tlist\t\tLists active profiles by name, indicating whether this");
                view.WriteLine("\t\t\tmachine is participating.");
                view.WriteLine("\t\t\tCan also be used with the '/d' switch to see directory");
                view.WriteLine("\t\t\tconfiguration and contributor/consumer status.");
                view.WriteLine("\treconfigure\tAllows reconfiguration of machine name and profile");
                view.WriteLine("\t\t\tstorage location.");
                view.WriteLine("\t/d\t\tShow detailed activity reports or configuration.");
                view.WriteLine("\t/?\t\tShows this help text.");

                view.WriteLine();
                #endregion
            }
            else if (args.Contains("addprofile"))
            {
                #region Add profile
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
                #endregion
            }
            else if (args.Contains("joinprofile"))
            {
                #region Join profile
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    view.WriteLine(p.ProfileName);
                }
                string profilename = configurator.ConfigureString("", "Profile to join");
                if ((from p in profiles select p.ProfileName.ToLower()).Contains(profilename.ToLower()))
                {
                    var profile = (from p in profiles where p.ProfileName == profilename select p).First();

                    var participant = new ProfileParticipant();
                    participant.LocalPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\Pictures", "Local path");
                    participant.SharedPath = configurator.ConfigurePath(@"D:\Src\MediaSync\TestData\SharedSpace", "Path to shared space");
                    participant.Consumer = true;
                    participant.Contributor = true;
                    participant.MachineName = Settings.Default.MachineName;
                    
                    profile.Participants.Add(participant);
                    profileManager.Save(profiles);
                }
                #endregion
            }
            else if (args.Contains("leaveprofile"))
            {
                #region Leave profile
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    view.WriteLine(p.ProfileName);
                }
                string profilename = configurator.ConfigureString("", "Profile to leave");
                if ((from p in profiles select p.ProfileName.ToLower()).Contains(profilename.ToLower()))
                {
                    var profile = (from p in profiles where p.ProfileName == profilename select p).First();

                    if (profile.ContainsParticipant(Settings.Default.MachineName))
                    {
                        var participant = profile.GetParticipant(Settings.Default.MachineName);

                        profile.Participants.Remove(participant);
                        profileManager.Save(profiles);
                    }
                }
                #endregion
            }
            else if (args.Contains("list"))
            {
                #region List profiles
                // Print a summary of profiles.
                view.WriteLine(string.Empty);
                view.WriteLine("Current profiles ('*' indicates this machine is participating)");
                view.WriteLine(string.Empty);
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    var star = p.ContainsParticipant(Settings.Default.MachineName);
                    view.WriteLine("{0}\t{1}", (star ? "*" : ""), p.ProfileName);
                    // Show participating paths if detailed view is selected.
                    if (args.Contains("/d") && star)
                    {
                        var party = p.GetParticipant(Settings.Default.MachineName);
                        view.WriteLine("\t\t{0}", party.LocalPath);
                        view.WriteLine("\t\t{0}", party.SharedPath);
                        // Indicate producer/consumer status.
                        view.WriteLine("\t\t{0}Contributing, {1}Consuming", (party.Contributor ? "" : "Not "), (party.Consumer ? "" : "Not "));
                    }
                }
                view.WriteLine(string.Empty);
                #endregion
            }
            else
            {
                foreach (SyncProfile opts in profileManager.Load())
                {
                    if (opts.ContainsParticipant(Settings.Default.MachineName))
                    {
                        view.WriteLine();
                        view.WriteLine(string.Format("Processing profile {0}", opts.ProfileName));

                        IIndexMapper indexer = new XmlIndexMapper(Path.Combine(Settings.Default.MetadataFolder, "Indexes.xml"));
                        IFileManager copier = new QueuedDiskCopier(opts, indexer);
                        SyncService s = new SyncService(opts, view, indexer, copier, false);
                        s.VerboseMode = args.Contains("/d");
                        try
                        {
                            if (args.Contains("indexonly"))
                            {
                                s.ShowIndexComparison();
                            }
                            else
                            {
                                s.Sync();
                                pulled += s.PulledCount;
                                pushed += s.PushedCount;
                                purged += s.PrunedCount;
                                errors += s.Errors.Count;
                            }
                        }
                        catch (Exception e)
                        {
                            view.WriteLine("Could not sync.");
                            view.WriteLine(e.Message);
                            var x = e;
                            while (x != null)
                            {
                                Debug.WriteLine(DateTime.Now);
                                Debug.WriteLine(x.Message);
                                Debug.WriteLine(x.StackTrace);
                                Debug.WriteLine("");

                                x = x.InnerException;
                            }
                        }
                    }
                    else
                    {
                        view.WriteLine(string.Empty);
                        view.WriteLine("Not participating in profile {0}", opts.ProfileName);
                    }
                }
            }

            view.WriteLine("Finished.");
            view.WriteLine("\t{0} files pushed", pushed);
            view.WriteLine("\t{0} files pulled", pulled);
            view.WriteLine("\t{0} files purged", purged);
            view.WriteLine("\t{0} errors encountered", errors);
            Debug.Flush();
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

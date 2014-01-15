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
            IInputView configurator = new ConsoleView();
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
            int pulled = 0, pushed = 0, pruned = 0, errors = 0;
            if (args.Contains("/?"))
            {
                #region Help text
                // Display help text.
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("client.exe [addprofile|joinprofile|leaveprofile|list|reconfigure] [/d] [/?]");
                Console.WriteLine();
                Console.WriteLine("\taddprofile\tAdd a new sync profile. Also adds this machine");
                Console.WriteLine("\t\t\tas a participant.");
                Console.WriteLine("\tjoinprofile\tJoins an existing profile as a participant.");
                Console.WriteLine("\tleaveprofile\tStops participating in an existing profile.");
                Console.WriteLine("\tlist\t\tLists active profiles by name, indicating whether this");
                Console.WriteLine("\t\t\tmachine is participating.");
                Console.WriteLine("\t\t\tCan also be used with the '/d' switch to see directory");
                Console.WriteLine("\t\t\tconfiguration and contributor/consumer status.");
                Console.WriteLine("\treconfigure\tAllows reconfiguration of machine name and profile");
                Console.WriteLine("\t\t\tstorage location.");
                Console.WriteLine("\t/d\t\tShow detailed activity reports or configuration.");
                Console.WriteLine("\t/?\t\tShows this help text.");

                Console.WriteLine();
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
                    Console.WriteLine(p.ProfileName);
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
                    Console.WriteLine(p.ProfileName);
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
                Console.WriteLine(string.Empty);
                Console.WriteLine("Current profiles ('*' indicates this machine is participating)");
                Console.WriteLine(string.Empty);
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    var star = p.ContainsParticipant(Settings.Default.MachineName);
                    Console.WriteLine("{0}\t{1}", (star ? "*" : ""), p.ProfileName);
                    // Show participating paths if detailed view is selected.
                    if (args.Contains("/d") && star)
                    {
                        var party = p.GetParticipant(Settings.Default.MachineName);
                        Console.WriteLine("\t\t{0}", party.LocalPath);
                        Console.WriteLine("\t\t{0}", party.SharedPath);
                        // Indicate give/consumer status.
                        Console.WriteLine("\t\t{0}Contributing, {1}Consuming", (party.Contributor ? "" : "Not "), (party.Consumer ? "" : "Not "));
                    }
                }
                Console.WriteLine(string.Empty);
                #endregion
            }
            else
            {
                foreach (SyncProfile opts in profileManager.Load())
                {
                    if (opts.ContainsParticipant(Settings.Default.MachineName))
                    {
                        Console.WriteLine();
                        Console.WriteLine(string.Format("Processing profile {0}", opts.ProfileName));

                        IIndexMapper indexer = new XmlIndexMapper(Path.Combine(Settings.Default.MetadataFolder, "Indexes.xml"));
                        IFileManager copier = new QueuedDiskCopier(opts, indexer);
                        SyncService s = new SyncService(opts, indexer, copier, false);
                        s.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SyncServicePropertyChanged);
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
                                pruned += s.PrunedCount;
                                errors += s.Errors.Count;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not sync.");
                            Console.WriteLine(e.Message);
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
                        Console.WriteLine(string.Empty);
                        Console.WriteLine("Not participating in profile {0}", opts.ProfileName);
                    }
                }
            }

            Console.WriteLine("Finished.");
            if (pushed + pulled + pruned > 0)
            {
                Console.WriteLine("\t{0} files pushed", pushed);
                Console.WriteLine("\t{0} files pulled", pulled);
                Console.WriteLine("\t{0} files pruned", pruned);
            }
            else
            {
                Console.WriteLine("\tNo actions taken");
            }
            if (errors > 0)
            {
                Console.WriteLine("\t{0} errors encountered", errors);
            }
            Debug.Flush();
        }

        static void SyncServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SyncService s = (SyncService)sender;
            switch (e.PropertyName)
            {
                case "Log":
                    Console.WriteLine(s.Log.Last());
                    break;
                case "Status":
                    Console.WriteLine(s.Status);
                    break;
            }
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

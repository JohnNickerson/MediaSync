using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using System.IO;
using AssimilationSoftware.MediaSync.Console.Properties;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Mappers.Xml;
using AssimilationSoftware.MediaSync.Model;

namespace AssimilationSoftware.MediaSync.Console
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
                System.Console.WriteLine("MediaSync version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                System.Console.WriteLine(@"
Usage:
    client.exe [command] [/d] [/?]

Commands:
    addprofile      Add a new sync profile with this machine as a participant.
    joinprofile     Joins an existing profile as a participant.
    leaveprofile    Stops participating in an existing profile.
    list            Lists active profiles by name, indicating whether this
                    machine is participating.
    reconfigure     Allows reconfiguration of machine name and profile
                    storage location.
    removemachine   Allows removal of a configured machine from every profile.
    /d              Show detailed activity reports or configuration.
    /?              Shows this help text.
");
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
                string firstprofile = string.Empty;
                foreach (SyncProfile p in profiles)
                {
                    if (p.ContainsParticipant(Settings.Default.MachineName))
                    {
                        System.Console.Write("*\t");
                    }
                    else
                    {
                        System.Console.Write("\t");
                        if (firstprofile.Length == 0)
                        {
                            firstprofile = p.ProfileName;
                        }
                    }
                    System.Console.WriteLine(p.ProfileName);
                }
                string profilename = configurator.ConfigureString(firstprofile, "Profile to join");
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
                string firstprofile = string.Empty;
                foreach (SyncProfile p in profiles)
                {
                    if (p.ContainsParticipant(Settings.Default.MachineName))
                    {
                        System.Console.Write("*\t");
                        if (firstprofile.Length == 0)
                        {
                            firstprofile = p.ProfileName;
                        }
                    }
                    else
                    {
                        System.Console.Write("\t");
                    }
                    System.Console.WriteLine(p.ProfileName);
                }
                string profilename = configurator.ConfigureString(firstprofile, "Profile to leave");
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
                System.Console.WriteLine(string.Empty);
                System.Console.WriteLine("Current profiles ('*' indicates this machine is participating)");
                System.Console.WriteLine(string.Empty);
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    var star = p.ContainsParticipant(Settings.Default.MachineName);
                    System.Console.WriteLine("{0}\t{1}", (star ? "*" : ""), p.ProfileName);
                    // Show participating paths if detailed view is selected.
                    if (args.Contains("/d") && star)
                    {
                        var party = p.GetParticipant(Settings.Default.MachineName);
                        System.Console.WriteLine("\t\t{0}", party.LocalPath);
                        System.Console.WriteLine("\t\t{0}", party.SharedPath);
                        // Indicate give/consumer status.
                        System.Console.WriteLine("\t\t{0}Contributing, {1}Consuming", (party.Contributor ? "" : "Not "), (party.Consumer ? "" : "Not "));
                    }
                }
                System.Console.WriteLine(string.Empty);
                #endregion
            }
			else if (args.Contains("listmachines"))
			{
				#region List participant machines
                ListMachines(profileManager);
				#endregion
			}
			else if (args.Contains("removemachine"))
            {
                #region Remove a machine from all profiles
                ListMachines(profileManager);
                string machine = configurator.ConfigureString("", "Machine to remove");
                var profiles = profileManager.Load();
                foreach (SyncProfile p in profiles)
                {
                    for (int x = 0; x < p.Participants.Count; )
                    {
                        if (p.Participants[x].MachineName == machine)
                        {
                            p.Participants.RemoveAt(x);
                        }
                        else
                        {
                            x++;
                        }
                    }
                }
                profileManager.Save(profiles);
                #endregion
            }
            else
            {
                foreach (SyncProfile opts in profileManager.Load())
                {
                    if (opts.ContainsParticipant(Settings.Default.MachineName))
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine(string.Format("Processing profile {0}", opts.ProfileName));

                        IIndexMapper indexer = new XmlIndexMapper(Path.Combine(Settings.Default.MetadataFolder, "Indexes.xml"));
                        IFileManager copier = new QueuedDiskCopier(opts, indexer, Settings.Default.MachineName);
                        SyncService s = new SyncService(opts, indexer, copier, false, Settings.Default.MachineName);
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
                            System.Console.WriteLine("Could not sync.");
                            System.Console.WriteLine(e.Message);
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
                        System.Console.WriteLine(string.Empty);
                        System.Console.WriteLine("Not participating in profile {0}", opts.ProfileName);
                    }
                }
            }

            System.Console.WriteLine("Finished.");
            if (pushed + pulled + pruned > 0)
            {
                System.Console.WriteLine("\t{0} files pushed", pushed);
                System.Console.WriteLine("\t{0} files pulled", pulled);
                System.Console.WriteLine("\t{0} files pruned", pruned);
            }
            else
            {
                System.Console.WriteLine("\tNo actions taken");
            }
            if (errors > 0)
            {
                System.Console.WriteLine("\t{0} errors encountered", errors);
            }
            Debug.Flush();
        }

        private static void ListMachines(IProfileMapper profileManager)
        {
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine("Current machines:");
            System.Console.WriteLine(string.Empty);
            var profiles = profileManager.Load();
            var participants = new List<String>();
            foreach (SyncProfile p in profiles)
            {
                foreach (ProfileParticipant party in p.Participants)
                {
                    if (!participants.Contains(party.MachineName))
                    {
                        participants.Add(party.MachineName);
                    }
                }
            }
            foreach (string p in participants)
            {
                System.Console.WriteLine("\t\t{0}{1}", p, (p == Settings.Default.MachineName ? " <-- This machine" : ""));
            }
            System.Console.WriteLine(string.Empty);
        }

        static void SyncServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SyncService s = (SyncService)sender;
            switch (e.PropertyName)
            {
                case "Log":
                    System.Console.WriteLine(s.Log.Last());
                    break;
                case "Status":
                    System.Console.WriteLine(s.Status);
                    break;
            }
        }
    }
}

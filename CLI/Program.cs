using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using AssimilationSoftware.MediaSync.CLI.Properties;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.CLI.Options;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.CLI.Views;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Mappers.PlainText;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.Extensions;

namespace AssimilationSoftware.MediaSync.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Settings.Default.Configured && !args.Contains("init") && !args.Contains("help"))
            {
                Console.WriteLine("Please use the 'init' command to set up first.");
                return;
            }


            string argverb = string.Empty;
            object argsubs = null;
            var options = new Options.Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
                {
                    argverb = verb;
                    argsubs = subOptions;
                }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            Debug.Listeners.Add(new TextWriterTraceListener("error.log"));

            var mapper = new XmlSyncSetMapper("SyncData.xml");
            var vm = new ViewModel(mapper, Settings.Default.MachineName, new SimpleFileManager(new Sha1Calculator()));
            vm.PropertyChanged += SyncServicePropertyChanged;

            switch (argverb)
            {
                case "add-profile":
                    #region Add profile
                    {
                        var addOptions = (AddProfileSubOptions)argsubs;
                        vm.CreateProfile(addOptions.ProfileName, addOptions.ReserveSpaceMB * 1000000, addOptions.IgnorePatterns);
                        vm.JoinProfile(addOptions.ProfileName, addOptions.LocalPath, addOptions.SharedPath, addOptions.Contributor, addOptions.Consumer);
                    }
                    #endregion
                    break;
                case "join-profile":
                    #region Join profile
                    {
                        var joinOptions = (JoinProfileSubOptions)argsubs;
                        vm.JoinProfile(joinOptions.ProfileName, joinOptions.LocalPath, joinOptions.SharedPath, joinOptions.Contributor, joinOptions.Consumer);
                    }
                    #endregion
                    break;
                case "leave-profile":
                    #region Leave profile
                    {
                        var leaveOptions = (LeaveProfileSubOptions)argsubs;
                        vm.LeaveProfile(leaveOptions.ProfileName);
                        PrintProfilesWithParticipation(vm.Profiles);
                    }
                    #endregion
                    break;
                case "list":
                    #region List profiles
                    {
                        var listOptions = (ListProfilesSubOptions)argsubs;
                        // Print a summary of profiles.
                        System.Console.WriteLine(string.Empty);
                        System.Console.WriteLine("Current profiles ('*' indicates this machine is participating)");
                        System.Console.WriteLine(string.Empty);
                        foreach (SyncSet p in vm.Profiles)
                        {
                            var star = p.ContainsParticipant(Settings.Default.MachineName);
                            System.Console.WriteLine("{0}\t{1}", (star ? "*" : ""), p.Name);
                            // Show participating paths if detailed view is selected.
                            if (listOptions.Verbose && star)
                            {
                                var party = p.GetIndex(Settings.Default.MachineName);
                                System.Console.WriteLine("\t\t{0}", party.LocalPath);
                                System.Console.WriteLine("\t\t{0}", party.SharedPath);
                                // Indicate give/consumer status.
                                System.Console.WriteLine("\t\t{0}Contributing, {1}Consuming", (party.IsPush ? "" : "Not "), (party.IsPull ? "" : "Not "));
                            }
                        }
                        System.Console.WriteLine(string.Empty);
                    }
                    #endregion
                    break;
                case "list-machines":
                    new MachineListConsoleView(vm).Run();
                    break;
                case "init":
                    var initOptions = (InitSubOptions)argsubs;
                    Settings.Default.MachineName = initOptions.MachineName;
                    Settings.Default.MetadataFolder = initOptions.MetadataFolder;
                    Settings.Default.Configured = true;

                    Settings.Default.Save();
                    break;
                case "remove-machine":
                    #region Remove a machine from all profiles
                    {
                        var removeOptions = (RemoveMachineSubOptions)argsubs;
                        string machine = removeOptions.MachineName;
                        vm.RemoveMachine(machine);
                        var machineview = new MachineListConsoleView(vm);
                        machineview.Run();
                    }
                    #endregion
                    break;
                case "run":
                    #region Run profiles
                    {
                        // TODO: SearchSpecification for which profiles to run.
                        // TODO: Confirm profile selections before running.
                        var runOptions = (RunSubOptions)argsubs;
                        var begin = DateTime.Now;
                        vm.RunSync(runOptions.Verbose, runOptions.IndexOnly, new System.ComponentModel.PropertyChangedEventHandler(SyncServicePropertyChanged));
                        Console.WriteLine("Total time taken: {0}", (DateTime.Now - begin).Verbalise());
                    }
                    #endregion
                    break;
                case "version":
                    Console.WriteLine("MediaSync v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                    break;
            }
            Debug.Flush();
        }

        private static void PrintProfilesWithParticipation(List<SyncSet> profiles)
        {
            foreach (SyncSet p in profiles)
            {
                if (p.ContainsParticipant(Settings.Default.MachineName))
                {
                    System.Console.Write("*\t");
                }
                else
                {
                    System.Console.Write("\t");
                }
                System.Console.WriteLine(p.Name);
            }
        }

        /// <summary>
        /// Responds to property change events.
        /// </summary>
        static void SyncServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var s = (ViewModel)sender;
            switch (e.PropertyName)
            {
                case "Log":
                    System.Console.WriteLine(s.Log.Last());
                    break;
                case "StatusMessage":
                    System.Console.WriteLine(s.StatusMessage);
                    break;
            }
        }
    }
}

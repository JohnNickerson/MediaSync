using System;
using System.Collections.Generic;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using System.IO;
using AssimilationSoftware.MediaSync.CLI.Properties;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.CLI.Options;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.CLI.Views;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.LiteDb;

namespace AssimilationSoftware.MediaSync.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var argverb = string.Empty;
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
            else if (argverb == "help")
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }
            else if (argverb == "init")
            {
                var initOptions = (InitSubOptions)argsubs;
                Settings.Default.MachineName = initOptions.MachineName;
                Settings.Default.MetadataFolder = initOptions.MetadataFolder;
                Settings.Default.DataFileFormat = initOptions.DataFileFormat;
                Settings.Default.Configured = true;

                Settings.Default.Save();
                return;
            }
            else if (!Settings.Default.Configured)
            {
                Console.WriteLine("Please use the 'init' command to set up first.");
                return;
            }

            Debug.Listeners.Add(new TextWriterTraceListener("error.log"));
#if DEBUG
            Trace.Listeners.Add(new ConsoleTraceListener());
#endif

            ISyncSetMapper mapper;
            if (string.Equals(Settings.Default.DataFileFormat, "litedb", StringComparison.CurrentCultureIgnoreCase))
            {
                mapper = new LiteDbSyncSetMapper(Path.Combine(Settings.Default.MetadataFolder, "SyncData.db"));
            }
            else
            {
                mapper = new XmlSyncSetMapper(Path.Combine(Settings.Default.MetadataFolder, "SyncData.xml"));
            }
            var vm = new ViewModel(mapper, Settings.Default.MachineName, new SimpleFileManager(new Sha1Calculator()));

            switch (argverb)
            {
                case "add-profile":
                    #region Add profile
                    {
                        var addOptions = (AddProfileSubOptions)argsubs;
                        vm.CreateProfile(addOptions.ProfileName, addOptions.ReserveSpaceMb * 1000000, addOptions.IgnorePatterns);
                        vm.JoinProfile(addOptions.ProfileName, addOptions.LocalPath, addOptions.SharedPath);
                    }
                    #endregion
                    break;
                case "join-profile":
                    #region Join profile
                    {
                        var joinOptions = (JoinProfileSubOptions)argsubs;
                        vm.JoinProfile(joinOptions.ProfileName, joinOptions.LocalPath, joinOptions.SharedPath);
                    }
                    #endregion
                    break;
                case "leave-profile":
                    #region Leave profile
                    {
                        var leaveOptions = (LeaveProfileSubOptions)argsubs;
                        if (leaveOptions.MachineName == "this")
                        {
                            leaveOptions.MachineName = Settings.Default.MachineName;
                        }
                        vm.LeaveProfile(leaveOptions.ProfileName, leaveOptions.MachineName);
                        PrintProfilesWithParticipation(vm.Profiles);
                    }
                    #endregion
                    break;
                case "list":
                    #region List profiles
                    {
                        var listOptions = (ListProfilesSubOptions)argsubs;
                        new ProfileListConsoleView(vm).Run(listOptions.Verbose);
                    }
                    #endregion
                    break;
                case "list-machines":
                    new MachineListConsoleView(vm).Run();
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
                case "remove-profile":
                    #region Remove an entire profile
                    {
                        var removeOptions = (RemoveProfileOptions)argsubs;
                        vm.RemoveProfile(removeOptions.ProfileName);
                        new ProfileListConsoleView(vm).Run(false);
                    }
                    break;
                #endregion
                case "run":
                    #region Run profiles
                    {
                        // TODO: SearchSpecification for which profiles to run.
                        // TODO: Confirm profile selections before running.
                        var runOptions = (RunSubOptions)argsubs;
                        var logger = new ConsoleLogger(runOptions.LogLevel);
                        var begin = DateTime.Now;
                        Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(Settings.Default.MetadataFolder, "MediaSync.log")));
                        vm.RunSync(runOptions.IndexOnly, logger, runOptions.QuickMode, runOptions.Profile);
                        logger.Log(1, "Total time taken: {0}", (DateTime.Now - begin).Verbalise());
                        Trace.Flush();
                    }
                    #endregion
                    break;
                case "update-profile":
                    #region Update a profile
                    {
                        var updateOptions = (UpdateProfileSubOptions)argsubs;
                        vm.ResizeProfile(updateOptions.ProfileName, updateOptions.ReserveSpaceMb * 1000000);
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
                Console.Write(p.ContainsParticipant(Settings.Default.MachineName) ? "*\t" : "\t");
                Console.WriteLine(p.Name);
            }
        }
    }
}

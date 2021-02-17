using System;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using System.IO;
using System.Text;
using AssimilationSoftware.MediaSync.CLI.Properties;
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
    static class Program
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
            Trace.Listeners.Add(new ConsoleTraceListener());

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

            // TODO: vm.CheckDriveLetter(Settings.Default.MachineName, Directory.GetDirectoryRoot(Environment.CurrentDirectory));
            // ie if profiles reference a different directory to the current working folder, offer to change the drive letter.

            switch (argverb)
            {
                case "add-profile":
                    #region Add profile
                    {
                        var addOptions = (AddProfileSubOptions)argsubs;
                        vm.CreateProfile(addOptions.ProfileName, addOptions.ReserveSpaceMb * 1000000);
                        vm.JoinProfile(addOptions.ProfileName, addOptions.LocalPath, addOptions.SharedPath);
                        new ProfileListConsoleView(vm).Run(false);
                    }
                    #endregion
                    break;
                case "change-drive":
                    #region change-drive
                    {
                        var changeDriveOptions = (ChangeSharedDriveOptions)argsubs;
                        // For each profile for this machine, load the shared path and change the drive letter.
                        var newDrive = new System.IO.DriveInfo(changeDriveOptions.NewDriveLetter);
                        vm.ChangeDriveLetter(newDrive, Settings.Default.MachineName);
                        new ProfileListConsoleView(vm).Run(false);
                    }
                    #endregion
                    break;
                case "join-profile":
                    #region Join profile
                    {
                        var joinOptions = (JoinProfileSubOptions)argsubs;
                        vm.JoinProfile(joinOptions.ProfileName, joinOptions.LocalPath, joinOptions.SharedPath);
                        new ProfileListConsoleView(vm).Run(false);
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
                        new ProfileListConsoleView(vm).Run(false);
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
                        var begin = DateTime.Now;
                        var logName = Path.Combine(Settings.Default.MetadataFolder, "MediaSync.log");
                        if (File.Exists(logName) && new FileInfo(logName).Length > 1000000)
                        {
                            File.Move(logName, Path.Combine(Settings.Default.MetadataFolder, $"{Path.GetFileNameWithoutExtension(logName)}.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log"));
                        }
                        Trace.Listeners.Add(new TextWriterTraceListener(logName));
                        vm.RunSync(runOptions.IndexOnly, runOptions.LogLevel >= 4, runOptions.ProfileSearch);
                        Trace.WriteLine($"Total time taken: {(DateTime.Now - begin).Verbalise()}");
                        Trace.Flush();
                    }
                    #endregion
                    break;
                case "update-profile":
                    #region Update a profile
                    {
                        var updateOptions = (UpdateProfileSubOptions)argsubs;
                        vm.ResizeProfile(updateOptions.ProfileName, updateOptions.ReserveSpaceMb * 1000000);
                        new ProfileListConsoleView(vm).Run(true);
                    }
                    #endregion
                    break;
                case "version":
                    Console.WriteLine("MediaSync v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                    break;
            }
            Debug.Flush();
        }
    }
}

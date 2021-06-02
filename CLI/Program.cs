using System;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AssimilationSoftware.MediaSync.CLI.Properties;
using AssimilationSoftware.MediaSync.CLI.Options;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.CLI.Views;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Mappers.LiteDb;
using AssimilationSoftware.MediaSync.Core.Model;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI
{
    static class Program
    {
        static int Main(string[] args)
        {
            var argverb = string.Empty;
            object argsubs = null;

            Debug.Listeners.Add(new TextWriterTraceListener("error.log"));
            Trace.Listeners.Add(new ConsoleTraceListener());

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            if (!Settings.Default.Configured)
            {
                return CommandLine.Parser.Default.ParseArguments<InitOptions>(args)
                    .MapResult(
                        Initialise,
                    errs => 1);
            }

            /*
             * 	init
		            - Identify this machine. Locate the shared folder path.
		            - Preferably stamp the shared folder (flash drive) with an ID file that can be found automatically later, in case drive letters are rearranged.
	            sync
		            - Run synchronisation for the local machine.
		            - Allow a list of libraries or replicas to run on?
	            delete-machine
		            - Remove a machine and all its replicas.
	            add-library
		            - Create (or update?) a library collection to be replicated across machines.
		            - Assume that this machine has a copy of the files, so include the local path.
	            delete-library
		            - Remove a library and all record of its replicas.
		            - Do not delete files, just indexes and model data.
	            add-replica
		            - Identify a replica of a library.
		            - Need not be on the current machine, but it's easier there.
	            delete-replica
		            - Stop synchronising a library to a particular location.
	            dir
		            - Inspect the database, looking at known machines, libraries, replicas, files and folders. 
		            - See which replicas have a copy of files in a library. Check whether a specific file has been deleted.
	            delete-file
		            - Mark a file in the primary index as deleted.
	            undelete-file
		            - Mark a file in the primary index as not deleted. Essentially halt the propagation of a deletion through the swarm.
                move-replica
                    - Update the local path of a replica.
                update-library
		            - Update a library collection to be replicated across machines.
             */

            return CommandLine.Parser.Default.ParseArguments<AddLibraryOptions, AddReplicaOptions, InitOptions, ViewIndexOptions, ListProfilesSubOptions, RemoveLibraryOptions, DeleteMachineOptions, RemoveReplicaOptions, RunOptions, UpdateLibraryOptions>(args)
                .MapResult(
                    (InitOptions opts) => Initialise(opts),
                    (RunOptions opts) => RunSync(opts, GetApi()),
                    (DeleteMachineOptions opts) => RemoveMachine(opts, GetApi()),
                    (AddLibraryOptions opts) => AddLibrary(opts, GetApi()),
                    (RemoveLibraryOptions opts) => RemoveLibrary(opts, GetApi()),
                    (AddReplicaOptions opts) => AddReplica(opts, GetApi()),
                    (RemoveReplicaOptions opts) => RemoveReplica(opts, GetApi()),
                    (ViewIndexOptions opts) => SearchIndexData(opts, GetApi()),
                    (ListProfilesSubOptions opts) => DoTheListProfilesSubOptionsThing(opts, GetApi()),
                    (DeleteFileOptions opts) => DeleteFile(opts, GetApi()),
                    (UndeleteFileOptions opts) => UndeleteFile(opts, GetApi()),
                    (MoveReplicaOptions opts) => MoveReplica(opts, GetApi()),
                    (UpdateLibraryOptions opts) => UpdateProfile(opts, GetApi()),
                    errs => 1);

            //ISyncSetMapper mapper;
            //if (string.Equals(Settings.Default.DataFileFormat, "litedb", StringComparison.CurrentCultureIgnoreCase))
            //{
            //    mapper = new LiteDbSyncSetMapper(Path.Combine(Settings.Default.MetadataFolder, "SyncData.db"));
            //}
            //else
            //{
            //    mapper = new XmlSyncSetMapper(Path.Combine(Settings.Default.MetadataFolder, "SyncData.xml"));
            //}
            //var vm = new ViewModel(mapper, Settings.Default.MachineName, new SimpleFileManager(new Sha1Calculator()));

            //switch (argverb)
            //{
            //    case "add-library":
            //        #region Add library
            //        {
            //            var addOptions = (AddLibrarySubOptions)argsubs;
            //            vm.CreateLibrary(addOptions.LibraryName, addOptions.ReserveSpaceMb * 1000000);
            //            vm.AddReplica(addOptions.LibraryName, addOptions.LocalPath, addOptions.SharedPath);
            //            new ProfileListConsoleView(vm).Run(false);
            //        }
            //        #endregion
            //        break;
            //    case "add-replica":
            //        #region Add replica
            //        {
            //            var joinOptions = (AddReplicaSubOptions)argsubs;
            //            vm.AddReplica(joinOptions.ProfileName, joinOptions.LocalPath, joinOptions.SharedPath);
            //            new ProfileListConsoleView(vm).Run(false);
            //        }
            //        #endregion
            //        break;
            //    case "remove-replica":
            //        #region Remove replica
            //        {
            //            var leaveOptions = (RemoveReplicaSubOptions)argsubs;
            //            if (leaveOptions.MachineName == "this")
            //            {
            //                leaveOptions.MachineName = Settings.Default.MachineName;
            //            }
            //            vm.LeaveProfile(leaveOptions.ProfileName, leaveOptions.MachineName);
            //            new ProfileListConsoleView(vm).Run(false);
            //        }
            //        #endregion
            //        break;
            //    case "list":
            //        #region List profiles
            //        {
            //            var listOptions = (ListProfilesSubOptions)argsubs;
            //            new ProfileListConsoleView(vm).Run(listOptions.Verbose);
            //        }
            //        #endregion
            //        break;
            //    case "list-machines":
            //        new MachineListConsoleView(vm).Run();
            //        break;
            //    case "remove-machine":
            //        #region Remove a machine from all profiles
            //        {
            //            var removeOptions = (RemoveMachineSubOptions)argsubs;
            //            string machine = removeOptions.MachineName;
            //            vm.RemoveMachine(machine);
            //            var machineview = new MachineListConsoleView(vm);
            //            machineview.Run();
            //        }
            //        #endregion
            //        break;
            //    case "remove-library":
            //        #region Remove an entire library
            //        {
            //            var removeOptions = (RemoveLibraryOptions)argsubs;
            //            vm.RemoveLibrary(removeOptions.ProfileName);
            //            new ProfileListConsoleView(vm).Run(false);
            //        }
            //        break;
            //    #endregion
            //    case "run":
            //        #region Run sync
            //        {
            //            // TODO: SearchSpecification for which profiles to run.
            //            // TODO: Confirm profile selections before running.
            //            var runOptions = (RunSubOptions)argsubs;
            //            var begin = DateTime.Now;
            //            var logName = Path.Combine(Settings.Default.MetadataFolder, "MediaSync.log");
            //            if (File.Exists(logName) && new FileInfo(logName).Length > 1000000)
            //            {
            //                File.Move(logName, Path.Combine(Settings.Default.MetadataFolder, $"{Path.GetFileNameWithoutExtension(logName)}.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log"));
            //            }
            //            Trace.Listeners.Add(new TextWriterTraceListener(logName));
            //            vm.RunSync(runOptions.IndexOnly, runOptions.LogLevel >= 4, runOptions.ProfileSearch);
            //            Trace.WriteLine($"Total time taken: {(DateTime.Now - begin).Verbalise()}");
            //            Trace.Flush();
            //        }
            //        #endregion
            //        break;
            //    case "update-replica":
            //        #region Update a replica
            //        {
            //            var updateOptions = (UpdateProfileSubOptions)argsubs;
            //            vm.ResizeReserve(updateOptions.ProfileName, updateOptions.ReserveSpaceMb * 1000000);
            //            new ProfileListConsoleView(vm).Run(true);
            //        }
            //        #endregion
            //        break;
            //    case "version":
            //        Console.WriteLine("MediaSync v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            //        break;
            //}
        }

        private static Core.ViewModel GetApi()
        {
            var repo = new DataStore(Settings.Default.MetadataFolder);
            var vm = new ViewModel(repo, Settings.Default.MachineName, Settings.Default.SharedPath, new SimpleFileManager(new Sha1Calculator()));
            return vm;
        }

        private static int MoveReplica(MoveReplicaOptions opts, ViewModel api)
        {
            api.MoveReplica(Guid.Parse(opts.Id), opts.Path, opts.MoveFiles);
            api.Save();
            return 0;
        }

        private static int UndeleteFile(UndeleteFileOptions opts, ViewModel api)
        {
            api.UndeleteFile(opts.LibraryName, opts.Path);
            api.Save();
            return 0;
        }

        private static int DeleteFile(DeleteFileOptions opts, ViewModel api)
        {
            api.DeleteFile(opts.LibraryName, opts.Path);
            api.Save();
            return 0;
        }

        private static int UpdateProfile(UpdateLibraryOptions opts, ViewModel api)
        {
            api.ResizeReserve(opts.LibraryName, opts.ReserveSpaceMb);
            api.Save();
            return 0;
        }

        private static int RunSync(RunOptions opts, ViewModel api)
        {
            api.RunSync(opts.IndexOnly, opts.LogLevel == 4, opts.LibraryName);
            return 0;
        }

        private static int RemoveReplica(RemoveReplicaOptions opts, ViewModel api)
        {
            api.DeleteReplica(Guid.Parse(opts.Id));
            api.Save();
            return 0;
        }

        private static int RemoveMachine(DeleteMachineOptions opts, ViewModel api)
        {
            api.RemoveMachine(opts.MachineName);
            api.Save();
            return 0;
        }

        private static int RemoveLibrary(RemoveLibraryOptions opts, ViewModel api)
        {
            api.RemoveLibrary(opts.LibraryName);
            api.Save();
            return 0;
        }

        private static int DoTheListProfilesSubOptionsThing(ListProfilesSubOptions opts, ViewModel api)
        {
            new ProfileListConsoleView(new DataStore(Settings.Default.MetadataFolder)).Run(opts.Verbose);
            return 0;
        }

        private static int SearchIndexData(ViewIndexOptions opts, ViewModel api)
        {
            new IndexFileView(new DataStore(Settings.Default.MetadataFolder)).Run(opts.MachineName, opts.LibraryName,
                opts.ReplicaId, opts.LocalPath, opts.ShowSubFolders);
            return 0;
        }

        private static int AddReplica(AddReplicaOptions opts, ViewModel api)
        {
            api.AddReplica(opts.LibraryName, opts.LocalPath, Settings.Default.MachineName);
            api.Save();
            return 0;
        }

        private static int AddLibrary(AddLibraryOptions addOptions, ViewModel api)
        {
            api.CreateLibrary(addOptions.LibraryName, addOptions.ReserveSpaceMb*1000000);
            if (!string.IsNullOrEmpty(addOptions.LocalPath))
            {
                api.AddReplica(addOptions.LibraryName, addOptions.LocalPath, Settings.Default.MachineName);
            }
            api.Save();
            return 0;
        }

        private static int Initialise(InitOptions initOptions)
        {
            Settings.Default.MachineName = initOptions.MachineName;
            Settings.Default.MetadataFolder = initOptions.MetadataFolder;
            Settings.Default.SharedPath = initOptions.SharedPath;
            Settings.Default.Configured = true;

            Settings.Default.Save();

            AddMachine(Settings.Default.MachineName, Settings.Default.SharedPath, GetApi());

            return 0;
        }

        private static void AddMachine(string machineName, string sharedPath, ViewModel api)
        {
            api.AddMachine(machineName, sharedPath);
            api.Save();
        }
    }

    internal class IndexFileView
    {
        private readonly DataStore _dataStore;

        public IndexFileView(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void Run(string machineName, string libraryName, string replicaId, string localPath, bool showSubFolders)
        {
            foreach (var file in _dataStore.ListFileSystemEntries().OrderBy(f => f.RelativePath))
            {
                var index = _dataStore.GetFileIndexById(file.IndexId);
                    Library library = null;
                    Replica replica = null;
                        Machine machine = null;
                
                // If we are looking only for a particular replica, reject others.
                if (index != null && !string.IsNullOrEmpty(replicaId) && index.ReplicaId.HasValue && !index.ReplicaId.Value.ToString().StartsWith(replicaId)) continue;

                // Check library name, if present.
                if (index != null)
                {
                    library = _dataStore.GetLibraryById(index.LibraryId);
                    if (library != null && !string.IsNullOrEmpty(libraryName) && !libraryName.Equals(library.Name, StringComparison.CurrentCultureIgnoreCase)) continue;
                }

                if (index != null)
                {
                    replica = _dataStore.GetReplicaById(index.ReplicaId);
                    if (replica != null)
                    {
                        machine = _dataStore.GetMachineById(replica.MachineId);
                        if (machine != null && !string.IsNullOrEmpty(machineName) && !machineName.Equals(machine.Name, StringComparison.CurrentCultureIgnoreCase)) continue;
                    }

                    if (replica != null && !string.IsNullOrEmpty(localPath) && !Path.Combine(replica.LocalPath, file.RelativePath)
                        .StartsWith(localPath, StringComparison.CurrentCultureIgnoreCase)) continue;
                }

                if (machine == null)
                {
                    Console.WriteLine($"\\\\*\\{library?.Name}\\{replica?.ID}\\{file.RelativePath} [{file.State}]");
                }
                else
                {
                    Console.WriteLine($"\\\\{machine.Name}\\{library?.Name}\\{replica?.ID}\\{file.RelativePath} [{file.ContentsHash}]");
                }
                // if (machineName != null && _dataStore.GetMachineById(machineId)...)
            }
        }
    }
}

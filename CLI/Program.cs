using System;
using AssimilationSoftware.MediaSync.Core;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AssimilationSoftware.MediaSync.CLI.Properties;
using AssimilationSoftware.MediaSync.CLI.Options;
using AssimilationSoftware.MediaSync.CLI.Views;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI
{
    static class Program
    {
        static int Main(string[] args)
        {
            if (File.Exists("MediaSync.log") && new FileInfo("MediaSync.log").Length > Math.Pow(2, 20))
            {
                Trace.Flush();
                // Archive the log file.
                File.Move("MediaSync.log", $"MediaSync.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log");
            }
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener("MediaSync.log"));

            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            if (!Settings.Default.Configured)
            {
                return Parser.Default.ParseArguments<InitOptions>(args)
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

            return Parser.Default.ParseArguments<AddLibraryOptions, AddReplicaOptions, InitOptions, ViewIndexOptions, ListReplicasOptions, RemoveLibraryOptions, DeleteMachineOptions, RemoveReplicaOptions, RunOptions, UpdateLibraryOptions, PurgeDataOptions>(args)
                .MapResult(
                    (InitOptions opts) => Initialise(opts),
                    (RunOptions opts) => RunSync(opts, GetApi()),
                    (DeleteMachineOptions opts) => RemoveMachine(opts, GetApi()),
                    (AddLibraryOptions opts) => AddLibrary(opts, GetApi()),
                    (RemoveLibraryOptions opts) => RemoveLibrary(opts, GetApi()),
                    (AddReplicaOptions opts) => AddReplica(opts, GetApi()),
                    (RemoveReplicaOptions opts) => RemoveReplica(opts, GetApi()),
                    (ViewIndexOptions opts) => SearchIndexData(opts),
                    (ListReplicasOptions opts) => ListReplicas(opts),
                    (DeleteFileOptions opts) => DeleteFile(opts, GetApi()),
                    (UndeleteFileOptions opts) => UndeleteFile(opts, GetApi()),
                    (MoveReplicaOptions opts) => MoveReplica(opts, GetApi()),
                    (UpdateLibraryOptions opts) => UpdateLibrary(opts, GetApi()),
                    (PurgeDataOptions opts) => PurgeDataStore(GetApi()),
                    errs => 1);
        }

        private static int PurgeDataStore(ViewModel api)
        {
            api.CleanUpDataStore();
            api.Save();
            return 0;
        }

        private static ViewModel GetApi()
        {
            var repo = new DataStore(Settings.Default.MetadataFolder);
            var vm = new ViewModel(repo, Settings.Default.MachineName, new SimpleFileManager(new Sha1Calculator()));
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

        private static int UpdateLibrary(UpdateLibraryOptions opts, ViewModel api)
        {
            api.ResizeReserve(opts.LibraryName, (ulong)(opts.ReserveSpaceMb * 1000000));
            api.Save();
            return 0;
        }

        private static int RunSync(RunOptions opts, ViewModel api)
        {
            api.RunSync(opts.IndexOnly, opts.LogLevel == 4, opts.LibraryName);
            // This should be done internally by Maroon now.
            PurgeDataStore(api);
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

        private static int ListReplicas(ListReplicasOptions opts)
        {
            new ReplicaListConsoleView(new DataStore(Settings.Default.MetadataFolder)).Run(opts.Verbose);
            return 0;
        }

        private static int SearchIndexData(ViewIndexOptions opts)
        {
            new IndexFileView(new DataStore(Settings.Default.MetadataFolder)).Run(opts.MachineName, opts.LibraryName,
                opts.ReplicaId, opts.LocalPath, opts.ShowSubFolders, opts.State);
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
            api.CreateLibrary(addOptions.LibraryName, addOptions.ReserveSpaceMb * 1000000);
            if (!string.IsNullOrEmpty(addOptions.LocalPath))
            {
                api.AddReplica(addOptions.LibraryName, addOptions.LocalPath, Settings.Default.MachineName);
            }
            api.Save();
            return 0;
        }

        private static int Initialise(InitOptions initOptions)
        {
            // Check that the shared and data paths are separated.
            var share = new DirectoryInfo(Path.GetFullPath(initOptions.SharedPath));
            var meta = new DirectoryInfo(Path.GetFullPath(initOptions.MetadataFolder));
            if (meta.IsSubPathOf(share))
            {
                Trace.WriteLine(share);
                Trace.WriteLine(meta);
                Console.WriteLine("The data folder must not be under the shared folder.");
                return 1;
            }
            Settings.Default.MachineName = initOptions.MachineName;
            Settings.Default.MetadataFolder = initOptions.MetadataFolder;
            Settings.Default.Configured = true;

            Settings.Default.Save();

            AddOrUpdateMachine(Settings.Default.MachineName, initOptions.SharedPath, new DataStore(Settings.Default.MetadataFolder));

            return 0;
        }

        private static void AddOrUpdateMachine(string machineName, string sharedPath, DataStore data)
        {
            var m = data.GetMachineByName(machineName);
            if (m != null)
            {
                m.SharedPath = sharedPath;
                data.Update(m);
            }
            else
            {
                data.Insert(new Machine
                {
                    ID = Guid.NewGuid(),
                    IsDeleted = false,
                    ImportHash = null,
                    LastModified = DateTime.Now,
                    Name = machineName,
                    PrevRevision = null,
                    RevisionGuid = Guid.NewGuid(),
                    SharedPath = sharedPath
                });
            }
            data.SaveChanges();
        }
    }

    internal class IndexFileView
    {
        private readonly DataStore _dataStore;

        public IndexFileView(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public void Run(string machineName, string libraryName, string replicaId, string localPath, bool showSubFolders,
            FileSyncState? fileSyncState)
        {
            foreach (var lib in _dataStore.ListLibraries(l =>
                string.IsNullOrEmpty(libraryName) ||
                libraryName.Equals(l.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                Console.WriteLine($"\\\\*\\{lib?.Name}\\*\\*");
                foreach (var libFile in _dataStore.ListFileSystemEntries(f => f.IndexId == lib.PrimaryIndexId).OrderBy(f => f.RelativePath))
                {
                    if (!fileSyncState.HasValue || libFile.State == fileSyncState.Value)
                        Console.WriteLine($"\\\\*\\{lib?.Name}\\*\\{libFile.RelativePath} [{libFile.State}]");
                }

                foreach (var rep in _dataStore.ListReplicas(r =>
                    r.LibraryId == lib.ID &&
                    (string.IsNullOrEmpty(replicaId) || r.ID.ToString().StartsWith(replicaId))))
                {
                    var machine = _dataStore.GetMachineById(rep.MachineId)?.Name ?? "*";
                    if (string.IsNullOrEmpty(machineName) || machine == machineName)
                    {
                        Console.WriteLine($"\\\\{machine}\\{lib?.Name}\\{rep.ID}\\*");
                        foreach (var repFile in _dataStore.ListFileSystemEntries(f => f.IndexId == rep.IndexId).OrderBy(f => f.RelativePath))
                        {
                            if (!string.IsNullOrEmpty(localPath))
                            {
                                var localDir = new DirectoryInfo(localPath);
                                var relativeDir = new DirectoryInfo(Path.Combine(rep.LocalPath, repFile.RelativePath));
                                if (!localDir.IsSubPathOf(relativeDir))
                                    continue;
                            }

                            if (!fileSyncState.HasValue || repFile.State == fileSyncState.Value)
                                Console.WriteLine($"\\\\{machine}\\{lib?.Name}\\{rep.ID}\\{repFile.RelativePath} [{repFile.ContentsHash}]");
                        }
                    }
                }
            }
        }
    }
}

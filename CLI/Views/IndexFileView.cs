using System;
using System.IO;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
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
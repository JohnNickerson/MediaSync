using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AssimilationSoftware.MediaSync.Core.Mappers;

namespace AssimilationSoftware.MediaSync.Core
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Fires when a data-bindable property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for a single property.
        /// </summary>
        /// <param name="propertyname">The name of the changed property. Default: caller.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        #endregion

        #region Fields
        public bool VerboseMode;

        private readonly IFileManager _fileManager;

        private readonly DataStore _repository;

        private bool _stopSync;
        private string _sharedBasePath;

        #endregion

        #region Constructors
        public ViewModel(DataStore dataContext, string machineId, IFileManager fileManager)
        {
            _repository = dataContext;
            _machineId = machineId;
            _fileManager = fileManager;
            _sharedBasePath = dataContext.GetMachineByName(machineId).SharedPath;
        }
        #endregion

        #region Methods
        public void CreateLibrary(string name, ulong reserve)
        {
            if (!LibraryExists(name))
            {
                var index = new FileIndex();
                var library = new Library()
                {
                    Name = name,
                    MaxSharedSize = reserve,
                    PrimaryIndexId = index.ID
                };
                index.LibraryId = library.ID;
                _repository.Insert(library);
                _repository.Insert(index);
                _repository.SaveChanges();
            }
            else
            {
                Trace.WriteLine("A library with the same name already exists: " + name);
            }
        }

        public void ResizeReserve(string name, ulong reserve)
        {
            if (LibraryExists(name))
            {
                var profile = GetLibrary(name);
                profile.MaxSharedSize = reserve;
                _repository.Update(profile);
                _repository.SaveChanges();
            }
        }

        private bool LibraryExists(string name)
        {
            return _repository.ListLibraries().Any(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        private Library GetLibrary(string name)
        {
            return _repository.ListLibraries().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void AddReplica(string libraryName, string localPath, string machineName)
        {
            var library = GetLibrary(libraryName);
            var machine = _repository.GetMachineByName(machineName);
            if (library == null)
            {
                Trace.WriteLine($"Library does not exist: {libraryName}");
            }
            else if (machine == null)
            {
                Trace.WriteLine($"Machine does not exist: {machineName}");
            }
            else if (ReplicaExists(library, localPath, machine))
            {
                Trace.WriteLine($"Replica of library {libraryName} on {machineName} already exists.");
            }
            else
            {
                AddReplica(library, localPath, machine);
            }
        }

        private bool ReplicaExists(Library library, string localPath, Machine machine)
        {
            return _repository.ListReplicas().Any(r =>
                r.LibraryId == library.ID && r.MachineId == machine.ID &&
                r.LocalPath.Equals(localPath, StringComparison.CurrentCultureIgnoreCase));
        }

        private void AddReplica(Library library, string localPath, Machine machine)
        {
            var replica = new Replica { LocalPath = localPath, LibraryId = library.ID, MachineId = machine.ID };
            _repository.Insert(replica);
        }

        public void DeleteReplica(Guid replicaId)
        {
            _repository.Delete(_repository.GetReplicaById(replicaId));
        }

        public List<string> Machines
        {
            get
            {
                return _repository.ListMachines().Select(m => m.Name).ToList();
            }
        }

        public void RemoveMachine(string machine)
        {
            var m = _repository.GetMachineByName(machine);
            _repository.Delete(m);
            _repository.PurgeOrphanedData();
        }

        public void RunSync(bool indexOnly, bool verbose, string profile = null)
        {
            _stopSync = false;
            var fullResults = new FileActionsCount();
            Trace.WriteLine(string.Empty);
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Starting run on {MachineId}");
            var machine = _repository.ListMachines(m => m.Name == _machineId).FirstOrDefault();
            foreach (var opts in _repository.ListLibraries().ToList())
            {
                // If we're looking for a specific profile and this one isn't it, skip it.
                if (!string.IsNullOrEmpty(profile) && !string.Equals(opts.Name, profile, StringComparison.CurrentCultureIgnoreCase)) continue;

                // If any replica of this library is on this machine...
                var repliCount = 0;
                foreach (var replica in _repository.ListReplicas(r => r.MachineId == machine.ID && r.LibraryId == opts.ID).ToArray())
                {
                    repliCount++;
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine($"Processing profile {opts.Name}");

                    VerboseMode = verbose;
                    try
                    {
                        // This is a huge error trap. If this gets triggered, it's pretty catastrophic.
                        var begin = DateTime.Now;
                        var syncResult = Sync(replica, indexOnly);
                        fullResults.Add(syncResult);
                        Trace.WriteLine($"Profile sync time taken: {(DateTime.Now - begin).Verbalise()}");
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Could not sync.");
                        Trace.WriteLine(e.Message);
                        var x = e;
                        while (x != null)
                        {
                            Debug.WriteLine(DateTime.Now.ToString(CultureInfo.CurrentCulture));
                            Debug.WriteLine(x.Message);
                            Debug.WriteLine(x.StackTrace);
                            Debug.WriteLine(string.Empty);

                            x = x.InnerException;
                        }
                    }
                }
                if (repliCount == 0)
                {
                    Trace.WriteLine($"Not participating in profile {opts.Name}");
                }
            }

            if (!indexOnly)
            {
                var begin = DateTime.Now;
                _repository.SaveChanges();
                Debug.WriteLine($"\tSave data: {(DateTime.Now - begin).Verbalise()}");
            }
            Trace.WriteLine(string.Empty);
            Trace.WriteLine("Finished.");
            Trace.WriteLine(fullResults.AnyChanges ? fullResults.GetDisplayString("Totals") : "No actions taken");
        }

        public void RemoveLibrary(string libraryName)
        {
            var library = _repository.ListLibraries(l => l.Name.Equals(libraryName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            _repository.Delete(library);
            _repository.PurgeOrphanedData();
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        private FileActionsCount Sync(Replica replica, bool preview = false)
        {
            var actionsCount = new FileActionsCount();
            // Check folders, just in case.
            var localIndex = _repository.GetFileIndexById(replica.IndexId) ?? new FileIndex();
            var machine = _repository.GetMachineByName(MachineId);
            if (!_fileManager.DirectoryExists(machine.SharedPath))
            {
                Trace.WriteLine($"Shared storage not available ({machine.SharedPath}). Cannot proceed.");
                return actionsCount;
            }
            if (!_fileManager.DirectoryExists(replica.LocalPath))
            {
                Trace.WriteLine($"Local storage not available ({replica.LocalPath}). Cannot proceed.");
                return actionsCount;
            }

            var libraryName = _repository.GetLibraryById(replica.LibraryId).Name;
            var sharedPath = Path.Combine(machine.SharedPath, libraryName);
            _fileManager.EnsureFolder(sharedPath);
            // 1. Compare the primary index to each remote index to determine each file's state.
            var begin = DateTime.Now;
            #region Determine State
            // Pre-process the indexes to ensure the loop doesn't have to.
            var comparisons = new Dictionary<string, ReplicaComparison>();
            foreach (var dex in _repository.GetIndexesByLibraryId(replica.LibraryId))
            {
                foreach (var file in _repository.GetFilesByIndex(dex))
                {
                    var path = file.RelativePath.ToLower();
                    if (!comparisons.ContainsKey(path))
                    {
                        comparisons[path] = new ReplicaComparison {Hash = file.ContentsHash, Count = 0};
                        if (_stopSync) return actionsCount;
                    }
                    comparisons[path].AllSame = comparisons[path].Hash == file.ContentsHash;
                    comparisons[path].Count++;
                }
            }

            var library = _repository.GetLibraryById(replica.LibraryId);
            var primaryIndex = _repository.GetFileIndexById(library.PrimaryIndexId);
            Dictionary<string, FileSystemEntry> primaryIndexFiles = null;
            if (primaryIndex == null)
            {
                // Need to create a new library index.
                primaryIndex = new FileIndex
                {
                    ID = library.PrimaryIndexId,
                    IsDeleted = false,
                    LastModified = DateTime.Now,
                    LibraryId = library.ID,
                    RevisionGuid = Guid.NewGuid(),
                    PrevRevision = null,
                    TimeStamp = DateTime.Now
                };
                _repository.Insert(primaryIndex);
            }

            primaryIndexFiles = _repository.GetFilesByIndex(primaryIndex)
                .ToDictionary(p => p.RelativePath);
            foreach (var mf in primaryIndexFiles.Values)
            {
                var path = mf.RelativePath.ToLower();
                if (mf.State == FileSyncState.Expiring)
                {
                    if (mf is FolderHeader folder && primaryIndexFiles.Values.Any(f => f.State != FileSyncState.Expiring && f.RelativePath.StartsWith(mf.RelativePath)))
                    {
                        // If the folder contains anything that's not deleted, its state is "transit", even if it is marked as deleted.
                        mf.State = FileSyncState.Transit;
                    }
                    else
                    {
                        mf.State = comparisons.ContainsKey(path) ? FileSyncState.Expiring : FileSyncState.Destroyed;
                    }
                }
                else if (comparisons.ContainsKey(path) && comparisons[path].Count == _repository.GetIndexesByLibraryId(library.ID).ToArray().Count() && comparisons[path].AllSame)
                {
                    mf.State = FileSyncState.Synchronised;
                }
                else
                {
                    mf.State = FileSyncState.Transit;
                }
                _repository.Update(mf);
                if (_stopSync) return actionsCount;
            }
            #endregion
            Debug.WriteLine($"\tPrimary index processing: {(DateTime.Now - begin).Verbalise()}");

            // 2. Determine the necessary action for each file.
            begin = DateTime.Now;
            #region Plan actions
            var allHashes = new Dictionary<string, LocalFileHashSet>();
            var localIndexFiles = _repository.ListFileSystemEntries(f => f.IndexId == localIndex.ID).ToArray();
            // Merge the local index, the primary index and the local file system.
            foreach (var f in localIndexFiles)
            {
                if (allHashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allHashes[f.RelativePath.ToLower()].LocalIndexHeader = f;
                    allHashes[f.RelativePath.ToLower()].LocalIndexHash = f.ContentsHash + (f is FolderHeader);
                }
                else
                {
                    allHashes[f.RelativePath.ToLower()] = new LocalFileHashSet { LocalIndexHeader = f, LocalIndexHash = f.ContentsHash + (f is FolderHeader) };
                }
                if (_stopSync) return actionsCount;
            }

            foreach (var f in _repository.GetFilesByIndex(primaryIndex))
            {
                if (allHashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allHashes[f.RelativePath.ToLower()].PrimaryHeader = f;
                    allHashes[f.RelativePath.ToLower()].PrimaryHash = f.ContentsHash + (f is FolderHeader);
                }
                else
                {
                    allHashes[f.RelativePath.ToLower()] = new LocalFileHashSet
                        {PrimaryHeader = f, PrimaryHash = f.ContentsHash + (f is FolderHeader)};
                }

                if (_stopSync) return actionsCount;
            }
            foreach (var f in _fileManager.ListLocalFiles(replica.LocalPath))
            {
                // This action can fail if the file is in use.
                var hed = _fileManager.TryCreateFileHeader(replica.LocalPath, f);
                if (hed == null)
                {
                    // Assume a file access exception. Skip this file by removing it and hope for better luck next run.
                    if (allHashes.ContainsKey(f.ToLower()))
                    {
                        allHashes.Remove(f.ToLower());
                    }

                    Trace.WriteLine($"Skipping locked file: {f}");
                }
                else if (allHashes.ContainsKey(f.ToLower()))
                {
                    allHashes[f.ToLower()].LocalFileHeader = hed;
                    allHashes[f.ToLower()].LocalFileHash = hed.ContentsHash + (hed is FolderHeader);
                }
                else
                {
                    allHashes[f.ToLower()] = new LocalFileHashSet
                    {
                        LocalFileHeader = hed,
                        LocalFileHash = hed.ContentsHash + (hed is FolderHeader)
                    };
                }
                if (_stopSync) return actionsCount;
            }

            // Gather the necessary file actions into queues first, then act and update indexes at the end.
            // One queue per type of action required: copy to local, copy to shared, delete local, delete primary, rename local, no-op.
            var copyToLocal = new List<FileSystemEntry>();
            var copyToShared = new List<FileSystemEntry>();
            var deleteLocal = new List<FileSystemEntry>();
            var deletePrimary = new List<FileSystemEntry>();
            var renameLocal = new List<FileSystemEntry>(); // Target names to be generated on demand when processing.
            var noAction = new List<FileSystemEntry>(); // Nothing to do to these, but keep a list in case of verbose preview.
            // Copy the collection for looping, so we can modify it in case of conflicts.
            foreach (var dex in allHashes.Keys)
            {
                var f = allHashes[dex];
                if (f.PrimaryHeader != null)
                {
                    switch (f.PrimaryHeader.State)
                    {
                        case FileSyncState.Synchronised:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    // Up to date. No action required.
                                    Debug.WriteLine($"SYNCED: {f.PrimaryHeader.RelativePath}");
                                    noAction.Add(f.PrimaryHeader);
                                }
                                else
                                {
                                    // Local update. Copy to shared. Update indexes.
                                    Debug.WriteLine($"UPDATED [-> SHARE]: {dex}");
                                    copyToShared.Add(f.LocalFileHeader);
                                }
                            }
                            else
                            {
                                Debug.WriteLine($"DELETED [DELETE ->]: {dex}");
                                // Local delete. Mark deleted in primary index. Remove from local index.
                                deletePrimary.Add(f.PrimaryHeader);
                            }
                            break;
                        case FileSyncState.Expiring:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    Debug.WriteLine($"EXPIRED [<- DELETE]: {dex}");
                                    // Remote delete. Delete file. Remove from local index.
                                    deleteLocal.Add(f.PrimaryHeader);
                                }
                                else
                                {
                                    // Update/delete conflict. Copy local to shared. Update local index. Mark not deleted in primary index.
                                    Debug.WriteLine($"UN-EXPIRED [-> SHARE]: {dex}");
                                    copyToShared.Add(f.LocalFileHeader);
                                }
                            }
                            else
                            {
                                // Already deleted. Just make sure it's not in the local index.
                                if (f.LocalIndexHeader != null)
                                {
                                    Debug.WriteLine($"SIDE-CHANNEL DELETE: {dex}");
                                    noAction.Add(f.PrimaryHeader);
                                }
                                else
                                {
                                    Debug.WriteLine($"ALREADY DELETED: {dex}");
                                    noAction.Add(f.PrimaryHeader);
                                }
                            }
                            break;
                        case FileSyncState.Transit:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    if (f.PrimaryHash == f.LocalIndexHash)
                                    {
                                        Debug.WriteLine($"TRANSIT [-> SHARE]: {dex}");
                                        // Up to date locally. Copy to shared to propagate changes.
                                        copyToShared.Add(f.LocalFileHeader);
                                    }
                                    else
                                    {
                                        Debug.WriteLine($"TRANSIT [LOCAL <-]: {dex}");
                                        // Remote update. Copy to local, update local index.
                                        copyToLocal.Add(f.PrimaryHeader);
                                    }
                                }
                                else
                                {
                                    if (f.PrimaryHash == f.LocalFileHash)
                                    {
                                        // Side-channel update. Repair the local index.
                                        Debug.WriteLine($"SIDE-CHANNEL UPDATED: {dex}");
                                        noAction.Add(f.PrimaryHeader);
                                    }
                                    else
                                    {
                                        if (f.PrimaryHash == f.LocalIndexHash)
                                        {
                                            // Local update. Copy to shared. Update local and primary indexes.
                                            Debug.WriteLine($"UPDATED [-> SHARE]: {dex}");
                                            copyToShared.Add(f.LocalFileHeader);
                                        }
                                        else
                                        {
                                            Debug.WriteLine($"CONFLICT [-> SHARE]: {dex}");
                                            // Update conflict. Rename the local file and add to the processing queue.
                                            renameLocal.Add((FileSystemEntry) f.PrimaryHeader.Clone());
                                            copyToLocal.Add(f.PrimaryHeader);
                                        }
                                        // TODO: Detect out-of-date copies and treat them as remote updates.
                                        // Use a history of contents hashes to detect old versions.
                                    }
                                }
                            }
                            else if (f.LocalIndexHeader != null && f.LocalIndexHash == f.PrimaryHash)
                            {
                                // Deleted while in transit - that is, before all machines had a copy.
                                Debug.WriteLine($"DELETED [-> CANCEL TRANSIT]: {dex}");
                                deletePrimary.Add(f.PrimaryHeader);
                            }
                            else if (_fileManager.FileExists(sharedPath, dex) || (f.PrimaryHeader is FolderHeader))
                            {
                                Debug.WriteLine($"TRANSIT [LOCAL <-]: {dex}");
                                // Update/delete conflict or remote create. Copy to local. Update local index.
                                copyToLocal.Add(f.PrimaryHeader);
                            }
                            else if (f.LocalIndexHeader != null)
                            {
                                // File does not exist on local file system or in shared folder. Remove it from local index.
                                Debug.WriteLine($"MISSING: {dex}");
                                noAction.Add(f.PrimaryHeader);
                            }
                            else
                            {
                                // Not on local drive.
                                // Not in shared folder.
                                // Not in local index.
                                // Remote create that can't be propagated yet.
                                Debug.WriteLine($"PHANTOM: {dex}");
                                noAction.Add(f.PrimaryHeader);
                            }
                            break;
                    }
                }
                else if (f.LocalFileHeader != null)
                {
                    // Does every other remote index have the same version of the file?
                    var libraryIndexes = _repository.ListIndexes(i => i.LibraryId == replica.LibraryId && i.ReplicaId.HasValue);
                    if (libraryIndexes.Count() == 1 || (comparisons.ContainsKey(dex) && comparisons[dex].AllSame))
                    {
                        // Side-channel or initial create. Just write the primary index.
                        Debug.WriteLine($"INITIAL CREATE: {dex}");
                        noAction.Add(f.LocalFileHeader);
                        _repository.CopyFileSystemEntry(f.LocalFileHeader, primaryIndex.ID);
                    }
                    else
                    {
                        // Local create. Copy to shared. Add to indexes.
                        Debug.WriteLine($"NEW [-> SHARE]: {dex}");
                        copyToShared.Add(f.LocalFileHeader);
                    }
                }

                if (_stopSync)
                {
                    return actionsCount;
                }
            }
            #endregion

            actionsCount.RenameLocalCount = renameLocal.Count;
            actionsCount.DeleteLocalCount = deleteLocal.Count;
            actionsCount.CopyToLocalCount = copyToLocal.Count;
            actionsCount.DeletePrimaryCount = deletePrimary.Count;
            actionsCount.NoActionCount = noAction.Count;
            actionsCount.CopyToSharedCount = copyToShared.Count;
            if (actionsCount.AnyChanges)
            {
                Trace.WriteLine("");
                Trace.WriteLine(actionsCount.GetDisplayString(replica.LocalPath));
                Trace.WriteLine("");
            }
            else
            {
                Trace.WriteLine($"{noAction.Count} files, no changes.");
            }
            Debug.WriteLine($"\tQueue generation: {(DateTime.Now - begin).Verbalise()}");

            // 3. Process the action queue according to the mode and limitations in place.
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Processing file actions for '{replica.LocalPath}' on {MachineId}:");
            var errorList = new List<string>();
            if (!preview)
            {
                begin = DateTime.Now;
                #region 3.1. Rename, Copy to Local, Delete local, Delete primary
                foreach (var r in renameLocal)
                {
                    var newName = _fileManager.GetConflictFileName(Path.Combine(replica.LocalPath, r.RelativePath), MachineId, DateTime.Now);
                    var newRelativePath = _fileManager.GetRelativePath(newName, replica.LocalPath);
                    var fmr = _fileManager.MoveFile(Path.Combine(replica.LocalPath, r.RelativePath), Path.Combine(replica.LocalPath, newRelativePath), false);
                    if (fmr == FileCommandResult.Failure)
                    {
                        errorList.Add($"File rename failed: {r.RelativePath}");
                    }
                    else
                    {
                        r.RelativePath = newRelativePath;
                        copyToShared.Add(r);
                    }

                    if (_stopSync)
                    {
                        return actionsCount;
                    }
                }
                foreach (var d in copyToLocal.OrderBy(i => i.RelativePath.Length))
                {
                    if (d is FolderHeader)
                    {
                        _fileManager.EnsureFolder(Path.Combine(replica.LocalPath, d.RelativePath));
                    }
                    else
                    {
                        var fcr = _fileManager.CopyFile(sharedPath, d.RelativePath, replica.LocalPath);
                        if (fcr == FileCommandResult.Failure)
                        {
                            errorList.Add($"Inbound file copy failed: {d.RelativePath}");
                        }
                    }
                    if (_stopSync)
                    {
                        return actionsCount;
                    }
                }
                foreach (var d in deleteLocal.OrderByDescending(f => f.RelativePath.Length)) // Simplest way to delete deepest-first.
                {
                    var result = _fileManager.Delete(Path.Combine(replica.LocalPath, d.RelativePath));
                    if (result != FileCommandResult.Failure || !_fileManager.DirectoryExists(Path.Combine(replica.LocalPath, d.RelativePath)))
                    {
                    }

                    if (_stopSync)
                    {
                        return actionsCount;
                    }

                }

                // Push.
                foreach (var m in deletePrimary.OrderByDescending(f => f.RelativePath.Length))
                {
                    var mf = _repository.ListFileSystemEntries(f =>
                        f.RelativePath.Equals(m.RelativePath, StringComparison.CurrentCultureIgnoreCase) &&
                        f.IndexId == primaryIndex.ID).FirstOrDefault();
                    if (mf != null)
                    {
                        mf.State = FileSyncState.Expiring;
                        _repository.Update(mf);
                    }
                    else
                    {
                        m.State = FileSyncState.Expiring;
                        _repository.CopyFileSystemEntry(m, primaryIndex.ID);
                    }

                    if (_stopSync)
                    {
                        return actionsCount;
                    }

                }
                #endregion
                Debug.WriteLine($"\tInbound actions: {(DateTime.Now - begin).Verbalise()}");
                begin = DateTime.Now;
                #region 3.2. Regenerate the local index.
                // Keep a copy of the old index in case of read failures.
                // TODO: Update the index instead of a full replace. Replacing with a new index doesn't make as much sense now.
                var oldIndex = _repository.ListFileSystemEntries(f => f.IndexId == localIndex.ID).ToDictionary(entry => entry.RelativePath);
                var nuDex = new FileIndex
                {
                    LibraryId = replica.LibraryId, ReplicaId = replica.ID, TimeStamp = DateTime.Now, ID = Guid.NewGuid()
                };
                foreach (var f in _fileManager.ListLocalFiles(replica.LocalPath))
                {
                    try
                    {
                        var onDisk = _fileManager.CreateFileHeader(replica.LocalPath, f);
                        onDisk.IndexId = nuDex.ID;
                        _repository.Insert(onDisk);
                    }
                    catch
                    {
                        // Keep the old file.
                        oldIndex[f].IndexId = nuDex.ID;
                        _repository.Update(oldIndex[f]);
                    }
                    if (_stopSync)
                    {
                        return actionsCount;
                    }

                }
                _repository.Insert(nuDex);
                replica.IndexId = nuDex.ID;
                _repository.Update(replica);
                // Perhaps: _repository.PurgeOrphanedData();
                foreach (var expiredIndex in _repository.ListIndexes()
                    .Where(e => e.ReplicaId == replica.ID && e.LastModified < nuDex.LastModified).ToArray())
                {
                    _repository.Delete(expiredIndex);
                }
                #endregion
                Debug.WriteLine($"\tLocal index regeneration: {(DateTime.Now - begin).Verbalise()}");
                begin = DateTime.Now;
                #region 3.3 Clean up the primary index and shared folder.
                // Pre-examine the indexes, for efficiency.
                comparisons = new Dictionary<string, ReplicaComparison>();
                foreach (var dex in _repository.GetIndexesByLibraryId(replica.LibraryId))
                {
                    foreach (var file in _repository.GetFilesByIndex(dex))
                    {
                        var path = file.RelativePath.ToLower();
                        if (!comparisons.ContainsKey(path))
                        {
                            comparisons[path] = new ReplicaComparison
                            {
                                Count = 0,
                                Hash = file.ContentsHash,
                                AllSame = true,
                                AllHashes = new List<string>()
                            };
                        }
                        comparisons[path].Count++;
                        comparisons[path].AllSame = comparisons[path].Hash == file.ContentsHash;
                        comparisons[path].AllHashes.Add(file.ContentsHash);
                        if (_stopSync)
                        {
                            return actionsCount;
                        }
                    }
                }

                foreach (var mf in _repository.GetFilesByIndex(primaryIndex).ToArray())
                {
                    var key = mf.RelativePath.ToLower();
                    var shareFileHead = _fileManager.TryCreateFileHeader(sharedPath, mf.RelativePath); // Returns null if not found.
                    try
                    {
                        if (mf.State == FileSyncState.Expiring)
                        {
                            if (!comparisons.ContainsKey(key))
                            {
                                // Marked deleted and has been removed from every replica.
                                Debug.WriteLine($"DESTROYED: {mf.RelativePath}");
                                _repository.Delete(mf);
                            }
                        }
                        else if (shareFileHead != null && !mf.Matches(shareFileHead))
                            // else if (_fileManager.FileExists(SharedPath, mf.RelativePath) && !syncSet.PrimaryIndex.MatchesFile(_fileManager.CreateFileHeader(SharedPath, mf.RelativePath)))
                        {
                            // The shared file does not match the primary index. It should be removed.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (shareFileHead != null && comparisons.ContainsKey(key) && comparisons[key].Count == _repository.GetIndexesByLibraryId(library.ID).ToArray().Count() && comparisons[key].AllSame && comparisons[key].Hash == mf.ContentsHash)
                        {
                            // Successfully transmitted to every replica. Remove from shared storage.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (!comparisons.ContainsKey(key))
                        {
                            // Simultaneous side-channel delete from every replica. This file is toast.
                            _repository.Delete(mf);
                        }
                        else if (comparisons[key].AllHashes.All(h => h != mf.ContentsHash))
                        {
                            // Slightly more subtle. No physical matching version of this file exists any more.
                            _repository.Delete(mf);
                        }
                    }
                    catch
                    {
                        // TODO: Avoid throwing exceptions from fileManager.Delete operations. If the file doesn't exist, that's success, not failure.
                    }
                    if (_stopSync)
                    {
                        return actionsCount;
                    }
                }
                primaryIndexFiles = _repository.ListFileSystemEntries(f => f.IndexId == primaryIndex.ID)
                    .ToDictionary(p => p.RelativePath);
                // Clean up shared storage. We already looked for primary/shared mismatches.
                var shareCleanMeta = from s in _fileManager.ListLocalFiles(sharedPath)
                                     where File.Exists(Path.Combine(sharedPath, s)) && (
                                         primaryIndexFiles != null && (!primaryIndexFiles.TryGetValue(s, out var minx) || minx.State == FileSyncState.Expiring))
                                     select s;
                // For every file now in shared storage,
                foreach (var s in shareCleanMeta)
                {
                    // Shared file is missing from primary index. Get rid of it.
                    _fileManager.Delete(Path.Combine(sharedPath, s));
                    Debug.WriteLine($"SHARE CLEANUP: {s}");
                    if (_stopSync)
                    {
                        return actionsCount;
                    }

                }

                // Remove empty subfolders from the Shared folder.
                // Somehow I always find myself rewriting this exact code.
                if (Directory.Exists(sharedPath))
                {
                    // Sort by length descending to get leaf nodes first.
                    var allDirs = from s in Directory.GetDirectories(sharedPath, "*", SearchOption.AllDirectories)
                                  where s != sharedPath // Don't kill the root folder.
                                  orderby s.Length descending
                                  select s;
                    foreach (string t in allDirs)
                    {
                        try
                        {
                            if (Directory.GetFiles(t).Length == 0 && Directory.GetDirectories(t).Length == 0)
                            {
                                Directory.Delete(t);
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine($"Could not delete {t}: {e.Message}");
                        }
                        if (_stopSync)
                        {
                            _repository.Update(replica);
                            return actionsCount;
                        }

                    }
                }
                #endregion
                Debug.WriteLine($"\tPrimary index cleanup: {(DateTime.Now - begin).Verbalise()}");
                begin = DateTime.Now;
                #region 3.4. Copy to shared
                var sharedSize = _fileManager.SharedPathSize(sharedPath);
                // Check the drive's available space (ie DriveInfo.AvailableFreeSpace) to keep from using more than 90% of the total space, regardless of reserve.
                var flashDrive = new DriveInfo(Path.GetPathRoot(Path.GetFullPath(sharedPath)));
                var carefulSpace = Math.Min(flashDrive.AvailableFreeSpace - 0.1 * flashDrive.TotalSize, library.MaxSharedSize);
                foreach (var s in copyToShared.OrderBy(f => f.RelativePath.Length))
                {
                    if (s != null)
                    {
                        var mf = _repository.ListFileSystemEntries(f =>
                            f.RelativePath.Equals(s.RelativePath, StringComparison.CurrentCultureIgnoreCase) &&
                            f.IndexId == primaryIndex.ID).FirstOrDefault();
                        if (sharedSize + (ulong)s.Size < carefulSpace)
                        {
                            Debug.WriteLine($"Copying {s.RelativePath} to {sharedPath}");
                            if (s is FolderHeader)
                            {
                                if (mf != null)
                                {
                                    mf.State = FileSyncState.Transit; // To un-delete folders that were being removed, but have been re-created.
                                    _repository.Update(mf);
                                }
                            }
                            else
                            {
                                var result = _fileManager.CopyFile(replica.LocalPath, s.RelativePath, sharedPath);
                                // Check for success.
                                if (result == FileCommandResult.Success || result == FileCommandResult.Async)
                                {
                                    // Assume potential success on Async.
                                    sharedSize += (ulong)s.Size;
                                    if (mf == null)
                                    {
                                        _repository.CopyFileSystemEntry(s, primaryIndex.ID);
                                    }
                                    else
                                    {
                                        mf.ContentsHash = s.ContentsHash;
                                        mf.State = FileSyncState.Transit;
                                        _repository.Update(mf);
                                    }
                                }
                                else
                                {
                                    Trace.WriteLine($"Outbound file copy failed: {s.RelativePath}");
                                }
                            }
                        }
                        else if ((ulong)s.Size > library.MaxSharedSize)
                        {
                            // TODO: split the file and copy in pieces, because it will never fit.
                        }
                        // Else there is no room yet. Better luck next time.
                    }
                    else
                    {
                        Trace.WriteLine("Error: Null file name scheduled for copying to shared storage.");
                    }
                    if (_stopSync)
                    {
                        return actionsCount;
                    }

                }
                #endregion
                Debug.WriteLine($"\tOutbound actions: {(DateTime.Now - begin).Verbalise()}");

                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Empty);
            }

            foreach (var e in errorList)
            {
                Trace.WriteLine(e);
            }

            _repository.Update(replica);
            return actionsCount;
        }

        public void Save()
        {
            _repository.SaveChanges();
        }

        public void StopSync()
        {
            _stopSync = true;
        }

        public void AddMachine(string machineName, string sharedPath)
        {
            var machine = new Machine();
            machine.Name = machineName;
            machine.SharedPath = sharedPath;
            _repository.Insert(machine);
        }

        public void DeleteFile(string libraryName, string filePath)
        {
            var library = GetLibrary(libraryName);
            var file = _repository.GetFileByPath(library.PrimaryIndexId, filePath);
            if (file != null)
            {
                file.State = FileSyncState.Expiring;
                _repository.Update(file);
            }
        }

        public void UndeleteFile(string libraryName, string filePath)
        {
            var library = GetLibrary(libraryName);
            var file = _repository.GetFileByPath(library.PrimaryIndexId, filePath);
            if (file != null)
            {
                file.State = FileSyncState.Transit;
                _repository.Update(file);
            }
        }

        public void MoveReplica(Guid replicaId, string newPath, bool moveFiles)
        {
            var replica = _repository.GetReplicaById(replicaId);
            if (moveFiles)
            {
                Directory.Move(replica.LocalPath, newPath);
            }

            replica.LocalPath = newPath;
            _repository.Update(replica);
        }
        #endregion

        #region Properties
        private string _machineId;
        public string MachineId
        {
            get => _machineId;
            set
            {
                _machineId = value;
                NotifyPropertyChanged();
            }
        }

        private class ReplicaComparison
        {
            internal bool AllSame;
            internal string Hash;
            internal int Count;
            internal List<string> AllHashes;
        }

        private class LocalFileHashSet
        {
            internal FileSystemEntry LocalFileHeader;
            internal string LocalFileHash;
            internal FileSystemEntry LocalIndexHeader;
            internal string LocalIndexHash;
            internal FileSystemEntry PrimaryHeader;
            internal string PrimaryHash;
        }
        #endregion
    }
}


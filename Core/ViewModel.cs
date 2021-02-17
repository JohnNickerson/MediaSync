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

        private readonly SyncSetRepository _repository;

        private bool _stopSync;

        #endregion

        #region Constructors
        public ViewModel(ISyncSetMapper dataContext, string machineId, IFileManager fileManager)
        {
            _repository = new SyncSetRepository(dataContext);
            _repository.FindAll();
            _machineId = machineId;
            _fileManager = fileManager;
        }
        #endregion

        #region Methods
        public void CreateProfile(string name, ulong reserve)
        {
            if (!ProfileExists(name))
            {
                var profile = new SyncSet
                {
                    Name = name,
                    ReserveSpace = reserve,
                    Indexes = new Dictionary<string, FileIndex>(StringComparer.CurrentCultureIgnoreCase),
                    PrimaryIndex = new FileIndex( )
                };
                _repository.Update(profile);
                _repository.SaveChanges();
            }
            else
            {
                Trace.WriteLine("Profile name already exists: " + name);
            }
        }

        public void ResizeProfile(string name, ulong reserve)
        {
            if (ProfileExists(name))
            {
                var profile = GetProfile(name);
                profile.ReserveSpace = reserve;
                _repository.Update(profile);
                _repository.SaveChanges();
            }
        }

        private bool ProfileExists(string name)
        {
            return _repository.Items.Any(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        private SyncSet GetProfile(string name)
        {
            return _repository.Items.FirstOrDefault(x => String.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void JoinProfile(string profileName, string localPath, string sharedPath)
        {
            var profile = GetProfile(profileName);
            if (profile == null)
            {
                Trace.WriteLine($"Profile does not exist: {profileName}");
            }
            else
            {
                JoinProfile(profile, localPath, sharedPath);
            }
        }

        private void JoinProfile(SyncSet profile, string localPath, string sharedPath)
        {
            if (!profile.ContainsParticipant(MachineId))
            {
                profile.Indexes[MachineId] = new FileIndex
                {
                    MachineName = MachineId,
                    LocalPath = localPath,
                    SharedPath = sharedPath
                };
                _repository.Update(profile);
            }
            else
            {
                var i = profile.GetIndex(MachineId);
                i.LocalPath = localPath;
                i.SharedPath = sharedPath;
                profile.UpdateIndex(i);
                _repository.Update(profile);
            }
            _repository.SaveChanges();
        }

        private void LeaveProfile(SyncSet profile, string machine)
        {
            if (profile.ContainsParticipant(machine))
            {
                profile.Indexes.Remove(machine);
                _repository.Update(profile);
                _repository.SaveChanges();
            }
        }

        public void LeaveProfile(string profileName, string machine)
        {
            var profile = GetProfile(profileName);
            LeaveProfile(profile, machine);
        }

        public Dictionary<string, SyncSet> Profiles => _repository.Items.ToDictionary(k => k.Name, v => v);

        public List<string> Machines
        {
            get
            {
                return _repository.Items.SelectMany(p => p.Indexes).Select(p => p.Value.MachineName).Distinct().ToList();
            }
        }

        public void RemoveMachine(string machine)
        {
            foreach (var p in Profiles)
            {
                LeaveProfile(p.Key, machine);
            }
        }

        public void RunSync(bool indexOnly, bool verbose, string profile = null)
        {
            _stopSync = false;
            var fullResults = new FileActionsCount();
            Trace.WriteLine(string.Empty);
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Starting run on {MachineId}");
            foreach (var opts in _repository.Items.ToList())
            {
                // If we're looking for a specific profile and this one isn't it, skip it.
                if (!string.IsNullOrEmpty(profile) && !string.Equals(opts.Name, profile, StringComparison.CurrentCultureIgnoreCase)) continue;

                if (opts.ContainsParticipant(_machineId))
                {
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine($"Processing profile {opts.Name}");

                    VerboseMode = verbose;
                    try
                    {
                        // This is a huge error trap. If this gets triggered, it's pretty catastrophic.
                        var begin = DateTime.Now;
                        var syncResult = Sync(opts, indexOnly);
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
                else
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

        public void RemoveProfile(string profileName)
        {
            _repository.Delete(_repository.Find(profileName));
            _repository.SaveChanges();
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        private FileActionsCount Sync(SyncSet syncSet, bool preview = false)
        {
            var actionsCount = new FileActionsCount();
            // Check folders, just in case.
            var localIndex = syncSet.GetIndex(MachineId) ?? new FileIndex();
            var sharedPath = localIndex.SharedPath;
            if (!_fileManager.DirectoryExists(sharedPath))
            {
                Trace.WriteLine($"Shared storage not available ({sharedPath}). Cannot proceed.");
                return actionsCount;
            }
            if (!_fileManager.DirectoryExists(localIndex.LocalPath))
            {
                Trace.WriteLine($"Local storage not available ({localIndex.LocalPath}). Cannot proceed.");
                return actionsCount;
            }

            // 1. Compare the primary index to each remote index to determine each file's state.
            var begin = DateTime.Now;
            #region Determine State
            // Pre-process the indexes to ensure the loop doesn't have to.
            var comparisons = new Dictionary<string, ReplicaComparison>();
            foreach (var dex in syncSet.Indexes.Values)
            {
                foreach (var file in dex.Files.Values)
                {
                    var path = file.RelativePath.ToLower();
                    if (!comparisons.ContainsKey(path))
                    {
                        comparisons[path] = new ReplicaComparison { Hash = file.ContentsHash, Count = 0 };
                        if (_stopSync) return actionsCount;
                    }
                    comparisons[path].AllSame = comparisons[path].Hash == file.ContentsHash;
                    comparisons[path].Count++;
                }
            }
            foreach (var mf in syncSet.PrimaryIndex.Files.Values)
            {
                var path = mf.RelativePath.ToLower();
                if (mf.IsDeleted)
                {
                    if (mf.IsFolder && syncSet.PrimaryIndex.Files.Values.Any(f => f.IsDeleted == false && f.RelativePath.StartsWith(mf.RelativePath)))
                    {
                        // If the folder contains anything that's not deleted, its state is "transit", even if it is marked as deleted.
                        mf.State = FileSyncState.Transit;
                    }
                    else
                    {
                        mf.State = comparisons.ContainsKey(path) ? FileSyncState.Expiring : FileSyncState.Destroyed;
                    }
                }
                else if (comparisons.ContainsKey(path) && comparisons[path].Count == syncSet.Indexes.Count && comparisons[path].AllSame)
                {
                    mf.State = FileSyncState.Synchronised;
                }
                else
                {
                    mf.State = FileSyncState.Transit;
                }
                if (_stopSync) return actionsCount;
            }
            #endregion
            Debug.WriteLine($"\tPrimary index processing: {(DateTime.Now - begin).Verbalise()}");

            // 2. Determine the necessary action for each file.
            begin = DateTime.Now;
            #region Plan actions
            var allHashes = new Dictionary<string, LocalFileHashSet>();
            // Merge the local index, the primary index and the local file system.
            foreach (var f in localIndex.Files.Values)
            {
                if (allHashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allHashes[f.RelativePath.ToLower()].LocalIndexHeader = f;
                    allHashes[f.RelativePath.ToLower()].LocalIndexHash = f.ContentsHash + f.IsFolder;
                }
                else
                {
                    allHashes[f.RelativePath.ToLower()] = new LocalFileHashSet { LocalIndexHeader = f, LocalIndexHash = f.ContentsHash + f.IsFolder };
                }
                if (_stopSync) return actionsCount;
            }
            foreach (var f in syncSet.PrimaryIndex.Files.Values)
            {
                if (allHashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allHashes[f.RelativePath.ToLower()].PrimaryHeader = f;
                    allHashes[f.RelativePath.ToLower()].PrimaryHash = f.ContentsHash + f.IsFolder;
                }
                else
                {
                    allHashes[f.RelativePath.ToLower()] = new LocalFileHashSet { PrimaryHeader = f, PrimaryHash = f.ContentsHash + f.IsFolder };
                }
                if (_stopSync) return actionsCount;
            }
            foreach (var f in _fileManager.ListLocalFiles(localIndex.LocalPath))
            {
                // This action can fail if the file is in use.
                var hed = _fileManager.TryCreateFileHeader(localIndex.LocalPath, f);
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
                    allHashes[f.ToLower()].LocalFileHash = hed.ContentsHash + hed.IsFolder;
                }
                else
                {
                    allHashes[f.ToLower()] = new LocalFileHashSet
                    {
                        LocalFileHeader = hed,
                        LocalFileHash = hed.ContentsHash + hed.IsFolder
                    };
                }
                if (_stopSync) return actionsCount;
            }

            // Gather the necessary file actions into queues first, then act and update indexes at the end.
            // One queue per type of action required: copy to local, copy to shared, delete local, delete primary, rename local, no-op.
            var copyToLocal = new List<FileHeader>();
            var copyToShared = new List<FileHeader>();
            var deleteLocal = new List<FileHeader>();
            var deletePrimary = new List<FileHeader>();
            var renameLocal = new List<FileHeader>(); // Target names to be generated on demand when processing.
            var noAction = new List<FileHeader>(); // Nothing to do to these, but keep a list in case of verbose preview.
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
                                            renameLocal.Add(f.PrimaryHeader.Clone());
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
                            else if (_fileManager.FileExists(sharedPath, dex) || f.PrimaryHeader.IsFolder)
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
                    if (syncSet.Indexes.Count == 1 || (comparisons.ContainsKey(dex) && comparisons[dex].AllSame))
                    {
                        // Side-channel or initial create. Just write the primary index.
                        Debug.WriteLine($"INITIAL CREATE: {dex}");
                        noAction.Add(f.LocalFileHeader);
                        syncSet.PrimaryIndex.UpdateFile(f.LocalFileHeader);
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
                    _repository.Update(syncSet);
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
                Trace.WriteLine(actionsCount.GetDisplayString(syncSet.Name));
                Trace.WriteLine("");
            }
            else
            {
                Trace.WriteLine($"{noAction.Count} files, no changes.");
            }
            Debug.WriteLine($"\tQueue generation: {(DateTime.Now - begin).Verbalise()}");

            // 3. Process the action queue according to the mode and limitations in place.
            Trace.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Processing file actions for '{syncSet.Name}' on {MachineId}:");
            var errorList = new List<string>();
            if (!preview)
            {
                begin = DateTime.Now;
                #region 3.1. Rename, Copy to Local, Delete local, Delete primary
                foreach (var r in renameLocal)
                {
                    var newName = _fileManager.GetConflictFileName(Path.Combine(localIndex.LocalPath, r.RelativePath), MachineId, DateTime.Now);
                    var newRelativePath = _fileManager.GetRelativePath(newName, localIndex.LocalPath);
                    var fmr = _fileManager.MoveFile(Path.Combine(localIndex.LocalPath, r.RelativePath), Path.Combine(localIndex.LocalPath, newRelativePath), false);
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
                        _repository.Update(syncSet);
                        return actionsCount;
                    }
                }
                foreach (var d in copyToLocal.Where(i => i.IsFolder))
                {
                    _fileManager.EnsureFolder(Path.Combine(localIndex.LocalPath, d.RelativePath));
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }
                foreach (var c in copyToLocal.Where(i => !i.IsFolder))
                {
                    var fcr = _fileManager.CopyFile(sharedPath, c.RelativePath, localIndex.LocalPath);
                    if (fcr == FileCommandResult.Failure)
                    {
                        errorList.Add($"Inbound file copy failed: {c.RelativePath}");
                    }
                    else
                    {
                    }
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }
                foreach (var d in deleteLocal.OrderByDescending(f => f.RelativePath.Length)) // Simplest way to delete deepest-first.
                {
                    var result = _fileManager.Delete(Path.Combine(localIndex.LocalPath, d.RelativePath));
                    if (result != FileCommandResult.Failure || !_fileManager.DirectoryExists(Path.Combine(localIndex.LocalPath, d.RelativePath)))
                    {
                    }

                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }

                // Push.
                foreach (var m in deletePrimary)
                {
                    m.IsDeleted = true;
                    syncSet.PrimaryIndex.UpdateFile(m);
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }
                #endregion
                Debug.WriteLine($"\tInbound actions: {(DateTime.Now - begin).Verbalise()}");
                begin = DateTime.Now;
                #region 3.2. Regenerate the local index.
                // Keep a copy of the old index in case of read failures.
                var oldIndex = new FileIndex();
                foreach (var f in localIndex.Files)
                {
                    oldIndex.Files.Add(f.Key, f.Value);
                }
                localIndex.Files.Clear();
                foreach (var f in _fileManager.ListLocalFiles(localIndex.LocalPath))
                {
                    try
                    {
                        localIndex.UpdateFile(_fileManager.CreateFileHeader(localIndex.LocalPath, f));
                    }
                    catch
                    {
                        // Keep the old file.
                        localIndex.UpdateFile(oldIndex.GetFile(f));
                    }
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }
                localIndex.TimeStamp = DateTime.Now;
                syncSet.UpdateIndex(localIndex);
                #endregion
                Debug.WriteLine($"\tLocal index regeneration: {(DateTime.Now - begin).Verbalise()}");
                begin = DateTime.Now;
                #region 3.3 Clean up the primary index and shared folder.
                // Pre-examine the indexes, for efficiency.
                comparisons = new Dictionary<string, ReplicaComparison>();
                foreach (var dex in syncSet.Indexes.Values)
                {
                    foreach (var file in dex.Files.Values)
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
                            _repository.Update(syncSet);
                            return actionsCount;
                        }

                    }
                }

                foreach (var mf in syncSet.PrimaryIndex.Files.Values.ToArray())
                {
                    var key = mf.RelativePath.ToLower();
                    var shareFileHead = _fileManager.TryCreateFileHeader(sharedPath, mf.RelativePath); // Returns null if not found.
                    try
                    {
                        if (mf.IsDeleted)
                        {
                            if (!comparisons.ContainsKey(key))
                            {
                                // Marked deleted and has been removed from every replica.
                                Debug.WriteLine($"DESTROYED: {mf.RelativePath}");
                                syncSet.PrimaryIndex.Remove(mf);
                            }
                        }
                        else if (shareFileHead != null && !syncSet.PrimaryIndex.MatchesFile(shareFileHead))
                        // else if (_fileManager.FileExists(SharedPath, mf.RelativePath) && !syncSet.PrimaryIndex.MatchesFile(_fileManager.CreateFileHeader(SharedPath, mf.RelativePath)))
                        {
                            // The shared file does not match the primary index. It should be removed.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (_fileManager.FileExists(sharedPath, mf.RelativePath) && comparisons.ContainsKey(key) && comparisons[key].Count == syncSet.Indexes.Count && comparisons[key].AllSame && comparisons[key].Hash == mf.ContentsHash)
                        {
                            // Successfully transmitted to every replica. Remove from shared storage.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (!comparisons.ContainsKey(key))
                        {
                            // Simultaneous side-channel delete from every replica. This file is toast.
                            syncSet.PrimaryIndex.Remove(mf);
                        }
                        else if (comparisons[key].AllHashes.All(h => h != mf.ContentsHash))
                        {
                            // Slightly more subtle. No physical matching version of this file exists any more.
                            syncSet.PrimaryIndex.Remove(mf);
                        }
                    }
                    catch
                    {
                        // TODO: Avoid throwing exceptions from fileManager.Delete operations. If the file doesn't exist, that's success, not failure.
                    }
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
                        return actionsCount;
                    }

                }
                // Clean up shared storage. We already looked for primary/shared mismatches.
                var minx = syncSet.PrimaryIndex;
                var shareCleanMeta = from s in _fileManager.ListLocalFiles(sharedPath)
                                     where File.Exists(Path.Combine(sharedPath, s)) && (!minx.Files.ContainsKey(s) || minx.GetFile(s).IsDeleted)
                                     select s;
                // For every file now in shared storage,
                foreach (var s in shareCleanMeta)
                {
                    // Shared file is missing from primary index. Get rid of it.
                    _fileManager.Delete(Path.Combine(sharedPath, s));
                    Debug.WriteLine($"SHARE CLEANUP: {s}");
                    if (_stopSync)
                    {
                        _repository.Update(syncSet);
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
                            _repository.Update(syncSet);
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
                var carefulSpace = Math.Min(flashDrive.AvailableFreeSpace - 0.1 * flashDrive.TotalSize, syncSet.ReserveSpace);
                foreach (var s in copyToShared)
                {
                    if (s != null)
                    {
                        if (sharedSize + (ulong)s.Size < carefulSpace)
                        {
                            Debug.WriteLine($"Copying {s.RelativePath} to {sharedPath}");
                            if (s.IsFolder)
                            {
                                s.IsDeleted = false; // To un-delete folders that were being removed, but have been re-created.
                                syncSet.PrimaryIndex.UpdateFile(s);
                            }
                            else
                            {
                                var result = _fileManager.CopyFile(localIndex.LocalPath, s.RelativePath, sharedPath);
                                // Check for success.
                                if (result == FileCommandResult.Success || result == FileCommandResult.Async)
                                {
                                    // Assume potential success on Async.
                                    sharedSize += (ulong)s.Size;
                                    syncSet.PrimaryIndex.UpdateFile(s);
                                }
                                else
                                {
                                    Trace.WriteLine($"Outbound file copy failed: {s.RelativePath}");
                                }
                            }
                        }
                        else if ((ulong)s.Size > syncSet.ReserveSpace)
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
                        _repository.Update(syncSet);
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

            _repository.Update(syncSet);
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

        public void ChangeDriveLetter(DriveInfo newDrive, string machineName)
        {
            foreach (var profile in Profiles.Values)
            {
                if (profile.ContainsParticipant(machineName))
                {
                    var newSharedPath = (newDrive.RootDirectory.FullName);
                    var party = profile.GetIndex(machineName);
                    var originalPath = party.SharedPath;
                    if (originalPath.StartsWith(newDrive.RootDirectory.FullName)) continue; // Already there.
                    originalPath = originalPath.Remove(0, originalPath.IndexOf(@"\", StringComparison.Ordinal) + 1);
                    newSharedPath = Path.Combine(newSharedPath, originalPath);
                    JoinProfile(profile.Name, party.LocalPath, newSharedPath);
                }
            }
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
            internal FileHeader LocalFileHeader;
            internal string LocalFileHash;
            internal FileHeader LocalIndexHeader;
            internal string LocalIndexHash;
            internal FileHeader PrimaryHeader;
            internal string PrimaryHash;
        }
        #endregion
    }
}


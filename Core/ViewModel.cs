﻿using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event for a list of properties.
        /// </summary>
        /// <param name="propertynames">The list of property names that have changed.</param>
        private void NotifyPropertiesChanged(params string[] propertynames)
        {
            foreach (string prop in propertynames)
            {
                NotifyPropertyChanged(prop);
            }
        }
        #endregion

        #region Fields
        public bool VerboseMode;

        /// <summary>
        /// An asynchronous file copier.
        /// </summary>
        private readonly IFileManager _fileManager;

        private readonly ISyncSetMapper _indexer;

        private List<string> _log;
        #endregion

        #region Constructors
        public ViewModel(ISyncSetMapper datacontext, string machineId, IFileManager filemanager)
        {
            _indexer = datacontext;
            _machineId = machineId;
            _fileManager = filemanager;
        }
        #endregion

        #region Methods
        public void CreateProfile(string name, ulong reserve, params string[] ignore)
        {
            if (!ProfileExists(name))
            {
                var profile = new SyncSet
                {
                    Name = name,
                    ReserveSpace = reserve,
                    IgnorePatterns = ignore.ToList(),
                    Indexes = new List<FileIndex>(),
                    MasterIndex = new FileIndex { Files = new List<FileHeader>() }
                };
                _indexer.Update(profile);
            }
            else
            {
                StatusMessage = "Profile name already exists: " + name;
            }
        }

        private bool ProfileExists(string name)
        {
            return _indexer.ReadAll().Any(x => x.Name.ToLower() == name.ToLower());
        }

        private SyncSet GetProfile(string name)
        {
            return _indexer.ReadAll().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public void JoinProfile(string profileName, string localpath, string sharedpath)
        {
            var profile = GetProfile(profileName);
            if (profile == null)
            {
                StatusMessage = String.Format("Profile does not exist: {0}", profileName);
            }
            else
            {
                JoinProfile(profile, localpath, sharedpath);
            }
        }

        public void JoinProfile(SyncSet profile, string localpath, string sharedpath)
        {
            if (!profile.ContainsParticipant(MachineId))
            {
                profile.Indexes.Add(new FileIndex
                {
                    MachineName = MachineId,
                    LocalPath = localpath,
                    SharedPath = sharedpath
                });
                _indexer.Update(profile);
            }
            else
            {
                var i = profile.GetIndex(MachineId);
                i.LocalPath = localpath;
                i.SharedPath = sharedpath;
                profile.UpdateIndex(i);
                _indexer.Update(profile);
            }
        }

        public void LeaveProfile(SyncSet profile, string machine)
        {
            if (profile.ContainsParticipant(machine))
            {
                profile.Indexes.RemoveAll(p => p.MachineName == machine);
                _indexer.Update(profile);
            }
        }

        public void LeaveProfile(string profileName, string machine)
        {
            var profile = GetProfile(profileName);
            LeaveProfile(profile, machine);
        }

        public List<string> GetProfileNames()
        {
            var names = _indexer.ReadAll().Select(x => x.Name).Distinct();
            return names.ToList();
        }

        public List<SyncSet> Profiles => _indexer.ReadAll().ToList();

        public List<string> Machines
        {
            get
            {
                return _indexer.ReadAll().SelectMany(p => p.Indexes).Select(p => p.MachineName).Distinct().ToList();
            }
        }

        public void RemoveMachine(string machine)
        {
            foreach (var p in Profiles)
            {
                LeaveProfile(p.Name, machine);
            }
        }

        public void RunSync(bool indexOnly, IStatusLogger logger, bool quickMode = false)
        {
            PushedCount = 0;
            PulledCount = 0;
            PrunedCount = 0;
            var profiles = _indexer.ReadAll();
            for (int i = 0; i < profiles.Count; i++)
            {
                SyncSet opts = profiles[i];
                if (opts.ContainsParticipant(_machineId))
                {
                    logger.Line(1);
                    logger.Log(1, "Processing profile {0}", opts.Name);

                    _log = new List<string>();
                    //PropertyChanged += SyncServicePropertyChanged;
                    VerboseMode = logger.LogLevel >= 4;
                    try
                    {
                        var begin = DateTime.Now;
                        Sync(ref opts, logger, indexOnly, quickMode);
                        profiles[i] = opts;
                        logger.Log(1, "Profile sync time taken: {0}", (DateTime.Now - begin).Verbalise());
                    }
                    catch (Exception e)
                    {
                        // This is a huge error trap. If this gets triggered, it's pretty catastrophic.
                        // TODO: Change to status message property setting.
                        logger.Log(0, "Could not sync.");
                        logger.Log(0, e.Message);
                        var x = e;
                        while (x != null)
                        {
                            logger.Log(0, DateTime.Now.ToString(CultureInfo.CurrentCulture));
                            logger.Log(0, x.Message);
                            logger.Log(0, x.StackTrace);
                            logger.Line(0);

                            x = x.InnerException;
                        }
                    }
                }
                else
                {
                    logger.Log(1, "Not participating in profile {0}", opts.Name);
                }
            }

            if (!indexOnly)
            {
                var begin = DateTime.Now;
                _indexer.UpdateAll(profiles);
                logger.Log(4, "\tSave data: {0}", (DateTime.Now - begin).Verbalise());
            }
            logger.Line(1);
            logger.Log(1, "Finished.");
            if (PushedCount + PulledCount + PrunedCount == 0)
            {
                logger.Log(1, "No actions taken");
            }
        }

        public void RemoveProfile(string profileName)
        {
            _indexer.UpdateAll(Profiles.Where(p => p.Name.ToLower() != profileName.ToLower()).ToList());
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        public void Sync(ref SyncSet syncSet, IStatusLogger logger, bool preview = false, bool quickMode = false)
        {
            // Check folders, just in case.
            var localindex = syncSet.GetIndex(MachineId) ?? new FileIndex();
            if (localindex.Files == null)
            {
                localindex.Files = new List<FileHeader>();
            }
            var sharedPath = localindex.SharedPath;
            if (!_fileManager.DirectoryExists(sharedPath))
            {
                logger.Log(0, "Shared storage not available ({0}). Aborting.", sharedPath);
                return;
            }
            if (!_fileManager.DirectoryExists(localindex.LocalPath))
            {
                logger.Log(0, $"Local storage not available ({localindex.LocalPath}). Aborting.");
                return;
            }

            if (syncSet.MasterIndex == null)
            {
                syncSet.MasterIndex = new FileIndex();
            }
            if (syncSet.MasterIndex.Files == null)
            {
                syncSet.MasterIndex.Files = new List<FileHeader>();
            }
            // 1. Compare the master index to each remote index to determine each file's state.
            var begin = DateTime.Now;
            #region Determine State
            // Pre-process the indexes to ensure the loop doesn't have to.
            var indexus = new Dictionary<string, ReplicaComparison>();
            foreach (var dex in syncSet.Indexes)
            {
                if (dex.Files == null)
                {
                    dex.Files = new List<FileHeader>();
                }
                foreach (var fyle in dex.Files)
                {
                    var path = fyle.RelativePath.ToLower();
                    if (!indexus.ContainsKey(path))
                    {
                        indexus[path] = new ReplicaComparison { Path = path, Hash = fyle.ContentsHash, Count = 0 };
                    }
                    indexus[path].AllSame = indexus[path].Hash == fyle.ContentsHash;
                    indexus[path].Count++;
                }
            }
            foreach (var mf in syncSet.MasterIndex.Files)
            {
                var path = mf.RelativePath.ToLower();
                if (mf.IsDeleted)
                {
                    mf.State = indexus.ContainsKey(path) ? FileSyncState.Expiring : FileSyncState.Destroyed;
                }
                else if (indexus.ContainsKey(path) && indexus[path].Count == syncSet.Indexes.Count && indexus[path].AllSame)
                {
                    mf.State = FileSyncState.Synchronised;
                }
                else
                {
                    mf.State = FileSyncState.Transit;
                }
            }
            #endregion
            logger.Log(4, "\tMaster index processing: {0}", (DateTime.Now - begin).Verbalise());

            // 2. Determine the necessary action for each file.
            begin = DateTime.Now;
            #region Plan actions
            var allhashes = new Dictionary<string, LocalFileHashSet>();
            // Merge the local index, the master index and the local file system.
            foreach (var f in localindex.Files)
            {
                if (allhashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allhashes[f.RelativePath.ToLower()].LocalIndexHeader = f;
                    allhashes[f.RelativePath.ToLower()].LocalIndexHash = f.ContentsHash + f.IsFolder;
                }
                else
                {
                    allhashes[f.RelativePath.ToLower()] = new LocalFileHashSet { LocalIndexHeader = f, LocalIndexHash = f.ContentsHash + f.IsFolder };
                }
            }
            foreach (var f in syncSet.MasterIndex.Files)
            {
                if (allhashes.ContainsKey(f.RelativePath.ToLower()))
                {
                    allhashes[f.RelativePath.ToLower()].MasterHeader = f;
                    allhashes[f.RelativePath.ToLower()].MasterHash = f.ContentsHash + f.IsFolder;
                }
                else
                {
                    allhashes[f.RelativePath.ToLower()] = new LocalFileHashSet { MasterHeader = f, MasterHash = f.ContentsHash + f.IsFolder };
                }
            }
            foreach (var f in _fileManager.ListLocalFiles(localindex.LocalPath))
            {
                // This action can fail if the file is in use.
                // TODO: Windows Shadow Volume integration?
                var hed = _fileManager.TryCreateFileHeader(localindex.LocalPath, f);
                if (hed == null)
                {
                    // Assume a file access exception. Skip this file by removing it and hope for better luck next run.
                    if (allhashes.ContainsKey(f.ToLower()))
                    {
                        allhashes.Remove(f.ToLower());
                    }

                    logger.Log(0, "Skipping locked file: {0}", f);
                }
                else if (allhashes.ContainsKey(f.ToLower()))
                {
                    allhashes[f.ToLower()].LocalFileHeader = hed;
                    allhashes[f.ToLower()].LocalFileHash = hed.ContentsHash + hed.IsFolder;
                }
                else
                {
                    allhashes[f.ToLower()] = new LocalFileHashSet
                    {
                        LocalFileHeader = hed,
                        LocalFileHash = hed.ContentsHash + hed.IsFolder
                    };
                }
            }

            // Gather the necessary file actions into queues first, then act and update indexes at the end.
            // One queue per type of action required: copy to local, copy to shared, delete local, delete master, rename local, no-op.
            var copyToLocal = new List<FileHeader>();
            var copyToShared = new List<FileHeader>();
            var deleteLocal = new List<FileHeader>();
            var deleteMaster = new List<FileHeader>();
            var renameLocal = new List<FileHeader>(); // Target names to be generated on demand when processing.
            var noAction = new List<FileHeader>(); // Nothing to do to these, but keep a list in case of verbose preview.
            // Copy the collection for looping, so we can modify it in case of conflicts.
            foreach (var dex in allhashes.Keys)
            {
                var f = allhashes[dex];
                if (f.MasterHeader != null)
                {
                    switch (f.MasterHeader.State)
                    {
                        case FileSyncState.Synchronised:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    // Up to date. No action required.
                                    logger.Log(4, "SYNCED: {0}", f.MasterHeader.RelativePath);
                                    noAction.Add(f.MasterHeader);
                                }
                                else
                                {
                                    // Local update. Copy to shared. Update indexes.
                                    logger.Log(3, "UPDATED [-> SHARE]: {0}", dex);
                                    copyToShared.Add(f.LocalFileHeader);
                                }
                            }
                            else
                            {
                                logger.Log(3, "DELETED [DELETE ->]: {0}", dex);
                                // Local delete. Mark deleted in master index. Remove from local index.
                                deleteMaster.Add(f.MasterHeader);
                            }
                            break;
                        case FileSyncState.Expiring:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    logger.Log(3, "EXPIRED [<- DELETE]: {0}", dex);
                                    // Remote delete. Delete file. Remove from local index.
                                    deleteLocal.Add(f.MasterHeader);
                                }
                                else
                                {
                                    // Update/delete conflict. Copy local to shared. Update local index. Mark not deleted in master index.
                                    logger.Log(2, "UN-EXPIRED [-> SHARE]: {0}", dex);
                                    copyToShared.Add(f.LocalFileHeader);
                                }
                            }
                            else
                            {
                                // Already deleted. Just make sure it's not in the local index.
                                if (f.LocalIndexHeader != null)
                                {
                                    logger.Log(4, "SIDE-CHANNEL DELETE: {0}", dex);
                                    noAction.Add(f.MasterHeader);
                                }
                                else
                                {
                                    logger.Log(4, "ALREADY DELETED: {0}", dex);
                                    noAction.Add(f.MasterHeader);
                                }
                            }
                            break;
                        case FileSyncState.Transit:
                            if (f.LocalFileHeader != null)
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                {
                                    if (f.MasterHash == f.LocalIndexHash)
                                    {
                                        logger.Log(3, "TRANSIT [-> SHARE]: {0}", dex);
                                        // Up to date locally. Copy to shared to propagate changes.
                                        copyToShared.Add(f.LocalFileHeader);
                                    }
                                    else
                                    {
                                        logger.Log(3, "TRANSIT [LOCAL <-]: {0}", dex);
                                        // Remote update. Copy to local, update local index.
                                        copyToLocal.Add(f.MasterHeader);
                                    }
                                }
                                else
                                {
                                    if (f.MasterHash == f.LocalFileHash)
                                    {
                                        // Side-channel update. Repair the local index.
                                        logger.Log(4, "SIDE-CHANNEL UPDATED: {0}", dex);
                                        noAction.Add(f.MasterHeader);
                                    }
                                    else
                                    {
                                        if (f.MasterHash == f.LocalIndexHash)
                                        {
                                            // Local update. Copy to shared. Update local and master indexes.
                                            logger.Log(3, "UPDATED [-> SHARE]: {0}", dex);
                                            copyToShared.Add(f.LocalFileHeader);
                                        }
                                        else
                                        {
                                            logger.Log(2, "CONFLICT [-> SHARE]: {0}", dex);
                                            // Update conflict. Rename the local file and add to the processing queue.
                                            renameLocal.Add(f.MasterHeader.Clone());
                                            copyToLocal.Add(f.MasterHeader);
                                        }
                                        // TODO: Detect out-of-date copies and treat them as remote updates.
                                        // Use a history of contents hashes to detect old versions.
                                    }
                                }
                            }
                            else if (f.LocalIndexHeader != null && f.LocalIndexHash == f.MasterHash)
                            {
                                // Deleted while in transit - that is, before all machines had a copy.
                                logger.Log(2, "DELETED [-> CANCEL TRANSIT]: {0}", dex);
                                deleteMaster.Add(f.MasterHeader);
                            }
                            else if (_fileManager.FileExists(sharedPath, dex) || f.MasterHeader.IsFolder)
                            {
                                logger.Log(3, "TRANSIT [LOCAL <-]: {0}", dex);
                                // Update/delete conflict or remote create. Copy to local. Update local index.
                                copyToLocal.Add(f.MasterHeader);
                            }
                            else if (f.LocalIndexHeader != null)
                            {
                                // File does not exist on local file system or in shared folder. Remove it from local index.
                                logger.Log(4, "MISSING: {0}", dex);
                                noAction.Add(f.MasterHeader);
                            }
                            else
                            {
                                // Not on local drive.
                                // Not in shared folder.
                                // Not in local index.
                                // Remote create that can't be propagated yet.
                                logger.Log(4, "PHANTOM: {0}", dex);
                                noAction.Add(f.MasterHeader);
                            }
                            break;
                    }
                }
                else if (f.LocalFileHeader != null)
                {
                    // Local create. Copy to shared. Add to indexes.
                    logger.Log(3, "NEW [-> SHARE]: {0}", dex);
                    copyToShared.Add(f.LocalFileHeader);
                }
            }
            #endregion

            logger.Log(1, "");
            logger.Log(1, "{0}{1} | Local   | Shared  |", syncSet.Name, new string(' ', Math.Max(0, 12 - syncSet.Name.Length)));
            logger.Log(1, "-------------+---------+---------+");
            logger.Log(1, "Copy To      | {0,7} | {1,7} |", copyToLocal.Count, copyToShared.Count);
            logger.Log(1, "Delete       | {0,7} | {1,7} |", deleteLocal.Count, deleteMaster.Count);
            logger.Log(1, "Conflicted   | {0,7} |         |", renameLocal.Count);
            logger.Log(1, "No Change    | {0,7} |         |", noAction.Count);
            logger.Log(1, "");

            logger.Log(4, "\tQueue generation: {0}", (DateTime.Now - begin).Verbalise());

            // 3. Process the action queue according to the mode and limitations in place.
            var errorList = new List<string>();
            if (!preview)
            {
                begin = DateTime.Now;
                #region 3.1. Rename, Copy to Local, Delete local, Delete master
                foreach (var r in renameLocal)
                {
                    var newname = _fileManager.GetConflictFileName(Path.Combine(localindex.LocalPath, r.RelativePath), MachineId, DateTime.Now);
                    var nurelpath = _fileManager.GetRelativePath(newname, localindex.LocalPath);
                    var fmr = _fileManager.MoveFile(Path.Combine(localindex.LocalPath, r.RelativePath), Path.Combine(localindex.LocalPath, nurelpath), false);
                    if (fmr == FileCommandResult.Failure)
                    {
                        errorList.Add(string.Format("File rename failed: {0}", r.RelativePath));
                    }
                    else
                    {
                        r.RelativePath = nurelpath;
                        copyToShared.Add(r);
                        PulledCount++;
                    }
                }
                WaitForCopies(); // Because if there are pending file moves, we could get a conflict overwrite here.
                foreach (var d in copyToLocal.Where(i => i.IsFolder))
                {
                    _fileManager.EnsureFolder(Path.Combine(localindex.LocalPath, d.RelativePath));
                    PulledCount++;
                }
                foreach (var c in copyToLocal.Where(i => !i.IsFolder))
                {
                    var fcr = _fileManager.CopyFile(sharedPath, c.RelativePath, localindex.LocalPath);
                    if (fcr == FileCommandResult.Failure)
                    {
                        errorList.Add(string.Format("Inbound file copy failed: {0}", c.RelativePath));
                    }
                    else
                    {
                        PulledCount++;
                    }
                }
                foreach (var d in deleteLocal.OrderByDescending(f => f.RelativePath.Length)) // Simplest way to delete deepest-first.
                {
                    _fileManager.Delete(Path.Combine(localindex.LocalPath, d.RelativePath));
                    PulledCount++; // I mean, kind of, right?
                }

                // Push.
                foreach (var m in deleteMaster)
                {
                    m.IsDeleted = true;
                    syncSet.MasterIndex.UpdateFile(m);
                    PushedCount++;
                }
                #endregion
                WaitForCopies();
                logger.Log(4, "\tInbound actions: {0}", (DateTime.Now - begin).Verbalise());
                begin = DateTime.Now;
                #region 3.2. Regenerate the local index.
                // Keep a copy of the old index in case of read failures.
                var oldex = new FileIndex { Files = new List<FileHeader>() };
                oldex.Files.AddRange(localindex.Files);
                localindex.Files = new List<FileHeader>();
                foreach (var f in _fileManager.ListLocalFiles(localindex.LocalPath))
                {
                    try
                    {
                        localindex.UpdateFile(_fileManager.CreateFileHeader(localindex.LocalPath, f));
                    }
                    catch
                    {
                        // Keep the old file.
                        localindex.UpdateFile(oldex.GetFile(f));
                    }
                }
                localindex.TimeStamp = DateTime.Now;
                syncSet.UpdateIndex(localindex);
                #endregion
                logger.Log(4, "\tLocal index regeneration: {0}", (DateTime.Now - begin).Verbalise());
                begin = DateTime.Now;
                #region 3.3 Clean up the master index and shared folder.
                // Pre-examine the indexes, for efficiency.
                indexus = new Dictionary<string, ReplicaComparison>();
                foreach (var dex in syncSet.Indexes)
                {
                    foreach (var fyle in dex.Files)
                    {
                        var path = fyle.RelativePath.ToLower();
                        if (!indexus.ContainsKey(path))
                        {
                            indexus[path] = new ReplicaComparison
                            {
                                Count = 0,
                                Path = path,
                                Hash = fyle.ContentsHash,
                                AllSame = true,
                                AllHashes = new List<string>()
                            };
                        }
                        indexus[path].Count++;
                        indexus[path].AllSame = indexus[path].Hash == fyle.ContentsHash;
                        indexus[path].AllHashes.Add(fyle.ContentsHash);
                    }
                }

                foreach (var mf in syncSet.MasterIndex.Files.ToArray())
                {
                    var key = mf.RelativePath.ToLower();
                    var shrfilehed = _fileManager.TryCreateFileHeader(sharedPath, mf.RelativePath); // Returns null if not found.
                    try
                    {
                        if (mf.IsDeleted)
                        {
                            if (!indexus.ContainsKey(key))
                            {
                                // Marked deleted and has been removed from every replica.
                                logger.Log(4, "DESTROYED: {0}", mf.RelativePath);
                                syncSet.MasterIndex.Remove(mf);
                            }
                        }
                        else if (shrfilehed != null && !syncSet.MasterIndex.MatchesFile(shrfilehed))
                        // else if (_fileManager.FileExists(SharedPath, mf.RelativePath) && !syncSet.MasterIndex.MatchesFile(_fileManager.CreateFileHeader(SharedPath, mf.RelativePath)))
                        {
                            // The shared file does not match the master index. It should be removed.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (_fileManager.FileExists(sharedPath, mf.RelativePath) && indexus.ContainsKey(key) && indexus[key].Count == syncSet.Indexes.Count && indexus[key].AllSame && indexus[key].Hash == mf.ContentsHash)
                        {
                            // Successfully transmitted to every replica. Remove from shared storage.
                            _fileManager.Delete(Path.Combine(sharedPath, mf.RelativePath));
                        }
                        else if (!indexus.ContainsKey(key))
                        {
                            // Simultaneous side-channel delete from every replica. This file is toast.
                            syncSet.MasterIndex.Remove(mf);
                        }
                        else if (indexus[key].AllHashes.All(h => h != mf.ContentsHash))
                        {
                            // Slightly more subtle. No physical matching version of this file exists any more.
                            syncSet.MasterIndex.Remove(mf);
                        }
                    }
                    catch
                    {
                        // TODO: Avoid throwing exceptions from fileManager.Delete operations. If the file doesn't exist, that's success, not failure.
                    }
                }
                // Clean up shared storage. We already looked for master/shared mismatches.
                var minx = syncSet.MasterIndex;
                var shareCleanMeta = from s in _fileManager.ListLocalFiles(sharedPath)
                                     where File.Exists(Path.Combine(sharedPath, s)) &&
                                           minx.Files.All(m => !string.Equals(m.RelativePath, s, StringComparison.CurrentCultureIgnoreCase))
                                     select s;
                // For every file now in shared storage,
                foreach (var s in shareCleanMeta)
                {
                    // Shared file is missing from master index. Get rid of it.
                    _fileManager.Delete(Path.Combine(sharedPath, s));
                    logger.Log(4, "SHARE CLEANUP: {0}", s);
                }

                // Remove empty subfolders from the Shared folder.
                // Somehow I always find myself rewriting this exact code.
                if (Directory.Exists(sharedPath))
                {
                    // Sort by length descending to get leaf nodes first.
                    var alldirs = from s in Directory.GetDirectories(sharedPath, "*", SearchOption.AllDirectories)
                                  where s != sharedPath // Don't kill the root folder.
                                  orderby s.Length descending
                                  select s;
                    foreach (string t in alldirs)
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
                            logger.Log(0, "Could not delete {0}: {1}", t, e.Message);
                        }
                    }
                }
                #endregion
                logger.Log(4, "\tMaster index cleanup: {0}", (DateTime.Now - begin).Verbalise());
                begin = DateTime.Now;
                #region 3.4. Copy to shared
                var sharedsize = _fileManager.SharedPathSize(sharedPath);
                foreach (var s in copyToShared)
                {
                    if (s != null)
                    {
                        if (sharedsize + (ulong)s.Size < syncSet.ReserveSpace)
                        {
                            logger.Log(4, "Copying {0} to {1}", s.RelativePath, sharedPath);
                            if (s.IsFolder)
                            {
                                s.IsDeleted = false; // To un-delete folders that were being removed, but have been re-created.
                                syncSet.MasterIndex.UpdateFile(s);
                                PushedCount++;
                            }
                            else
                            {
                                var result = _fileManager.CopyFile(localindex.LocalPath, s.RelativePath, sharedPath);
                                // Check for success.
                                if (result == FileCommandResult.Success || result == FileCommandResult.Async)
                                {
                                    // Assume potential success on Async.
                                    sharedsize += (ulong)s.Size;
                                    syncSet.MasterIndex.UpdateFile(s);
                                    PushedCount++;
                                }
                                else
                                {
                                    logger.Log(0, "Outbound file copy failed: {0}", s.RelativePath);
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
                        logger.Log(0, "Error: Null file name scheduled for copying to shared storage.");
                    }
                }
                WaitForCopies();
                #endregion
                logger.Log(4, "\tOutbound actions: {0}", (DateTime.Now - begin).Verbalise());
            }

            foreach (var e in errorList)
            {
                logger.Log(0, e);
            }

            //if (!preview)
            //{
            //    begin = DateTime.Now;
            //    _indexer.Update(syncSet);
            //    logger.Log(4, "\tSave data: {0}", (DateTime.Now - begin).Verbalise());
            //}
        }

        /// <summary>
        /// Compares indexes.
        /// </summary>
        public void ShowIndexComparison()
        {
            // QAD way: Preserve consumer/give flags, call Sync.
            // TODO: Count files in full sync, only here, and only elsewhere.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Spins until all async file copies are complete.
        /// </summary>
        private void WaitForCopies()
        {
            // Wait for file copies to finish.
            int lastcount = 0;
            DateTime startedWaiting = DateTime.Now;
            var firstCount = _fileManager.Count;
            while (_fileManager.Count > 0)
            {
                if (_fileManager.Count != lastcount)
                {
                    // Estimate time left via copies per second. Assumes even distribution of file sizes in queue.
                    var secondswaiting = (DateTime.Now - startedWaiting).TotalSeconds;
                    if (secondswaiting > 0)
                    {
                        var cps = (firstCount - _fileManager.Count) / secondswaiting;
                        if (cps > 0)
                        {
                            var timeleft = new TimeSpan(0, 0, _fileManager.Count / (int)cps);
                            StatusMessage = string.Format("\t\tWaiting on {0} {1}... (~{2} remaining)",
                                _fileManager.Count, (_fileManager.Count == 1 ? "copy" : "copies"), timeleft);
                        }
                    }

                    lastcount = _fileManager.Count;
                }
                Thread.Sleep(1000);
            }
        }

        public void Save()
        {
            // TODO: Save the profile data.
            // Note: None is currently kept in memory. This might be part of why the program is so slow sometimes.
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

        private string _statusMessage;
        [Obsolete("Use IStatusLogger instead.")]
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                NotifyPropertyChanged();
            }
        }

        private int _pulledCount;
        public int PulledCount
        {
            get => _pulledCount;
            set
            {
                _pulledCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _pushedCount;
        public int PushedCount
        {
            get => _pushedCount;
            set
            {
                _pushedCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _prunedCount;
        public int PrunedCount
        {
            get => _prunedCount;
            set
            {
                _prunedCount = value;
                NotifyPropertyChanged();
            }
        }
        public List<Exception> Errors => _fileManager.Errors;

        public List<string> Log
        {
            get => _log;
            set
            {
                _log = value;
                NotifyPropertiesChanged("Log");
            }
        }

        private class ReplicaComparison
        {
            internal bool AllSame;
            internal string Path;
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
            internal FileHeader MasterHeader;
            internal string MasterHash;
        }
        #endregion
    }
}


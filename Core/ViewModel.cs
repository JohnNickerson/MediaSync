﻿using AssimilationSoftware.MediaSync.Core.Commands;
using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        private int NumPeers;

        public bool VerboseMode;

        /// <summary>
        /// An asynchronous file copier.
        /// </summary>
        private IFileManager _fileManager;

        private ISyncSetMapper _indexer;

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
                var profile = new SyncSet();
                profile.Name = name;
                profile.ReserveSpace = reserve;
                profile.IgnorePatterns = ignore.ToList();
                profile.Indexes = new List<FileIndex>();
                profile.MasterIndex = new FileIndex
                {
                    Files = new List<FileHeader>()
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

        public void JoinProfile(string profileName, string localpath, string sharedpath, bool contributor, bool consumer)
        {
            var profile = GetProfile(profileName);
            if (profile == null)
            {
                StatusMessage = String.Format("Profile does not exist: {0}", profile);
            }
            else
            {
                JoinProfile(profile, localpath, sharedpath, contributor, consumer);
            }
        }

        public void JoinProfile(SyncSet profile, string localpath, string sharedpath, bool contributor, bool consumer)
        {
            if (!profile.ContainsParticipant(this.MachineId))
            {
                profile.Indexes.Add(new FileIndex
                {
                    MachineName = this.MachineId,
                    LocalPath = localpath,
                    SharedPath = sharedpath,
                    IsPush = contributor,
                    IsPull = consumer
                });
                _indexer.Update(profile);
            }
        }

        public void LeaveProfile(SyncSet profile)
        {
            if (profile.ContainsParticipant(this.MachineId))
            {
                profile.Indexes.RemoveAll(p => p.MachineName == MachineId);
                _indexer.Update(profile);
            }
        }

        public void LeaveProfile(string profileName)
        {
            var profile = GetProfile(profileName);
            LeaveProfile(profile);
        }

        public List<string> GetProfileNames()
        {
            var names = _indexer.ReadAll().Select(x => x.Name).Distinct();
            return names.ToList();
        }

        public List<SyncSet> Profiles
        {
            get
            {
                return _indexer.ReadAll().ToList();
            }
        }

        public List<string> Machines
        {
            get
            {
                return _indexer.ReadAll().SelectMany(p => p.Indexes).Select(p => p.MachineName).Distinct().ToList();
            }
        }

        public void RemoveMachine(string machine)
        {
            var savemachine = MachineId;
            MachineId = machine;

            foreach (var p in Profiles)
            {
                LeaveProfile(p.Name);
            }

            MachineId = savemachine;
        }

        public void RunSync(bool IndexOnly, IStatusLogger logger)
        {
            PushedCount = 0;
            PulledCount = 0;
            PrunedCount = 0;
            foreach (SyncSet opts in this.Profiles)
            {
                if (opts.ContainsParticipant(_machineId))
                {
                    logger.Line(1);
                    logger.Log(1, "Processing profile {0}", opts.Name);

                    NumPeers = 0;
                    _log = new List<string>();
                    //PropertyChanged += SyncServicePropertyChanged;
                    VerboseMode = logger.LogLevel >= 4;
                    try
                    {
                        if (IndexOnly)
                        {
                            ShowIndexComparison();
                        }
                        else
                        {
                            var begin = DateTime.Now;
                            Sync(opts, logger);
                            logger.Log(1, "Profile sync time taken: {0}", (DateTime.Now - begin).Verbalise());
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: Change to status message property setting.
                        logger.Log(0, "Could not sync.");
                        logger.Log(0, e.Message);
                        var x = e;
                        while (x != null)
                        {
                            logger.Log(0, DateTime.Now.ToString());
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

            logger.Line(1);
            logger.Log(1, "Finished.");
            if (PushedCount + PulledCount + PrunedCount == 0)
            {
                logger.Log(1, "No actions taken");
            }
        }

        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles(SyncSet syncSet)
        {
            var _localSettings = syncSet.GetIndex(MachineId);
            var index = _fileManager.CreateIndex(_localSettings.LocalPath, "*.*");
            index.IsPull = _localSettings.IsPull;
            index.IsPush = _localSettings.IsPush;
            index.MachineName = _localSettings.MachineName;
            index.SharedPath = _localSettings.SharedPath;
            syncSet.Indexes.Add(index);
            _indexer.Update(syncSet);

            // Compare this index with others.
            NumPeers = syncSet.Indexes.Count;
        }

        /// <summary>
        /// Removes empty folders from the shared path.
        /// </summary>
        internal void ClearEmptyFolders(string SharedPath)
        {
            string inbox = SharedPath;
            // Sort by descending length to get leaf nodes first.
            foreach (string dir in from s in _fileManager.GetDirectories(inbox)
                                   orderby s.Length descending
                                   select s)
            {
                if (_fileManager.ListLocalFiles(dir).Length == 0 && _fileManager.GetDirectories(dir).Length == 0)
                {
                    // Empty folder. Remove it.
                    try
                    {
                        if (VerboseMode)
                        {
                            Report(new DeleteFile(dir));
                        }
                        _fileManager.Delete(dir);
                    }
                    catch
                    {
                        ReportMessage("Could not delete apparently empty folder: {0}", dir);
                    }
                }
            }
        }

        [Obsolete("Use IStatusLogger instead.")]
        private void ReportMessage(string format, params object[] args)
        {
            _log.Add(string.Format(format, args));
            NotifyPropertiesChanged("Log");
        }

        private void Report(FileCommand op)
        {
            if (op is CopyCommand)
            {
                _log.Add(string.Format("Copying:\t{0}{1}", ((CopyCommand)op).Source, Environment.NewLine));
            }
            else if (op is MoveCommand)
            {
                _log.Add(string.Format("Moving:\t{0}{1}", ((MoveCommand)op).Source, Environment.NewLine));
            }
            else if (op is DeleteFile)
            {
                _log.Add(string.Format("Deleting:\t{0}{1}", ((DeleteFile)op).Path, Environment.NewLine));
            }
            NotifyPropertiesChanged("Log");
        }

        /// <summary>
        /// Pushes files to shared storage where they are found wanting in other peers.
        /// </summary>
        internal int PushFiles(SyncSet syncSet)
        {
            // No point trying to push files when they'll all be ignored.
            if (NumPeers == 1)
            {
                ReportMessage("No peers, no point.");
                return 0;
            }

            var localIndex = syncSet.GetIndex(MachineId);
            var _sizecache = _fileManager.SharedPathSize(localIndex.SharedPath);
            int pushcount = 0;
            // TODO: Prioritise the least-common files.
            // TODO: Select files that have been updated or created locally.
            //var sortedfilelist = from f in FileCounts.Keys orderby FileCounts[f] select f;
            // For every file in the index
            foreach (string fullFileName in _fileManager.ListLocalFiles(localIndex.LocalPath, "*.*"))
            {
                // If the size allocation has been exceeded, stop.
                if (_sizecache > syncSet.ReserveSpace)
                {
                    ReportMessage("Shared space exhausted ({0}). Stopping for now.", VerbaliseBytes(_sizecache));
                    break;
                }
                string filename = _fileManager.GetRelativePath(fullFileName, localIndex.LocalPath);
                var indexFile = localIndex.GetFile(filename);
                string targetfile = Path.Combine(localIndex.SharedPath, filename);

                // If the file mismatches the local index and space is available...
                if (!_fileManager.FilesMatch(fullFileName, indexFile))
                {
                    Report(new CopyCommand(fullFileName, targetfile));
                    _fileManager.CopyFile(fullFileName, targetfile);
                    // Update size cache.
                    _sizecache += (ulong)new FileInfo(fullFileName).Length;
                    // Update the local and master indexes.
                    var updatedHeader = _fileManager.CreateFileHeader(localIndex.LocalPath, filename);
                    localIndex.UpdateFile(updatedHeader);
                    syncSet.MasterIndex.UpdateFile(updatedHeader.Clone());
                    pushcount++;
                    StatusMessage = string.Format("\t\tConstructing copy queue: {1} {0}.", _fileManager.Count, (_fileManager.Count == 1 ? "item" : "items"));
                }
                else
                {
                    //ReportMessage("Excluding file {0} because it is already everywhere.", file);
                }
            }

            foreach (var delfile in localIndex.Files.Where(f => !_fileManager.FileExists(localIndex.LocalPath, f.RelativePath)).ToList())
            {
                var masterindexfile = syncSet.MasterIndex.GetFile(delfile.RelativePath);
                if (masterindexfile != null && _fileManager.FilesMatch(delfile, masterindexfile))
                {
                    Report(new DeleteFile(delfile.RelativePath));
                    masterindexfile.IsDeleted = true;
                    localIndex.Remove(delfile);
                }
            }

            WaitForCopies();
            _fileManager.SetNormalAttributes(localIndex.SharedPath);
            return pushcount;
        }

        /// <summary>
        /// Turns a number of bytes into a more human-friendly reading.
        /// </summary>
        /// <param name="bytes">The number of bytes to represent.</param>
        /// <returns>The number of bytes represented as B, KB, MB, GB or TB, whatever is most appropriate.</returns>
        private string VerbaliseBytes(ulong bytes)
        {
            if (bytes < 1000)
            {
                return string.Format("{0}B", bytes);
            }
            else if (bytes < Math.Pow(10, 6))
            {
                return string.Format("{0:0}KB", bytes / Math.Pow(10, 3));
            }
            else if (bytes < Math.Pow(10, 9))
            {
                return string.Format("{0:0}MB", bytes / Math.Pow(10, 6));
            }
            else if (bytes < Math.Pow(10, 12))
            {
                return string.Format("{0:0}GB", bytes / Math.Pow(10, 9));
            }
            else
            {
                return string.Format("{0:0}TB", bytes / Math.Pow(10, 12));
            }
        }

        /// <summary>
        /// Copies files from shared storage if they are not present locally.
        /// </summary>
        internal int PullFiles(SyncSet repo)
        {
            int pullcount = 0;
            var localIndex = repo.GetIndex(MachineId);
            foreach (var masterfile in repo.MasterIndex.Files)
            {
                var localIndexFile = localIndex.GetFile(masterfile.RelativePath);
                var localFile = Path.Combine(localIndex.LocalPath, masterfile.RelativePath);
                if (masterfile.IsDeleted)
                {
                    // File has been deleted somewhere else. If it's still the same here, delete it here.
                    // Note: files deleted elsewhere and changed here will be handled later.
                    if (_fileManager.FilesMatch(localFile, localIndexFile))
                    {
                        _fileManager.Delete(localFile);
                        localIndex.Remove(localIndexFile);
                    }
                }
                else
                {
                    // If the local file and both indexes are all mismatches, that's a conflict.
                    // Rename the local file and add the change to the index.
                    if (localIndexFile != null && !_fileManager.FilesMatch(masterfile, localIndexFile) && _fileManager.FileExists(localFile) && !_fileManager.FilesMatch(localFile, localIndexFile))
                    {
                        var newname = _fileManager.GetConflictFileName(localFile, MachineId, DateTime.Now);
                        // TODO: _fileManager.MoveFileNoOverwrite(...);
                        // TODO: Return the new file name from MoveFile().
                        _fileManager.MoveFile(localFile, newname, false);
                        localIndex.UpdateFile(_fileManager.CreateFileHeader(localIndex.LocalPath, _fileManager.GetRelativePath(localIndex.LocalPath, newname)));
                    }

                    // If the master index mismatches the local index and the local index matches the local file or the local file is missing, copy the file locally.
                    if (localIndexFile != null && !_fileManager.FilesMatch(masterfile, localIndexFile) && (!_fileManager.FileExists(localFile) || _fileManager.FilesMatch(localFile, localIndexFile)))
                    {
                        var sharedFile = Path.Combine(localIndex.SharedPath, masterfile.RelativePath);
                        if (_fileManager.FileExists(sharedFile))
                        {
                            _fileManager.CopyFile(sharedFile, localFile);
                            pullcount++;
                            localIndex.UpdateFile(masterfile.Clone());
                        }
                    }

                    // File is just missing locally.
                    if (localIndexFile == null && !_fileManager.FileExists(localFile))
                    {
                        var sharedFile = Path.Combine(localIndex.SharedPath, masterfile.RelativePath);
                        if (_fileManager.FileExists(sharedFile))
                        {
                            _fileManager.CopyFile(sharedFile, localFile);
                            pullcount++;
                            localIndex.UpdateFile(masterfile.Clone());
                        }
                    }
                }
            }
            repo.UpdateIndex(localIndex);
            _indexer.Update(repo);
            WaitForCopies();
            return pullcount;
        }

        /// <summary>
        /// Checks for files in shared storage that are now present everywhere and removes them from shared
        /// storage to make more room.
        /// </summary>
        internal int PruneFiles(SyncSet repo)
        {
            int prunecount = 0;
            var localIndex = repo.GetIndex(MachineId);
            foreach (string sharedfile in _fileManager.ListLocalFiles(localIndex.SharedPath, "*.*"))
            {
                var relativeSharedPath = _fileManager.GetRelativePath(sharedfile, localIndex.SharedPath);
                var indexfile = localIndex.GetFile(relativeSharedPath);
                var masterFile = repo.MasterIndex.GetFile(relativeSharedPath);
                bool matchallindex = true;
                foreach (var remoteindex in repo.Indexes)
                {
                    matchallindex &= _fileManager.FilesMatch(sharedfile, remoteindex.GetFile(relativeSharedPath));
                }
                if (matchallindex && !_fileManager.FilesMatch(sharedfile, masterFile))
                {
                    _fileManager.Delete(sharedfile);
                    prunecount++;
                }
            }
            ClearEmptyFolders(localIndex.SharedPath);
            return prunecount;
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        public void Sync(SyncSet syncSet, IStatusLogger logger)
        {
            // Check folders, just in case.
            var localindex = syncSet.GetIndex(MachineId) ?? new FileIndex();
            if (localindex.Files == null)
            {
                localindex.Files = new List<FileHeader>();
            }
            var SharedPath = localindex.SharedPath;
            if (!_fileManager.DirectoryExists(SharedPath))
            {
                logger.Log(0, "Shared storage not available ({0}). Aborting.", SharedPath);
                return;
            }

            if (syncSet.MasterIndex == null)
            {
                syncSet.MasterIndex = new Model.FileIndex();
            }
            if (syncSet.MasterIndex.Files == null)
            {
                syncSet.MasterIndex.Files = new List<Model.FileHeader>();
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
                    dex.Files = new List<Model.FileHeader>();
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
                    if (indexus.ContainsKey(path))
                    {
                        mf.State = FileSyncState.Expiring;
                    }
                    else
                    {
                        mf.State = FileSyncState.Destroyed;
                    }
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
            var allfiles = new Dictionary<string, FileHeader>();
            var allhashes = new Dictionary<string, LocalFileHashSet>();
            // Merge the local index, the master index and the local file system.
            foreach (var f in _fileManager.ListLocalFiles(localindex.LocalPath))
            {
                allfiles[f] = _fileManager.CreateFileHeader(localindex.LocalPath, f);
                allhashes[f] = new Core.ViewModel.LocalFileHashSet { LocalFileHeader = allfiles[f], LocalFileHash = allfiles[f].ContentsHash };
            }
            foreach (var f in localindex.Files)
            {
                allfiles[f.RelativePath] = f;
                if (allhashes.ContainsKey(f.RelativePath))
                {
                    allhashes[f.RelativePath].LocalIndexHeader = f;
                    allhashes[f.RelativePath].LocalIndexHash = f.ContentsHash;
                }
                else
                {
                    allhashes[f.RelativePath] = new LocalFileHashSet { LocalIndexHeader = f, LocalIndexHash = f.ContentsHash };
                }
            }
            foreach (var f in syncSet.MasterIndex.Files)
            {
                allfiles[f.RelativePath] = f;
                if (allhashes.ContainsKey(f.RelativePath))
                {
                    allhashes[f.RelativePath].MasterHeader = f;
                    allhashes[f.RelativePath].MasterHash = f.ContentsHash;
                }
                else
                {
                    allhashes[f.RelativePath] = new LocalFileHashSet { MasterHeader = f, MasterHash = f.ContentsHash };
                }
            }

            // Gather the necessary file actions into queues first, then act and update indexes at the end.
            // One queue per type of action required: copy to local, copy to shared, delete local, delete master, rename local, no-op.
            var CopyToLocal = new List<FileHeader>();
            var CopyToShared = new List<FileHeader>();
            var DeleteLocal = new List<FileHeader>();
            var DeleteMaster = new List<FileHeader>();
            var RenameLocal = new List<FileHeader>(); // Target names to be generated on demand when processing.
            var NoAction = new List<FileHeader>(); // Nothing to do to these, but keep a list in case of verbose preview.
            // Copy the collection for looping, so we can modify it in case of conflicts.
            var allFileList = allhashes.Keys.ToList();
            foreach (var dex in allhashes.Keys)
            // [OPTIMISED] for (int dex = 0; dex < allFileList.Count; dex++)
            {
                var f = allhashes[dex];
                // [OPTIMISED] var f = allFileList[dex];
                if (f.MasterHeader != null)
                // [OPTIMISED] if (syncSet.MasterIndex.Exists(f.RelativePath))
                {
                    //var localheader = f;
                    //if (File.Exists(Path.Combine(localindex.LocalPath, dex)))
                    //{
                    //    localheader = _fileManager.CreateFileHeader(localindex.LocalPath, dex);
                    //}
                    switch (f.MasterHeader.State)
                    {
                        case FileSyncState.Synchronised:
                            if (f.LocalFileHeader != null)
                            // [OPTIMISED] if (_fileManager.FileExists(localindex.LocalPath, f.RelativePath))
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                // [OPTIMISED] if (localindex.MatchesFile(localheader))
                                {
                                    // Up to date. No action required.
                                    logger.Log(4, "SYNCED: {0}", f.MasterHeader.RelativePath);
                                    NoAction.Add(f.MasterHeader);
                                }
                                else
                                {
                                    // Local update. Copy to shared. Update indexes.
                                    logger.Log(3, "UPDATED [-> SHARE]: {0}", dex);
                                    CopyToShared.Add(f.MasterHeader);
                                }
                            }
                            else
                            {
                                logger.Log(3, "DELETED [DELETE ->]: {0}", dex);
                                // Local delete. Mark deleted in master index. Remove from local index.
                                DeleteMaster.Add(f.MasterHeader);
                            }
                            break;
                        case FileSyncState.Expiring:
                            if (f.LocalFileHeader != null)
                            // [OPTIMISED] if (_fileManager.FileExists(localindex.LocalPath, f.RelativePath))
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                // [OPTIMISED] if (localindex.MatchesFile(localheader))
                                {
                                    logger.Log(3, "EXPIRED [<- DELETE]: {0}", dex);
                                    // Remote delete. Delete file. Remove from local index.
                                    DeleteLocal.Add(f.MasterHeader);
                                }
                                else
                                {
                                    // Update/delete conflict. Copy local to shared. Update local index. Mark not deleted in master index.
                                    logger.Log(2, "UN-EXPIRED [-> SHARE]: {0}", dex);
                                    CopyToShared.Add(f.MasterHeader);
                                }
                            }
                            else
                            {
                                // Already deleted. Just make sure it's not in the local index.
                                if (f.LocalIndexHeader != null)
                                // [OPTIMISED] if (localindex.Exists(f.RelativePath))
                                {
                                    logger.Log(2, "SIDE-CHANNEL DELETE: {0}", dex);
                                    NoAction.Add(f.MasterHeader);
                                }
                                else
                                {
                                    logger.Log(2, "ALREADY DELETED: {0}", dex);
                                    NoAction.Add(f.MasterHeader);
                                }
                            }
                            break;
                        case FileSyncState.Transit:
                            if (f.LocalFileHeader != null)
                            // [OPTIMISED] if (_fileManager.FileExists(localindex.LocalPath, f.RelativePath))
                            {
                                if (f.LocalIndexHash == f.LocalFileHash)
                                // [OPTIMISED] if (localindex.MatchesFile(localheader))
                                {
                                    if (f.MasterHash == f.LocalIndexHash)
                                    // [OPTIMISED] if (syncSet.MasterIndex.MatchesFile(localindex.GetFile(f.RelativePath)))
                                    {
                                        logger.Log(3, "TRANSIT [-> SHARE]: {0}", dex);
                                        // Up to date locally. Copy to shared to propagate changes.
                                        CopyToShared.Add(f.MasterHeader);
                                    }
                                    else
                                    {
                                        logger.Log(3, "TRANSIT [LOCAL <-]: {0}", dex);
                                        // Remote update. Copy to local, update local index.
                                        CopyToLocal.Add(f.MasterHeader);
                                    }
                                }
                                else
                                {
                                    if (f.MasterHash == f.LocalFileHash)
                                    // [OPTIMISED] if (syncSet.MasterIndex.MatchesFile(localheader))
                                    {
                                        // Side-channel update. Repair the local index.
                                        logger.Log(3, "SIDE-CHANNEL UPDATED: {0}", dex);
                                        NoAction.Add(f.MasterHeader);
                                    }
                                    else
                                    {
                                        if (f.MasterHash == f.LocalIndexHash)
                                        // [OPTIMISED] if (syncSet.MasterIndex.MatchesFile(localindex.GetFile(f.RelativePath)))
                                        {
                                            // Local update. Copy to shared. Update local and master indexes.
                                            logger.Log(3, "UPDATED [-> SHARE]: {0}", dex);
                                            CopyToShared.Add(f.MasterHeader);
                                        }
                                        else
                                        {
                                            logger.Log(2, "CONFLICT [-> SHARE]: {0}", dex);
                                            // Update conflict. Rename the local file and add to the processing queue.
                                            RenameLocal.Add(f.MasterHeader);
                                            CopyToLocal.Add(f.MasterHeader);
                                        }
                                        // TODO: Detect out-of-date copies and treat them as remote updates.
                                        // Use a history of contents hashes to detect old versions.
                                    }
                                }
                            }
                            else if (_fileManager.FileExists(SharedPath, dex))
                            {
                                logger.Log(3, "TRANSIT [LOCAL <-]: {0}", dex);
                                // Update/delete conflict or remote create. Copy to local. Update local index.
                                CopyToLocal.Add(f.MasterHeader);
                            }
                            else if (f.LocalIndexHeader != null)
                            // [OPTIMISED] else if (localindex.Exists(f.RelativePath))
                            {
                                // File does not exist on local file system or in shared folder. Remove it from local index.
                                logger.Log(3, "MISSING: {0}", dex);
                                NoAction.Add(f.MasterHeader);
                            }
                            else
                            {
                                // Not on local drive.
                                // Not in shared folder.
                                // Not in local index.
                                // Remote create that can't be propagated yet.
                                logger.Log(3, "PHANTOM: {0}", dex);
                                NoAction.Add(f.MasterHeader);
                            }
                            break;
                    }
                }
                else
                {
                    // Local create. Copy to shared. Add to indexes.
                    logger.Log(3, "NEW [-> SHARE]: {0}", dex);
                    CopyToShared.Add(f.LocalFileHeader);
                }
            }
            #endregion
            logger.Log(CopyToLocal.Count == 0 ? 3 : 1, "{0} file(s) to copy to this machine.", CopyToLocal.Count);
            logger.Log(CopyToShared.Count == 0 ? 3 : 1, "{0} file(s) to copy to shared storage.", CopyToShared.Count);
            logger.Log(RenameLocal.Count == 0 ? 3 : 1, "{0} file(s) to rename to avoid conflicts.", RenameLocal.Count);
            logger.Log(DeleteLocal.Count == 0 ? 3 : 1, "{0} file(s) to delete locally.", DeleteLocal.Count);
            logger.Log(DeleteMaster.Count == 0 ? 3 : 1, "{0} file(s) to delete remotely.", DeleteMaster.Count);
            logger.Log(NoAction.Count == 0 ? 3 : 1, "{0} file(s) already in sync.", NoAction.Count);
            logger.Log(4, "\tQueue generation: {0}", (DateTime.Now - begin).Verbalise());

            // 3. Process the action queue according to the mode and limitations in place.
            var ErrorList = new List<string>();
            begin = DateTime.Now;
            #region 3.1. Rename, Copy to Local, Delete local, Delete master
            if (localindex.IsPull)
            {
                foreach (var r in RenameLocal)
                {
                    var newname = _fileManager.GetConflictFileName(Path.Combine(localindex.LocalPath, r.RelativePath), MachineId, DateTime.Now);
                    var nurelpath = _fileManager.GetRelativePath(newname, localindex.LocalPath);
                    _fileManager.MoveFile(Path.Combine(localindex.LocalPath, r.RelativePath), Path.Combine(localindex.LocalPath, nurelpath), false);
                    r.RelativePath = nurelpath;
                    CopyToShared.Add(r);
                    PulledCount++;
                }
                WaitForCopies(); // Because if there are pending file moves, we could get a conflict overwrite here.
                foreach (var c in CopyToLocal)
                {
                    var fcr = _fileManager.CopyFile(SharedPath, c.RelativePath, localindex.LocalPath);
                    if (fcr == FileCommandResult.Failure)
                    {
                        ErrorList.Add(string.Format("File copy failed: {0}", c.RelativePath));
                    }
                    else
                    {
                        PulledCount++;
                    }
                }
                foreach (var d in DeleteLocal)
                {
                    _fileManager.Delete(Path.Combine(localindex.LocalPath, d.RelativePath));
                    PulledCount++; // I mean, kind of, right?
                }
            }
            if (localindex.IsPush)
            {
                foreach (var m in DeleteMaster)
                {
                    m.IsDeleted = true;
                    syncSet.MasterIndex.UpdateFile(m);
                    PushedCount++;
                }
            }
            #endregion
            WaitForCopies();
            logger.Log(4, "\tInbound actions: {0}", (DateTime.Now - begin).Verbalise());
            begin = DateTime.Now;
            #region 3.2. Regenerate the local index.
            localindex.Files = new List<FileHeader>();
            foreach (var f in _fileManager.ListLocalFiles(localindex.LocalPath))
            {
                localindex.UpdateFile(_fileManager.CreateFileHeader(localindex.LocalPath, f));
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
                if (mf.IsDeleted)
                {
                    if (!indexus.ContainsKey(key))
                    //[OPTIMISED] if (!syncSet.Indexes.Any(i => i.Exists(mf.RelativePath)))
                    {
                        // Marked deleted and has been removed from every replica.
                        logger.Log(4, "DESTROYED: {0}", mf.RelativePath);
                        syncSet.MasterIndex.Remove(mf);
                    }
                }
                else if (_fileManager.FileExists(SharedPath, mf.RelativePath) && !syncSet.MasterIndex.MatchesFile(_fileManager.CreateFileHeader(SharedPath, mf.RelativePath)))
                {
                    // The shared file does not match the master index. It should be removed.
                    _fileManager.Delete(Path.Combine(SharedPath, mf.RelativePath));
                    PrunedCount++;
                }
                else if (indexus.ContainsKey(key) && indexus[key].Count == syncSet.Indexes.Count && indexus[key].AllSame && indexus[key].Hash == mf.ContentsHash)
                //[OPTIMISED] else if (syncSet.Indexes.All(i => i.Exists(mf.RelativePath) && i.GetFile(mf.RelativePath).ContentsHash == mf.ContentsHash))
                {
                    // Successfully transmitted to every replica. Remove from shared storage.
                    _fileManager.Delete(Path.Combine(SharedPath, mf.RelativePath));
                    PrunedCount++;
                }
                else if (!indexus.ContainsKey(key))
                //[OPTIMISED] else if (!syncSet.Indexes.Any(i => i.Exists(mf.RelativePath)))
                {
                    // Simultaneous side-channel delete from every replica. This file is toast.
                    syncSet.MasterIndex.Remove(mf);
                }
                else if (!indexus[key].AllHashes.Any(h => h == mf.ContentsHash))
                //[OPTIMISED] else if (!syncSet.Indexes.Any(i => i.MatchesFile(mf)))
                {
                    // Slightly more subtle. No physical matching version of this file exists any more.
                    syncSet.MasterIndex.Remove(mf);
                }
            }
            // Remove empty subfolders from the Shared folder.
            // Somehow I always find myself rewriting this exact code.
            if (Directory.Exists(SharedPath))
            {
                // Sort by length descending to get leaf nodes first.
                var alldirs = from s in Directory.GetDirectories(SharedPath, "*", SearchOption.AllDirectories)
                              where s != SharedPath // Don't kill the root folder.
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
            var sharedsize = _fileManager.SharedPathSize(SharedPath);
            foreach (var s in CopyToShared)
            {
                if (sharedsize + (ulong)s.Size < syncSet.ReserveSpace)
                {
                    var result = _fileManager.CopyFile(localindex.LocalPath, s.RelativePath, SharedPath);
                    // Check for success.
                    if (result == FileCommandResult.Success || result == FileCommandResult.Async)
                    {
                        // Assume potential success on Async.
                        sharedsize += (ulong)s.Size;
                        syncSet.MasterIndex.UpdateFile(_fileManager.CreateFileHeader(SharedPath, s.RelativePath));
                        PushedCount++;
                    }
                }
                else if ((ulong)s.Size > syncSet.ReserveSpace)
                {
                    // TODO: split the file and copy in pieces
                }
                // Else there is no room yet. Better luck next time.
            }
            WaitForCopies();
            #endregion
            logger.Log(4, "\tOutbound actions: {0}", (DateTime.Now - begin).Verbalise());

            foreach (var e in ErrorList)
            {
                logger.Log(0, e);
            }

            begin = DateTime.Now;
            _indexer.Update(syncSet);
            logger.Log(4, "\tSave data: {0}", (DateTime.Now - begin).Verbalise());
            return;
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
            bool estimate_time = true;
            DateTime started_waiting = DateTime.Now;
            var first_count = _fileManager.Count;
            while (_fileManager.Count > 0)
            {
                if (_fileManager.Count != lastcount)
                {
                    if (estimate_time)
                    {
                        // Estimate time left via copies per second. Assumes even distribution of file sizes in queue.
                        var secondswaiting = (DateTime.Now - started_waiting).TotalSeconds;
                        if (secondswaiting > 0)
                        {
                            var cps = (first_count - _fileManager.Count) / secondswaiting;
                            if (cps > 0)
                            {
                                var timeleft = new TimeSpan(0, 0, _fileManager.Count / (int)cps);
                                StatusMessage = string.Format("\t\tWaiting on {0} {1}... (~{2} remaining)", _fileManager.Count, (_fileManager.Count == 1 ? "copy" : "copies"), timeleft);
                            }
                        }
                    }
                    else
                    {
                        StatusMessage = string.Format("\t\tWaiting on {0} {1}...", _fileManager.Count, (_fileManager.Count == 1 ? "copy" : "copies"));
                    }
                    lastcount = _fileManager.Count;
                }
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Properties
        private string _machineId;
        public string MachineId
        {
            get
            {
                return _machineId;
            }
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
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
                NotifyPropertyChanged();
            }
        }

        private int _pulledCount;
        public int PulledCount
        {
            get
            {
                return _pulledCount;
            }
            set
            {
                _pulledCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _pushedCount;
        public int PushedCount
        {
            get
            {
                return _pushedCount;
            }
            set
            {
                _pushedCount = value;
                NotifyPropertyChanged();
            }
        }

        private int _prunedCount;
        public int PrunedCount
        {
            get
            {
                return _prunedCount;
            }
            set
            {
                _prunedCount = value;
                NotifyPropertyChanged();
            }
        }
        public List<Exception> Errors
        {
            get
            {
                return _fileManager.Errors;
            }
        }
        public List<string> Log
        {
            get
            {
                return _log;
            }
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


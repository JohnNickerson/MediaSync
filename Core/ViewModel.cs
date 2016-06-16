using AssimilationSoftware.MediaSync.Core.Commands;
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
            JoinProfile(profile, localpath, sharedpath, contributor, consumer);
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

        public void RunSync(bool Verbose, bool IndexOnly, PropertyChangedEventHandler SyncServicePropertyChanged)
        {
            PushedCount = 0;
            PulledCount = 0;
            PrunedCount = 0;
            foreach (SyncSet opts in this.Profiles)
            {
                if (opts.ContainsParticipant(_machineId))
                {
                    StatusMessage = string.Format("Processing profile {0}", opts.Name);

                    NumPeers = 0;
                    _log = new List<string>();
                    PropertyChanged += SyncServicePropertyChanged;
                    VerboseMode = Verbose;
                    try
                    {
                        if (IndexOnly)
                        {
                            ShowIndexComparison();
                        }
                        else
                        {
                            Sync(opts);
                        }
                    }
                    catch (Exception e)
                    {
                        // TODO: Change to status message property setting.
                        System.Console.WriteLine("Could not sync.");
                        System.Console.WriteLine(e.Message);
                        var x = e;
                        while (x != null)
                        {
                            System.Console.WriteLine(DateTime.Now);
                            System.Console.WriteLine(x.Message);
                            System.Console.WriteLine(x.StackTrace);
                            System.Console.WriteLine("");

                            x = x.InnerException;
                        }
                    }
                }
                else
                {
                    StatusMessage = string.Format("Not participating in profile {0}", opts.Name);
                }
            }

            StatusMessage = "Finished.";
            if (PushedCount + PulledCount + PrunedCount == 0)
            {
                StatusMessage = "\tNo actions taken";
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

            foreach (var delfile in localIndex.Files.Where(f => !_fileManager.FileExists(Path.Combine(localIndex.LocalPath, f.RelativePath))).ToList())
            {
                var masterindexfile = syncSet.MasterIndex.GetFile(delfile.RelativePath);
                if (masterindexfile != null && _fileManager.FilesMatch(delfile, masterindexfile))
                {
                    Report(new DeleteFile(delfile.RelativePath));
                    masterindexfile.IsDeleted = true;
                    localIndex.Files.Remove(delfile);
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
        public void Sync(SyncSet syncSet)
        {
            // Check folders, just in case.
            var localindex = syncSet.GetIndex(MachineId);
            var SharedPath = localindex.SharedPath;
            if (!_fileManager.DirectoryExists(SharedPath))
            {
                ReportMessage("Shared storage not available ({0}). Aborting.", SharedPath);
                return;
            }
            
            // Check for files in storage wanted here, and copy them.
            // Doing this first ensures that any found everywhere can be removed early.
            PulledCount = 0;
            if (localindex.IsPull)
            {
                ReportMessage("\tPulling files from shared space.");
                PulledCount = PullFiles(syncSet);
            }

            // Check for files found in all indexes and in storage, and remove them.
            ReportMessage("\tRemoving shared files that are in every client already.");
            PrunedCount = PruneFiles(syncSet);

            // Where files are found wanting in other machines, push to shared storage.
            // If storage is full, do not copy any further.
            PushedCount = 0;
            if (localindex.IsPush)
            {
                ReportMessage("\tPushing files.");
                PushedCount = PushFiles(syncSet);
            }

            // Report a summary of actions taken.
            if (PulledCount + PushedCount + PrunedCount > 0)
            {
                ReportMessage("Pulled: {0}\tPushed: {1}\tPruned: {2}", PulledCount, PushedCount, PrunedCount);
            }
            else
            {
                ReportMessage("No actions taken.");
            }

            // Report any errors.
            if (_fileManager.Errors.Count > 0)
            {
                ReportMessage("Errors encountered:");
                for (int x = 0; x < _fileManager.Errors.Count; x++)
                {
                    ReportMessage(_fileManager.Errors[x].Message);
                }
            }
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
        #endregion
    }
}


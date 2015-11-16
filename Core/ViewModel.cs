﻿using AssimilationSoftware.MediaSync.Core.Commands;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Mock;
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
        public string LocalPath;
        public string SharedPath;
        private int NumPeers;

        [Obsolete("Too simplistic for performing updates and deletes.")]
        private Dictionary<string, int> FileCounts;
        public ulong SizeLimit;
        private ulong _sizecache;

        public bool VerboseMode;

        public List<string> FileSearches = new List<string>();

        /// <summary>
        /// An asynchronous file copier.
        /// </summary>
        private IFileManager _copyq;

        private SyncSet _options;
        private FileIndex _localSettings;

        private IDataStore _indexer;

        private List<string> _log;
        #endregion

        #region Constructors
        public ViewModel(IDataStore datacontext, string machineId)
        {
            _indexer = datacontext;
            _machineId = machineId;
        }

        public void SetOptions(SyncSet opts, IFileManager filemanager)
        {
            _localSettings = opts.GetIndex(_machineId);
            LocalPath = _localSettings.LocalPath;
            SharedPath = _localSettings.SharedPath;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = opts.ReserveSpace;
            FileSearches = opts.SearchPatterns;
            if (FileSearches.Count == 0)
            {
                FileSearches.Add("*.*");
            }
            _sizecache = 0;
            _options = opts;
            _copyq = filemanager;
            _log = new List<string>();
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
                _indexer.CreateSyncProfile(profile);
                _indexer.SaveChanges();
            }
            else
            {
                StatusMessage = "Profile name already exists: " + name;
            }
        }

        private bool ProfileExists(string name)
        {
            return _indexer.GetAllSyncProfile().Select(x => x.Name.ToLower()).Contains(name.ToLower());
        }

        private SyncSet GetProfile(string name)
        {
            return _indexer.GetAllSyncProfile().Where(x => x.Name.ToLower() == name.ToLower()).First();
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
                _indexer.SaveChanges();
            }
        }

        public void LeaveProfile(SyncSet profile)
        {
            if (profile.ContainsParticipant(this.MachineId))
            {
                profile.Indexes.Remove((from p in profile.Indexes where p.MachineName == this.MachineId select p).Single());
                _indexer.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            _indexer.SaveChanges();
        }

        public void LeaveProfile(string profileName)
        {
            var profile = GetProfile(profileName);
            LeaveProfile(profile);
        }

        public List<string> GetProfileNames()
        {
            var names = _indexer.GetAllSyncProfile().Select(x => x.Name).Distinct();
            return names.ToList();
        }

        public List<SyncSet> Profiles
        {
            get
            {
                return _indexer.GetAllSyncProfile().ToList();
            }
        }

        public List<string> Machines
        {
            get
            {
                return _indexer.GetAllSyncProfile().SelectMany(p => p.Indexes).Select(p => p.MachineName).Distinct().ToList();
            }
        }

        public void RemoveMachine(string machine)
        {
            throw new NotImplementedException();
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

                    _copyq = new QueuedDiskCopier();
                    // TODO: Remove this sub-self-reference.
                    SetOptions(opts, _copyq);
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
                            Sync();
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
        public void IndexFiles()
        {
            var index = _copyq.CreateIndex(_localSettings.LocalPath, _options.SearchPatterns.ToArray());
            index.IsPull = _localSettings.IsPull;
            index.IsPush = _localSettings.IsPush;
            index.MachineName = _localSettings.MachineName;
            index.SharedPath = _localSettings.SharedPath;
            _indexer.CreateFileIndex(index);

            // Compare this index with others.
            NumPeers = _options.Indexes.Count;
            FileCounts = CompareCounts(_options);
        }

        /// <summary>
        /// Compares all index contents to get a count of file existences.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        /// <remarks>
        /// This method is now obsolete for what I want. I need a queue (maybe two) of file actions
        /// derived from a three-way comparison between the current file system, the previous local
        /// index and the current master index.
        /// </remarks>
        public Dictionary<string, int> CompareCounts(SyncSet options)
        {
            var FileCounts = new Dictionary<string, int>();

            // For each other most recent index...
            foreach (var p in options.Indexes)
            {
                foreach (var idxfile in p.Files)
                {
                    var relfile = Path.Combine(idxfile.RelativePath, idxfile.FileName);
                    if (FileCounts.ContainsKey(relfile))
                    {
                        FileCounts[relfile]++;
                    }
                    else
                    {
                        FileCounts[relfile] = 1;
                    }
                }
            }
            return FileCounts;
        }

        /// <summary>
        /// Removes empty folders from the watch path.
        /// </summary>
        internal void ClearEmptyFolders()
        {
            string inbox = SharedPath;
            // Sort by descending length to get leaf nodes first.
            foreach (string dir in from s in Directory.GetDirectories(inbox, "*", SearchOption.AllDirectories)
                                   orderby s.Length descending
                                   select s)
            {
                if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                {
                    // Empty folder. Remove it.
                    try
                    {
                        if (VerboseMode)
                        {
                            Report(new DeleteFile(dir));
                        }
                        _copyq.Delete(dir);
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
        internal int PushFiles()
        {
            // No point trying to push files when they'll all be ignored.
            if (NumPeers == 1)
            {
                ReportMessage("No peers, no point.");
                return 0;
            }

            _sizecache = _copyq.SharedPathSize(_localSettings.SharedPath);
            int pushcount = 0;
            // TODO: Prioritise the least-common files.
            // TODO: Select files that have been updated or created locally.
            //var sortedfilelist = from f in FileCounts.Keys orderby FileCounts[f] select f;
            // For every file in the index
            foreach (string filename in FileCounts.Keys)
            {
                // If the size allocation has been exceeded, stop.
                if (_sizecache > SizeLimit)
                {
                    ReportMessage("Shared space exhausted ({0}). Stopping for now.", VerbaliseBytes(_sizecache));
                    break;
                }
                string filename_local = filename.Replace('\\', Path.DirectorySeparatorChar);
                string targetfile = Path.Combine(SharedPath, filename);

                // If the file is missing from somewhere
                if (FileCounts[filename] < NumPeers)
                {
                    // ...and exists locally
                    if (File.Exists(Path.Combine(LocalPath, filename_local)))
                    {
                        // ...and is not in shared storage
                        if (!File.Exists(targetfile))
                        {
                            if (_copyq.ShouldCopy(filename))
                            {
                                // ...copy it to shared storage.
                                string targetdir = Path.GetDirectoryName(targetfile);
                                if (VerboseMode)
                                {
                                    Report(new CopyCommand(filename_local, targetfile));
                                }
                                _copyq.EnsureFolder(targetdir);
                                string fullpathlocal = Path.Combine(LocalPath, filename_local);
                                _copyq.CopyFile(fullpathlocal, targetfile);
                                // Update size cache.
                                _sizecache += (ulong)new FileInfo(fullpathlocal).Length;
                                pushcount++;
                                StatusMessage = string.Format("\t\tConstructing copy queue: {1} {0}.", _copyq.Count, (_copyq.Count == 1 ? "item" : "items"));
                            }
                            else
                            {
                                ReportMessage("Excluding file {0} because the file copy manager says no.", filename);
                            }
                        }
                        else
                        {
                            //ReportMessage("Excluding file {0} because it is already in shared storage.", file);
                        }
                    }
                    else
                    {
                        //ReportMessage("Excluding file {0} because it does not exist here.", file);
                    }
                }
                else
                {
                    //ReportMessage("Excluding file {0} because it is already everywhere.", file);
                }
            }
            WaitForCopies();
            _copyq.SetNormalAttributes(_localSettings.SharedPath);
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
        internal int PullFiles()
        {
            int pullcount = 0;
            foreach (string FileSearch in FileSearches)
            {
                foreach (string incoming in Directory.GetFiles(SharedPath, FileSearch, SearchOption.AllDirectories))
                {
                    // These paths might need some more processing.
                    // Remove the watch path.
                    string relativepath = incoming.Substring(SharedPath.Length + 1);
                    string targetfile = Path.Combine(LocalPath, relativepath);
                    string targetdir = Path.GetDirectoryName(targetfile);
                    _copyq.EnsureFolder(targetdir);
                    if (!incoming.Equals(targetfile))
                    {
                        if (!File.Exists(targetfile))
                        {
                            try
                            {
                                if (VerboseMode)
                                {
                                    Report(new CopyCommand(incoming, targetfile));
                                }
                                // Linux Bug: Source and target locations the same. Probably a slash problem.
                                _copyq.CopyFile(incoming, targetfile);
                                pullcount++;
                            }
                            catch (Exception)
                            {
                                ReportMessage("Could not copy file: {0}->{1}", incoming, targetfile);
                            }
                        }
                        else if (VerboseMode)
                        {
                            ReportMessage("Skipping file {0}, already exists.", relativepath);
                        }
                    }
                    else
                    {
                        ReportMessage("Error: source file location for move is target location.");
                    }
                }
            }
            WaitForCopies();
            return pullcount;
        }

        /// <summary>
        /// Checks for files in shared storage that are now present everywhere and removes them from shared
        /// storage to make more room.
        /// </summary>
        internal int PruneFiles()
        {
            int prunecount = 0;
            foreach (string FileSearch in FileSearches)
            {
                // For each file in the watch path
                foreach (string filename in Directory.GetFiles(SharedPath, FileSearch, SearchOption.AllDirectories))
                {
                    // If the file exists in all peers
                    string relativefile = filename.Remove(0, SharedPath.Length + 1);
                    if (FileCounts.ContainsKey(relativefile) && FileCounts[relativefile] == NumPeers)
                    {
                        if (VerboseMode)
                        {
                            Report(new DeleteFile(filename));
                        }
                        // Remove it from shared storage.
                        try
                        {
                            _copyq.Delete(filename);
                            prunecount++;
                        }
                        catch (Exception e)
                        {
                            ReportMessage("Error deleting file: {0}", e.Message);
                        }
                    }
                }
            }
            ClearEmptyFolders();
            return prunecount;
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        public void Sync()
        {
            // Check folders, just in case.
            if (!Directory.Exists(SharedPath))
            {
                ReportMessage("Shared storage not available ({0}). Aborting.", SharedPath);
                return;
            }
            // Reset size cache in case this is a multiple-run and other changes have been made.
            _sizecache = 0;

            // Check for files in storage wanted here, and copy them.
            // Doing this first ensures that any found everywhere can be removed early.
            PulledCount = 0;
            if (_localSettings.IsPull)
            {
                ReportMessage("\tPulling files from shared space.");
                PulledCount = PullFiles();
            }

            // Index local files.
            ReportMessage("\tIndexing local files.");
            IndexFiles();
            // Compare this index to other indices.
            // For each index, including local,
            // for each file name,
            // Increment a file name count.
            // Need names of files that are:
            //  1. Here but missing elsewhere. (count < consumers && File.Exists)
            //  2. Elsewhere but missing here. (!File.Exists)
            //  3. Found everywhere.            (count == consumers)
            // TODO: Need separate peer counts for contributors and consumers.

            // Check for files found in all indexes and in storage, and remove them.
            ReportMessage("\tRemoving shared files that are in every client already.");
            PrunedCount = PruneFiles();

            // TODO: Find delete operations to pass on?

            // Where files are found wanting in other machines, push to shared storage.
            // If storage is full, do not copy any further.
            PushedCount = 0;
            if (_localSettings.IsPush)
            {
                ReportMessage("\tPushing files.");
                PushedCount = PushFiles();
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
            if (_copyq.Errors.Count > 0)
            {
                ReportMessage("Errors encountered:");
                for (int x = 0; x < _copyq.Errors.Count; x++)
                {
                    ReportMessage(_copyq.Errors[x].Message);
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
            bool take = _localSettings.IsPull;
            bool give = _localSettings.IsPush;

            Sync();

            _localSettings.IsPull = take;
            _localSettings.IsPush = give;
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
            var first_count = _copyq.Count;
            while (_copyq.Count > 0)
            {
                if (_copyq.Count != lastcount)
                {
                    if (estimate_time)
                    {
                        // Estimate time left via copies per second. Assumes even distribution of file sizes in queue.
                        var cps = (first_count - _copyq.Count) / (DateTime.Now - started_waiting).TotalSeconds;
                        var timeleft = new TimeSpan(0, 0, _copyq.Count / (int)cps);
                        StatusMessage = string.Format("\t\tWaiting on {0} {1}... ({2:hh:mm:ss} remaining)", _copyq.Count, (_copyq.Count == 1 ? "copy" : "copies"), timeleft);
                    }
                    else
                    {
                        StatusMessage = string.Format("\t\tWaiting on {0} {1}...", _copyq.Count, (_copyq.Count == 1 ? "copy" : "copies"));
                    }
                    lastcount = _copyq.Count;
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
                return _copyq.Errors;
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


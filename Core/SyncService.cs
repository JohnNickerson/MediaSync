using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Mock;
using AssimilationSoftware.MediaSync.Core.Properties;
using System.ComponentModel;

namespace AssimilationSoftware.MediaSync.Core
{
    /// <summary>
    /// A file synchronising service.
    /// </summary>
    public class SyncService : INotifyPropertyChanged
    {
        #region Fields
        public string LocalPath;
        public string SharedPath;
        private int NumPeers;
        private Dictionary<string, int> FileCounts;
        public ulong SizeLimit;
        private ulong _sizecache;

        /// <summary>
        /// A flag that indicates whether the processes should be simulated (no actual file operations).
        /// </summary>
        /// <remarks>
        /// TODO: Allow for creation of an update script rather than just simulation output.
        /// </remarks>
        private bool Simulate;

        public bool VerboseMode;

        public List<string> FileSearches = new List<string>();

		/// <summary>
		/// An asynchronous file copier.
		/// </summary>
		private IFileManager _copyq;

		private SyncProfile _options;
        private ProfileParticipant _localSettings;

        private IIndexMapper _indexer;

        private List<string> _log;

        private string _status;
        #endregion

        #region Events
        /// <summary>
        /// An event that indicates a property has changed value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event with given arguments.
        /// </summary>
        /// <param name="e"></param>
        public void RaisePropertyChanged(params string[] propnames)
        {
            foreach (string prop in propnames)
            {
                var e = new PropertyChangedEventArgs(prop);
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, e);
                }
            }
        }
        #endregion

        #region Constructors
        public SyncService(SyncProfile opts, IIndexMapper indexer, IFileManager filemanager, bool simulate, string thismachine)
        {
            _localSettings = opts.GetParticipant(thismachine);
            LocalPath = _localSettings.LocalPath;
            SharedPath = _localSettings.SharedPath;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = opts.ReserveSpace;
            Simulate = simulate;
            FileSearches = opts.SearchPatterns;
            if (FileSearches.Count == 0)
            {
                FileSearches.Add("*.*");
            }
            _sizecache = 0;
			_options = opts;
            if (Simulate)
            {
                _copyq = new MockFileManager();
                _indexer = new MockIndexMapper();
            }
            else
            {
                _copyq = filemanager;
                _indexer = indexer;
            }
            _log = new List<string>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles()
        {
            var index = _copyq.CreateIndex();
            _indexer.Save(index);

            // Compare this index with others.
            NumPeers = _options.Participants.Count;
            FileCounts = CompareCounts(_options);
            // TODO: Construct or load an action queue.
        }

        /// <summary>
        /// Compares all index contents to get a count of file existences.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        public Dictionary<string, int> CompareCounts(SyncProfile options)
        {
            var FileCounts = new Dictionary<string, int>();

            // For each other most recent index...
            foreach (var p in options.Participants)
            {
                var f = _indexer.LoadLatest(p.MachineName, options.Name);
                if (f != null)
                {
                    foreach (var idxfile in f.Files)
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
                            Report(new SyncOperation(dir));
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
            RaisePropertyChanged("Log");
        }

        private void Report(SyncOperation op)
        {
            switch (op.Action)
            {
                case SyncOperation.SyncAction.Copy:
                    _log.Add(string.Format("Copying:{2}\t{0}{2}\t->{2}\t{1}", op.SourceFile, op.TargetFile, Environment.NewLine));
                    break;
                case SyncOperation.SyncAction.Delete:
                    _log.Add(string.Format("Deleting {0}", op.TargetFile));
                    break;
                default:
                    _log.Add(string.Format("Unknown sync action: {0}", op.Action));
                    break;
            }
            RaisePropertyChanged("Log");
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

            _sizecache = _copyq.SharedPathSize();
            int pushcount = 0;
            // TODO: Prioritise the least-common files.
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
                                    Report(new SyncOperation(filename_local, targetfile, SyncOperation.SyncAction.Copy));
                                }
                                _copyq.EnsureFolder(targetdir);
                                string fullpathlocal = Path.Combine(LocalPath, filename_local);
                                _copyq.CopyFile(fullpathlocal, targetfile);
                                // Update size cache.
                                _sizecache += (ulong)new FileInfo(fullpathlocal).Length;
                                pushcount++;
                                Status = string.Format("\t\tConstructing copy queue: {1} {0}.", _copyq.Count, (_copyq.Count == 1 ? "item" : "items"));
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
            _copyq.SetNormalAttributes();
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
                                    Report(new SyncOperation(incoming, targetfile, SyncOperation.SyncAction.Copy));
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
                            Report(new SyncOperation(filename));
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
            if (_localSettings.Consumer)
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
			if (_localSettings.Contributor)
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
            bool take = _localSettings.Consumer;
            bool give = _localSettings.Contributor;

            Sync();

            _localSettings.Consumer = take;
            _localSettings.Contributor = give;
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
                        Status = string.Format("\t\tWaiting on {0} {1}... ({2:hh:mm:ss} remaining)", _copyq.Count, (_copyq.Count == 1 ? "copy" : "copies"), timeleft);
                    }
                    else
                    {
                        Status = string.Format("\t\tWaiting on {0} {1}...", _copyq.Count, (_copyq.Count == 1 ? "copy" : "copies"));
                    }
                    lastcount = _copyq.Count;
				}
				Thread.Sleep(1000);
			}
		}
        #endregion

        #region Properties
        public int PulledCount { get; set; }
        public int PushedCount { get; set; }
        public int PrunedCount { get; set; }
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
                RaisePropertyChanged("Log");
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        #endregion
    }
}

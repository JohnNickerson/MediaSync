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
using AssimilationSoftware.MediaSync.Core.Indexing;
using AssimilationSoftware.MediaSync.Core.Views;

namespace AssimilationSoftware.MediaSync.Core
{
    /// <summary>
    /// A photo synchronising service.
    /// </summary>
    public class SyncService
    {
        #region Fields
        public string LocalPath;
        public string WatchPath;
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

        /// <summary>
        /// A view to show output from operations.
        /// </summary>
        private IOutputView _view;

        public string FileSearch = "*.*";

		/// <summary>
		/// An asynchronous file copier.
		/// </summary>
		public IFileManager _copyq;

		private SyncProfile _options;

        private IIndexService _indexer;
        #endregion

        #region Constructors
        public SyncService(SyncProfile opts, IOutputView view, IIndexService indexer, IFileManager filemanager)
        {
            LocalPath = opts.LocalPath;
            WatchPath = opts.SharedPath;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = opts.ReserveSpace;
            Simulate = opts.Simulate;
            FileSearch = opts.SearchPattern;
            _view = view;
            _sizecache = 0;
			_options = opts;
            if (opts.Simulate)
            {
                _copyq = new MockFileManager();
                _indexer = new MockIndexer();
            }
            else
            {
                _copyq = filemanager;
                _indexer = indexer;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles()
        {
            _copyq.CreateIndex(_indexer);
            // Overwrite any old index that exists.
            _indexer.WriteIndex();

            // Compare this index with others.
            NumPeers = _indexer.PeerCount;
            FileCounts = _indexer.CompareCounts();
        }

        /// <summary>
        /// Removes empty folders from the watch path.
        /// </summary>
        internal void ClearEmptyFolders()
        {
            string inbox = WatchPath;
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
                        _view.Report(new SyncOperation(dir));
                        _copyq.Delete(dir);
                    }
                    catch
                    {
                        _view.WriteLine("Could not delete apparently empty folder: {0}", dir);
                    }
                }
            }
        }

        /// <summary>
        /// Pushes files to shared storage where they are found wanting in other peers.
        /// </summary>
        internal void PushFiles()
        {
            // No point trying to push files when they'll all be ignored.
            if (NumPeers == 1)
            {
                _view.WriteLine("No peers, no point.");
                return;
            }

            // TODO: Prioritise the least-common files.
            //var sortedfilelist = from f in FileCounts.Keys orderby FileCounts[f] select f;
            // For every file in the index
            foreach (string filename in FileCounts.Keys)
            {
                // If the size allocation has been exceeded, stop.
                if (_copyq.WatchPathSize() > SizeLimit)
                {
                    _view.WriteLine("Shared space exhausted. Stopping for now.");
                    break;
                }
                string filename_local = filename.Replace('\\', Path.DirectorySeparatorChar);
                string targetfile = Path.Combine(WatchPath, filename);

                // If the file is missing from somewhere
                if (FileCounts[filename] < NumPeers
                    // ...and exists locally
                    && File.Exists(Path.Combine(LocalPath, filename_local))
                    // ...and is not in shared storage
                    && !File.Exists(targetfile)
                    && _copyq.ShouldCopy(filename))
                {
                    // ...copy it to shared storage.
                    string targetdir = Path.GetDirectoryName(targetfile);
                    _view.Report(new SyncOperation(filename_local, targetfile, SyncOperation.SyncAction.Copy));
                    _copyq.EnsureFolder(targetdir);
					string fullpathlocal = Path.Combine(LocalPath, filename_local);
					_copyq.CopyFile(fullpathlocal, targetfile);
					// Update size cache.
					_sizecache += (ulong)new FileInfo(fullpathlocal).Length;
                }
            }
			WaitForCopies();
		}

        /// <summary>
        /// Copies files from shared storage if they are not present locally.
        /// </summary>
        internal void PullFiles()
        {
            foreach (string incoming in Directory.GetFiles(WatchPath, FileSearch, SearchOption.AllDirectories))
            {
                // These paths might need some more processing.
                // Remove the watch path.
                string relativepath = incoming.Substring(WatchPath.Length + 1);
                string targetfile = Path.Combine(LocalPath, relativepath);
                string targetdir = Path.GetDirectoryName(targetfile);
                _copyq.EnsureFolder(targetdir);
                if (!incoming.Equals(targetfile))
                {
                    try
                    {
                        _view.Report(new SyncOperation(incoming, targetfile, SyncOperation.SyncAction.Copy));
                        // Linux Bug: Source and target locations the same. Probably a slash problem.
                        _copyq.CopyFile(incoming, targetfile);
                    }
                    catch (Exception)
                    {
                        _view.WriteLine("Could not copy file: {0}->{1}", incoming, targetfile);
                    }
                }
                else
                {
                    _view.WriteLine("Error: source file location for move is target location.");
                }
            }
			WaitForCopies();
		}

        /// <summary>
        /// Checks for files in shared storage that are now present everywhere and removes them from shared
        /// storage to make more room.
        /// </summary>
        internal void PruneFiles()
        {
            // For each file in the watch path
            foreach (string filename in Directory.GetFiles(WatchPath, FileSearch, SearchOption.AllDirectories))
            {
                // If the file exists in all peers
                string relativefile = filename.Remove(0, WatchPath.Length + 1);
                if (FileCounts.ContainsKey(relativefile) && FileCounts[relativefile] == NumPeers)
                {
                    _view.Report(new SyncOperation(filename));
                    // Remove it from shared storage.
                    _copyq.Delete(filename);
                }
            }
            ClearEmptyFolders();
        }

        /// <summary>
        /// Performs a 4-step, shared storage, limited space, partial sync operation as configured.
        /// </summary>
        public void Sync()
        {
            // Setup, just in case.
            if (!Directory.Exists(WatchPath))
            {
                _view.WriteLine("Shared storage not available ({0}). Aborting.", WatchPath);
                return;
            }
            // Reset size cache in case this is a multiple-run and other changes have been made.
            _sizecache = 0;

            // Check for files in storage wanted here, and copy them.
            // Doing this first ensures that any found everywhere can be removed early.
			if (_options.Consumer)
			{
                _view.WriteLine("Pulling files from shared space.");
				PullFiles();
			}

            // Index local files.
            _view.WriteLine("Indexing local files.");
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
            _view.WriteLine("Removing shared files that are in every client already.");
            PruneFiles();

            // TODO: Find delete operations to pass on?

            // Where files are found wanting in other machines, push to shared storage.
            // If storage is full, do not copy any further.
			if (_options.Contributor)
			{
                _view.WriteLine("Pushing files.");
				PushFiles();
			}

			// Report any errors.
			if (_copyq.Errors.Count > 0)
			{
				_view.Status = "Errors encountered:";
				for (int x = 0; x < _copyq.Errors.Count; x++)
				{
					_view.WriteLine(_copyq.Errors[x].Message);
				}
			}
		}

		/// <summary>
		/// Spins until all async file copies are complete.
		/// </summary>
		private void WaitForCopies()
		{
			// Wait for file copies to finish.
            int lastcount = 0;
			while (_copyq.Count > 0)
			{
				if (_copyq.Count != lastcount)
				{
					_view.Status = string.Format("Waiting on {0} copies...", _copyq.Count);
					lastcount = _copyq.Count;
				}
				Thread.Sleep(1000);
			}
		}
        #endregion
    }
}

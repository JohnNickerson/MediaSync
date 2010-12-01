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
    public class Service
    {
        #region Fields
        public string SourcePath;
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

        private string filesearch = "*.*";

        /// <summary>
        /// A list of file name patterns to exclude from synchronisation.
        /// </summary>
        public List<Regex> Exclusions;

		/// <summary>
		/// An asynchronous file copier.
		/// </summary>
		public FileCopyQueue _copyq;

		private SyncOptions _options;

        private IIndexService _indexer;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new photo sync service instance.
        /// </summary>
        /// <param name="source">The photo folder location.</param>
        /// <param name="watch">The sync folder, where requests are made and fulfilled.</param>
        [Obsolete]
		public Service(string source, string watch, ulong reservesize, bool simulate, IOutputView view)
        {
            SourcePath = source;
            WatchPath = watch;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = reservesize;
            Simulate = simulate;
            Exclusions = new List<Regex>();
            Exclusions.Add(new Regex(".*_index.txt"));
            _view = view;
            _sizecache = 0;
			_copyq = new FileCopyQueue();
        }

        public Service(SyncOptions opts, IOutputView view, IIndexService indexer)
        {
            SourcePath = opts.SourcePath;
            WatchPath = opts.SharedPath;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = opts.ReserveSpace;
            Simulate = opts.Simulate;
            Exclusions = new List<Regex>();
            foreach (string r in opts.ExcludePatterns)
            {
                Exclusions.Add(new Regex(r));
            }
            _view = view;
            _sizecache = 0;
			_copyq = new FileCopyQueue();
			_options = opts;
            _indexer = indexer;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles()
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(SourcePath);
            // While the queue is not empty,
            while (queue.Count > 0)
            {
                // Dequeue a folder to process.
                string folder = queue.Dequeue();
                // Enqueue subfolders.
                foreach (string subfolder in Directory.GetDirectories(folder))
                {
                    queue.Enqueue(subfolder);
                }
                // Add all image files to the index.
                foreach (string file in Directory.GetFiles(folder, filesearch))
                {
                    if (!Exclude(file))
                    {
                        // Remove the base path.
                        string trunc_file = file.Remove(0, this.SourcePath.Length + 1).Replace("/", "\\");
                        _indexer.Add(trunc_file);
                        FileCounts[trunc_file] = 1;
                    }
                }
            }
            // Overwrite any old index that exists.
            if (Simulate)
            {
                _view.WriteLine("Simulation run: no index writing.");
            }
            else
            {
                _indexer.WriteIndex();
			}

            // Compare this index with others.
            NumPeers = _indexer.PeerCount;
            FileCounts = _indexer.CompareCounts();
        }

        /// <summary>
        /// Checks whether a file name matches any exclusion pattern.
        /// </summary>
        /// <param name="file">The file name to test.</param>
        /// <returns>True if any of the exclusion patterns match the given file name, false otherwise.</returns>
        private bool Exclude(string file)
        {
            bool result = false;
            string testfile = Path.GetFileName(file);

            foreach (Regex r in Exclusions)
            {
                if (r.IsMatch(testfile))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Combines multiple paths into one string according to environment settings.
        /// </summary>
        /// <param name="paths">A list of paths to combine.</param>
        /// <returns>One path value, combined into an environment-appropriate string.</returns>
        public static string PathCombine(params string[] paths)
        {
            Stack<string> stack = new Stack<string>();
            for (int x = 0; x < paths.Length; x++)
            {
                stack.Push(paths[x]);
            }
            while (stack.Count > 1)
            {
                string path2 = stack.Pop();
                string path1 = stack.Pop();
                stack.Push(Path.Combine(path1, path2));
            }
            return stack.Pop();
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
                        if (!Simulate)
                        {
                            Directory.Delete(dir);
                        }
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
            if (NumPeers == 1) return;

            // TODO: Prioritise the least-common files.
            //var sortedfilelist = from f in FileCounts.Keys orderby FileCounts[f] select f;
            // For every file in the index
            foreach (string filename in FileCounts.Keys)
            {
                // If the size allocation has been exceeded, stop.
                if (WatchPathSize() > SizeLimit)
                {
                    break;
                }
                string filename_local = filename.Replace('\\', Path.DirectorySeparatorChar);
                string targetfile = PathCombine(WatchPath, filename);

                // If the file is missing from somewhere
                if (FileCounts[filename] < NumPeers
                    // ...and exists locally
                    && File.Exists(PathCombine(SourcePath, filename_local))
                    // ...and is not in shared storage
                    && !File.Exists(targetfile)
                    // ...and it doesn't match an exclusion pattern
                    && !Exclude(filename))
                {
                    // ...copy it to shared storage.
                    string targetdir = Path.GetDirectoryName(targetfile);
                    _view.Report(new SyncOperation(filename_local, targetfile, SyncOperation.SyncAction.Copy));
                    if (!Simulate)
                    {
                        if (!Directory.Exists(targetdir))
                            Directory.CreateDirectory(targetdir);
						string fullpathlocal = PathCombine(SourcePath, filename_local);
						_copyq.CopyFile(fullpathlocal, targetfile);
						// Update size cache.
						_sizecache += (ulong)new FileInfo(fullpathlocal).Length;
                    }
                }
            }
			WaitForCopies();
		}

        /// <summary>
        /// Copies files from shared storage if they are not present locally.
        /// </summary>
        internal void PullFiles()
        {
            foreach (string incoming in Directory.GetFiles(WatchPath, filesearch, SearchOption.AllDirectories))
            {
                // These paths might need some more processing.
                // Remove the watch path.
                string relativepath = incoming.Substring(WatchPath.Length + 1);
                string targetfile = PathCombine(SourcePath, relativepath);
                string targetdir = Path.GetDirectoryName(targetfile);
                if (!Directory.Exists(targetdir) && !Simulate)
                    Directory.CreateDirectory(targetdir);
                if (!incoming.Equals(targetfile))
                {
                    if (!File.Exists(targetfile))
                    {
                        try
                        {
                            _view.Report(new SyncOperation(incoming, targetfile, SyncOperation.SyncAction.Copy));
                            if (!Simulate && !Exclude(incoming))
                            {
                                // Linux Bug: Source and target locations the same. Probably a slash problem.
                                _copyq.CopyFile(incoming, targetfile);
                            }
                        }
                        catch (Exception)
                        {
                            _view.WriteLine("Could not copy file: {0}->{1}", incoming, targetfile);
                        }
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
            foreach (string filename in Directory.GetFiles(WatchPath, filesearch, SearchOption.AllDirectories))
            {
                // If the file exists in all peers
                string relativefile = filename.Remove(0, WatchPath.Length + 1);
                if (FileCounts.ContainsKey(relativefile) && FileCounts[relativefile] == NumPeers)
                {
                    _view.Report(new SyncOperation(filename));
                    if (!Simulate)
                    {
                        // Remove it from shared storage.
                        File.Delete(filename);
                    }
                }
            }
            ClearEmptyFolders();
        }

        /// <summary>
        /// Gets the size of all files in the watch path combined.
        /// </summary>
        /// <returns>A size, in bytes, representing all files combined.</returns>
        public ulong WatchPathSize()
        {
            if (_sizecache == 0)
            {
                ulong total = 0;

                foreach (string filename in Directory.GetFiles(WatchPath, filesearch, SearchOption.AllDirectories))
                {
                    total += (ulong)new FileInfo(filename).Length;
                }
                _sizecache = total;
                return total;
            }
            else
            {
                return _sizecache;
            }
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
			// TODO: Only pull files for consumer profiles.
			if (_options.Consumer)
			{
				PullFiles();
			}

            // Index local files.
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
            PruneFiles();

            // Where files are found wanting in other machines, push to shared storage.
            // If storage is full, do not copy any further.
			// TODO: Only push files for contributor profiles.
			if (_options.Contributor)
			{
				PushFiles();
			}

			// Report any errors.
			if (_copyq.Errors.Count > 0)
			{
				_view.WriteLine("Errors encountered:");
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
					_view.WriteLine("Waiting on {0} copies...", _copyq.Count);
					lastcount = _copyq.Count;
				}
				Thread.Sleep(1000);
			}
		}
        #endregion
    }
}

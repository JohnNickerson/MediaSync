using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Client
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

        /// <summary>
        /// A flag that indicates whether the processes should be simulated (no actual file operations).
        /// </summary>
        private bool Simulate;

        /// <summary>
        /// A view to show output from operations.
        /// </summary>
        private IOutputView _view;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new photo sync service instance.
        /// </summary>
        /// <param name="source">The photo folder location.</param>
        /// <param name="watch">The sync folder, where requests are made and fulfilled.</param>
        public Service(string source, string watch, ulong reservesize, bool simulate, IOutputView view)
        {
            SourcePath = source;
            WatchPath = watch;
            NumPeers = 0;
            FileCounts = new Dictionary<string, int>();
            SizeLimit = reservesize;
            Simulate = simulate;
            _view = view;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles()
        {
            string indexfile = PathCombine(WatchPath, string.Format("{0}_index.txt", Environment.MachineName));

            Queue<string> queue = new Queue<string>();
            queue.Enqueue(SourcePath);
            List<string> contents = new List<string>();
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
                foreach (string file in Directory.GetFiles(folder, "*.jpg"))
                {
                    // Remove the base path.
                    string trunc_file = file.Remove(0, this.SourcePath.Length + 1).Replace("/", "\\");
                    contents.Add(trunc_file);
                    FileCounts[trunc_file] = 1;
                }
            }
            // Overwrite any old index that exists.
            if (Simulate)
            {
                _view.WriteLine("Simulation run: no index writing.");
            }
            else
            {
                File.WriteAllLines(indexfile, contents.ToArray());
            }

            // Compare this index with others.
            NumPeers = Directory.GetFiles(WatchPath, "*_index.txt").Length;

            foreach (string otherindex in Directory.GetFiles(WatchPath, "*_index.txt"))
            {
                if (!otherindex.Equals(indexfile))
                {
                    foreach (string idxfilename in File.ReadAllLines(otherindex))
                    {
                        if (FileCounts.ContainsKey(idxfilename))
                        {
                            FileCounts[idxfilename]++;
                        }
                        else
                        {
                            FileCounts[idxfilename] = 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares this machine's stock with another machine's index and requests any missing files.
        /// </summary>
        /// <param name="machine">The machine whose index to compare with.</param>
        [Obsolete]
        public void RequestFiles()
        {
            string machine;
            foreach (string folder in Directory.GetDirectories(WatchPath))
            {
                if (!folder.EndsWith(Environment.MachineName))
                {
                    string[] parts = folder.Split(Path.DirectorySeparatorChar);
                    machine = (parts[parts.Length - 1]);
                    string indexfile = PathCombine(WatchPath, machine, "index.txt");
                    if (File.Exists(indexfile))
                    {
                        // Open other machine's index.
                        string[] index = File.ReadAllLines(PathCombine(WatchPath, machine, "index.txt"));
                        
                        // For each line...
                        foreach (string filename in index)
                        {
                            // Check whether the listed file exists locally.
                            if (!File.Exists(PathCombine(SourcePath, filename)))
                            {
                                // If not, construct a request.
                                string[] request = new string[] { filename, Environment.MachineName };
                                _view.WriteLine("Requesting {0} from {1}", filename, machine);
                                // Bug: This results in invalid file names.
                                File.WriteAllLines(PathCombine(WatchPath, machine, Path.GetFileName(filename.Replace('\\', Path.DirectorySeparatorChar)) + ".get"), request);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Combines multiple paths into one string according to environment settings.
        /// </summary>
        /// <param name="paths">A list of paths to combine.</param>
        /// <returns>One path value, combined into an environment-appropriate string.</returns>
        private static string PathCombine(params string[] paths)
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
            foreach (string dir in Directory.GetDirectories(inbox, "*", SearchOption.AllDirectories))
            {
                if (Directory.GetFiles(dir).Length == 0)
                {
                    // Empty folder. Remove it.
                    try
                    {
                        if (Simulate)
                        {
                            _view.WriteLine("Simulation: remove directory {0}", dir);
                        }
                        else
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
                    && !File.Exists(targetfile))
                {
                    // ...copy it to shared storage.
                    string targetdir = Path.GetDirectoryName(targetfile);
                    if (Simulate)
                    {
                        _view.WriteLine("Simulation: copy -> {0}", targetfile);
                    }
                    else
                    {
                        _view.WriteLine("Copying -> {0}", targetfile);
                        if (!Directory.Exists(targetdir))
                            Directory.CreateDirectory(targetdir);
                        File.Copy(PathCombine(SourcePath, filename_local), targetfile);
                    }
                }
            }
        }

        /// <summary>
        /// Copies files from shared storage if they are not present locally.
        /// </summary>
        internal void PullFiles()
        {
            foreach (string incoming in Directory.GetFiles(WatchPath, "*.jpg", SearchOption.AllDirectories))
            {
                // These paths might need some more processing.
                // Remove the watch path.
                string relativepath = incoming.Substring(WatchPath.Length + 1);
                string targetfile = PathCombine(SourcePath, relativepath);
                string targetdir = Path.GetDirectoryName(targetfile);
                if (!Directory.Exists(targetdir) && !Simulate)
                    Directory.CreateDirectory(targetdir);
                if (!incoming.Equals(targetfile)
                    && !File.Exists(targetfile))
                {
                    try
                    {
                        if (Simulate)
                        {
                            _view.WriteLine("Simulation: copy <- {0}", targetfile);
                        }
                        else
                        {
                            // Linux Bug: Source and target locations the same. Probably a slash problem.
                            _view.WriteLine("Copying <- {0}", targetfile);
                            File.Copy(incoming, targetfile);
                        }
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
        }

        /// <summary>
        /// Checks for files in shared storage that are now present everywhere and removes them from shared
        /// storage to make more room.
        /// </summary>
        internal void PruneFiles()
        {
            // For each file in the watch path
            foreach (string filename in Directory.GetFiles(WatchPath, "*.jpg", SearchOption.AllDirectories))
            {
                // If the file exists in all peers
                string relativefile = filename.Remove(0, WatchPath.Length + 1);
                if (FileCounts.ContainsKey(relativefile) && FileCounts[relativefile] == NumPeers)
                {
                    if (Simulate)
                    {
                        _view.WriteLine("Simulation: remove {0}", filename);
                    }
                    else
                    {
                        // Remove it from shared storage.
                        _view.WriteLine("Removing {0}", filename);
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
        private ulong WatchPathSize()
        {
            ulong total = 0;

            foreach (string filename in Directory.GetFiles(WatchPath, "*.jpg"))
            {
                total += (ulong)File.ReadAllBytes(filename).Length;
            }

            return total;
        }
        #endregion

        public void Sync()
        {
            // Check for files in storage wanted here, and copy them.
            // Doing this first ensures that any found everywhere can be removed early.
            PullFiles();

            // Index local files.
            IndexFiles();
            // Compare this index to other indices.
            // For each index, including local,
            // for each file name,
            // Increment a file name count.
            // Need names of files that are:
            //  1. Here but missing elsewhere. (count < peers && File.Exists)
            //  2. Elsewhere but missing here. (!File.Exists)
            //  3. Found everywhere.            (count == peers)

            // Check for files found in all indexes and in storage, and remove them.
            PruneFiles();

            // Where files are found wanting in other machines, push to shared storage.
            // If storage is full, do not copy any further.
            PushFiles();
        }
    }
}

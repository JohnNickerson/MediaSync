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
    class Service
    {
        #region Fields
        public string SourcePath;
        public string WatchPath;
        private List<string> ActiveRequests;
        private string InboxPath;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new photo sync service instance.
        /// </summary>
        /// <param name="source">The photo folder location.</param>
        /// <param name="watch">The sync folder, where requests are made and fulfilled.</param>
        public Service(string source, string watch)
        {
            SourcePath = source;
            WatchPath = watch;
            ActiveRequests = new List<string>();
            InboxPath = PathCombine(WatchPath, Environment.MachineName);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Checks the machine inbox for requests and images.
        /// </summary>
        public void CheckInbox()
        {
            if (!Directory.Exists(InboxPath))
            {
                Directory.CreateDirectory(InboxPath);
            }

            // Look for incoming photos.
            // TODO: Handle all image file types.
            foreach (string incoming in Directory.GetFiles(InboxPath, "*.jpg", SearchOption.AllDirectories))
            {
                // These paths might need some more processing.
                // Remove the watch path.
                string relativepath = incoming.Substring(WatchPath.Length + Environment.MachineName.Length + 2);
                string targetfile = PathCombine(SourcePath, relativepath);
                string targetdir = Path.GetDirectoryName(targetfile);
                if (!Directory.Exists(targetdir))
                    Directory.CreateDirectory(targetdir);
                if (!incoming.Equals(targetfile))
                {
                    try
                    {
                        // Bug: Linux not moving files. Source and target locations the same.
                        File.Move(incoming, targetfile);
                        if (ActiveRequests.Contains(relativepath))
                        {
                            ActiveRequests.Remove(relativepath);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not move file: {0}->{1}", incoming, targetfile);
                    }
                }
                else
                {
                    Console.WriteLine("Error: source file location for move is target location.");
                }
                Console.WriteLine("Moving {0} to {1}", incoming, Path.Combine(SourcePath, incoming));
            }
        }

        /// <summary>
        /// Compares this machine's stock with another machine's index and requests any missing files.
        /// </summary>
        /// <param name="machine">The machine whose index to compare with.</param>
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
                        int requestcount = Directory.GetFiles(PathCombine(WatchPath, machine), "*.get").Length + ActiveRequests.Count;
                        
                        // For each line...
                        foreach (string filename in index)
                        {
                            // Check whether the listed file exists locally.
                            if (!File.Exists(PathCombine(SourcePath, filename))
                                // Only make a few requests at a time.
                                && requestcount < 10
                                // Don't keep requesting the same files from everywhere
                                && !ActiveRequests.Contains(filename))
                            {
                                // If not, construct a request.
                                string[] request = new string[] { filename, Environment.MachineName };
                                Console.WriteLine("Requesting {0} from {1}", filename, machine);
                                // Bug: This results in invalid file names.
                                File.WriteAllLines(PathCombine(WatchPath, machine, Path.GetFileName(filename.Replace('\\', Path.DirectorySeparatorChar)) + ".get"), request);
                                requestcount++;
                                ActiveRequests.Add(filename);
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
            string inbox = Path.Combine(WatchPath, Environment.MachineName);
            foreach (string dir in Directory.GetDirectories(inbox))
            {
                if (Directory.GetFiles(dir).Length == 0)
                {
                    // Empty folder. Remove it.
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch
                    {
                        Console.WriteLine("Could not delete apparently empty folder: {0}", dir);
                    }
                }
            }
        }

        /// <summary>
        /// Removes requests that have somehow been missed, perhaps due to Dropbox lag.
        /// </summary>
        public void RemoveMissedRequests()
        {
            if (ActiveRequests.Count > 100)
            {
                for (int x = 0; x < ActiveRequests.Count; )
                {
                    string filename = ActiveRequests[x];
                    if (!File.Exists(PathCombine(SourcePath, filename)))
                    {
                        ActiveRequests.RemoveAt(x);
                    }
                    else
                    {
                        x++;
                    }
                }
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of currently-active requests.
        /// </summary>
        public int OutgoingRequests
        {
            get
            {
                return ActiveRequests.Count;
            }
        }

        public int IncomingRequests
        {
            get
            {
                return Directory.GetFiles(InboxPath, "*.get").Length;
            }
        }
        #endregion
    }
}

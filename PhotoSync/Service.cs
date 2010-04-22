using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PhotoSync
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

            // Look for request files.
            foreach (string request in Directory.GetFiles(InboxPath, "*.get"))
            {
                // Get the file name.
                string[] requestdata = File.ReadAllLines(request);
                string filename = requestdata[0];
                // Get the destination machine name.
                string machine = requestdata[1];
                // Copy the file to the destination machine's inbox.
                // Make sure not to exceed size limits. If there are too many files already present, leave
                // the request intact to fulfil later.
                string sourcefile = PathCombine(SourcePath, filename);
                string targetfile = PathCombine(WatchPath, machine, filename);
                if (!Directory.Exists(Path.GetDirectoryName(targetfile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(targetfile));
                if (sourcefile != targetfile)
                {
                    try
                    {
                        File.Copy(sourcefile, targetfile);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not copy file:{0}->{1}", sourcefile, targetfile);
                    }
                }
                else
                {
                    Console.WriteLine("Error: source file location for copy is target location.");
                }
                Console.WriteLine("Copying {0} to {1}", Path.Combine(SourcePath, filename), PathCombine(WatchPath, machine, filename));
                // Delete the request, as long as it has been successfully processed.
                File.Delete(request);
            }
        }

        /// <summary>
        /// Creates an index file for this machine.
        /// </summary>
        public void IndexFiles()
        {
            string indexfile = PathCombine(WatchPath, Environment.MachineName, "index.txt");

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
                    contents.Add(file.Remove(0, this.SourcePath.Length + 1));
                }
            }
            // Overwrite any old index that exists.
            File.WriteAllLines(indexfile, contents.ToArray());
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using System.Diagnostics;
using AssimilationSoftware.MediaSync.Core.Commands;

namespace AssimilationSoftware.MediaSync.Core
{
	/// <summary>
	/// A class to handle asynchronous file copying.
	/// </summary>
    public class QueuedDiskCopier : IFileManager
	{
		#region Types
		/// <summary>
		/// A delegate type for performing asynchronous file copies.
		/// </summary>
		/// <param name="source">The file to be copied.</param>
		/// <param name="dest">The location to copy to.</param>
		delegate void CopyFileDelegate(string source, string dest);

        /// <summary>
        /// A delegate type for performing asynchronous directory deletions.
        /// </summary>
        /// <param name="dir">The path of the directory to delete.</param>
        delegate void DeleteDirectoryDelegate(string dir);

        /// <summary>
        /// A delegate type for performing asynchronous file deletions.
        /// </summary>
        /// <param name="file">The path of the file to delete.</param>
        delegate void DeleteFileDelegate(string file);

        /// <summary>
        /// A delegate type for performing async file moves.
        /// </summary>
        /// <param name="source">The file to be copied.</param>
        /// <param name="dest">The location to copy to.</param>
        delegate void MoveFileDelegate(string source, string dest);
		#endregion

		#region Variables
		/// <summary>
		/// A list of in-progress file copy actions.
		/// </summary>
		private List<IAsyncResult> InProgressActions;

		/// <summary>
		/// Sync operations waiting to go ahead.
		/// </summary>
		private Queue<FileCommand> PendingFileActions;

		/// <summary>
		/// The maximum number of simultaneous copies to perform.
		/// </summary>
		private int MaxActions;

        private List<Exception> _errors;
        private IFileHashProvider _fileHasher;

        /// <summary>
        /// A list of errors that occurred during copies.
        /// </summary>
        List<Exception> IFileManager.Errors
        {
            get
            {
                return _errors;
            }
        }
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new asynchronous file copy service.
		/// </summary>
		public QueuedDiskCopier(IFileHashProvider hashMaker)
        {
			InProgressActions = new List<IAsyncResult>();
			PendingFileActions = new Queue<FileCommand>();
			MaxActions = 2;

            _fileHasher = hashMaker;
			_errors = new List<Exception>();
		}
		#endregion

		#region Methods
        /// <summary>
		/// Starts an asynchronous file copy operation.
		/// </summary>
		/// <param name="source">The source file to copy.</param>
		/// <param name="target">The destination where the file will be copied to.</param>
		public void CopyFile(string source, string target)
        {
            if (!source.Equals(target) && !File.Exists(target))
            {
                lock (PendingFileActions)
                {
                    PendingFileActions.Enqueue(new CopyCommand(source, target));
                }
                BeginThreads();
            }
        }

        public void CopyFile(string basePath, string relativePath, string targetPath)
        {
            CopyFile(Path.Combine(basePath, relativePath), Path.Combine(targetPath, relativePath));
        }

        public void MoveFile(string source, string target, bool overwrite)
        {
            if (!source.Equals(target) && !File.Exists(target))
            {
                lock (PendingFileActions)
                {
                    PendingFileActions.Enqueue(new MoveCommand(source, target));
                }
                BeginThreads();
            }
        }

        private void BeginThreads()
        {
            while (InProgressActions.Count < MaxActions && 0 < PendingFileActions.Count)
            {
                // Pop a pending action off the queue.
                FileCommand op = null;
                lock (PendingFileActions)
                {
                    if (PendingFileActions.Count > 0)
                    {
                        op = PendingFileActions.Dequeue();
                    }
                }
                if (op != null)
                {
                    // Kick off a thread.
                    var cf = new Action(op.Replay);
                    lock (InProgressActions)
                    {
                        InProgressActions.Add(cf.BeginInvoke(FinishAction, cf));
                    }
                }
            }
        }

		/// <summary>
		/// Tidies up after a file operation is complete.
		/// </summary>
		/// <param name="result">The asynchronous details of the file operation.</param>
        /// <remarks>
        /// I'd rather this be a generic method to avoid the type conditionals, but I can't figure out the root type for the delegates.
        /// It's not System.Delegate because why would it be?
        /// </remarks>
        public void FinishAction(IAsyncResult result)
        {
            lock (InProgressActions)
            {
                InProgressActions.Remove(result);
            }
            try
            {
                object finished = result.AsyncState;
                if (finished is CopyFileDelegate)
                {
                    ((CopyFileDelegate)finished).EndInvoke(result);
                }
                else if (finished is DeleteFileDelegate)
                {
                    ((DeleteFileDelegate)finished).EndInvoke(result);
                }
                else if (finished is DeleteDirectoryDelegate)
                {
                    ((DeleteDirectoryDelegate)finished).EndInvoke(result);
                }
                else if (finished is MoveFileDelegate)
                {
                    ((MoveFileDelegate)finished).EndInvoke(result);
                }
            }
            catch (Exception e)
            {
                _errors.Add(e);
            }
            BeginThreads();
        }

        public string[] ListLocalFiles(string path, params string[] SearchPatterns)
        {
            List<string> result = new List<string>();
            Queue<string> queue = new Queue<string>();
            if (SearchPatterns == null || SearchPatterns.Length == 0)
            {
                SearchPatterns = new string[] { "*.*" };
            }
            queue.Enqueue(path);
            // While the queue is not empty,
            while (queue.Count > 0)
            {
                // Dequeue a folder to process.
                string folder = queue.Dequeue();
                // Enqueue subfolders.
                foreach (string subfolder in Directory.GetDirectories(folder))
                {
                    // Ignore .SyncArchive folders. They're like BitTorrent Sync recycle bins, and don't need to be copied.
                    if (new DirectoryInfo(subfolder).Name != ".SyncArchive")
                    {
                        queue.Enqueue(subfolder);
                    }
                }
                // Add all image files to the index.
                foreach (string search in SearchPatterns)
                {
                    foreach (string file in Directory.GetFiles(folder, search))
                    {
                        // Remove the base path.
                        string trunc_file = file.Remove(0, path.Length + 1).Replace("/", "\\");
                        result.Add(trunc_file);
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Gets the size of all files in the watch path combined.
        /// </summary>
        /// <returns>A size, in bytes, representing all files combined.</returns>
        ulong IFileManager.SharedPathSize(string path)
        {
            // Calculate the actual size of the shared path.
            ulong total = 0;
            // Search for all files, not just matching ones.
            // If some other files get mixed in, it could overrun the reserve space.
            foreach (string filename in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                total += (ulong)new FileInfo(filename).Length;
            }

            return total;
        }

        /// <summary>
        /// Determines whether the given file should be copied, according to the exclusion filter rules.
        /// </summary>
        /// <param name="file">The file name to check.</param>
        /// <returns>True if the file should be copied, false otherwise.</returns>
        bool IFileManager.ShouldCopy(string filename)
        {
            return true;
        }
        /// <summary>
        /// Deletes a file or directory.
        /// </summary>
        /// <param name="dir">The full path to the file or directory to remove.</param>
        void IFileManager.Delete(string dir)
        {
            lock (PendingFileActions)
            {
                PendingFileActions.Enqueue(new DeleteFile(dir));
            }
            BeginThreads();
        }
        /// <summary>
        /// Ensures that a given path exists, creating it if necessary.
        /// </summary>
        /// <param name="targetdir">The target directory.</param>
        void IFileManager.EnsureFolder(string targetdir)
        {
            if (!Directory.Exists(targetdir))
            {
                Directory.CreateDirectory(targetdir);
            }
        }

        /// <summary>
        /// Sets normal attributes on all shared files.
        /// </summary>
        /// <remarks>
        /// I don't like this much, but I get a lot of errors when a read-only file gets into the mix.
        /// </remarks>
        void IFileManager.SetNormalAttributes(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        public FileIndex CreateIndex(string path, string[] SearchPatterns)
        {
            FileIndex index = new FileIndex {
                LocalPath=path,
                TimeStamp=DateTime.Now
            };
            IFileHashProvider hasher = new Sha1Calculator();

            foreach (string file in ListLocalFiles(path, SearchPatterns))
            {
                try
                {
                    var fileinfo = new FileInfo(Path.Combine(index.LocalPath, file));
                    var f = new FileHeader
                    {
                        ContentsHash = hasher.ComputeHash(fileinfo.FullName),
                        Size = fileinfo.Length,
                        RelativePath = file,
                        IsDeleted = false,
                    };
                    index.UpdateFile(f);
                }
                catch (Exception e)
                {
                    _errors.Add(e);
                }
            }
            return index;
        }

        /// <summary>
        /// Compares a literal file to a calculated file header record.
        /// </summary>
        /// <param name="literalFilePath">The local path of the file to compare.</param>
        /// <param name="indexFile">The file header info to compare.</param>
        /// <returns>True for a match, false otherwise, including a null record.</returns>
        /// <remarks>
        /// TODO: Delegate to the other FilesMatch method, for consistency.
        /// </remarks>
        public bool FilesMatch(string literalFilePath, FileHeader indexFile)
        {
            var fileDetails = new FileInfo(literalFilePath);
            return indexFile != null && File.Exists(literalFilePath) && indexFile.Size == fileDetails.Length && (indexFile.ContentsHash == null || indexFile.ContentsHash == ComputeHash(literalFilePath));
        }

        public bool FilesMatch(FileHeader masterFile, FileHeader indexFile)
        {
            return masterFile.Size == indexFile.Size && (masterFile.ContentsHash == null || indexFile.ContentsHash == null || masterFile.ContentsHash == indexFile.ContentsHash);
        }

        public string GetConflictFileName(string localfile, string machine, DateTime now)
        {
            var fileInfo = new FileInfo(localfile);
            var justname = fileInfo.Name.Remove(fileInfo.Name.Length - fileInfo.Extension.Length);
            var newname = Path.Combine(fileInfo.DirectoryName, string.Format("{0} ({1}'s conflicted copy {2:yyyy-MM-dd}){3}", justname, machine, now, fileInfo.Extension));
            int ver = 0;
            while (File.Exists(newname))
            {
                ver++;
                newname = Path.Combine(fileInfo.DirectoryName, string.Format("{0} ({1}'s conflicted copy {2:yyyy-MM-dd}) ({3}){4}", justname, machine, now, ver, fileInfo.Extension));
            }
            return newname;
        }

        public string ComputeHash(string literalFilePath)
        {
            return _fileHasher.ComputeHash(literalFilePath);
        }

        public FileHeader CreateFileHeader(string localPath, string relativePath)
        {
            var fileInfo = new FileInfo(Path.Combine(localPath, relativePath));
            if (File.Exists(fileInfo.FullName))
            {
                return new FileHeader
                {
                    BasePath = localPath,
                    RelativePath = relativePath,
                    ContentsHash = ComputeHash(fileInfo.FullName),
                    IsDeleted = false,
                    Size = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime
                };
            }
            else
            {
                return null;
            }
        }

        public string GetRelativePath(string absolutePath, string basePath)
        {
            if (absolutePath.StartsWith(basePath))
            {
                if (basePath.EndsWith("\\"))
                {
                    return absolutePath.Remove(0, basePath.Length);
                }
                else
                {
                    return absolutePath.Remove(0, basePath.Length + 1);
                }
            }
            else
            {
                return absolutePath;
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string[] GetDirectories(string parentFolder)
        {
            return Directory.GetDirectories(parentFolder, "*", SearchOption.AllDirectories);
        }

        public bool FileExists(string file)
        {
            return File.Exists(file);
        }

        public bool FileExists(string basepath, string relativePath)
        {
            return FileExists(Path.Combine(basepath, relativePath));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of pending copy operations.
        /// </summary>
        int IFileManager.Count
		{
			get
			{
				return InProgressActions.Count + PendingFileActions.Count;
			}
		}
		#endregion

    }
}

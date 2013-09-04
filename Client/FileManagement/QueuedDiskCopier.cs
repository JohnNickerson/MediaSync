using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Indexing;
using System.Text.RegularExpressions;

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
		#endregion

		#region Variables
		/// <summary>
		/// A list of asynchronous file copy results.
		/// </summary>
		private List<IAsyncResult> CopyActions;

		/// <summary>
		/// Sync operations waiting to go ahead.
		/// </summary>
		private Queue<SyncOperation> PendingActions;

		/// <summary>
		/// The maximum number of simultaneous copies to perform.
		/// </summary>
		private int MaxCopies;

        private List<Exception> _errors;

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

        private SyncProfile _profile;

        private IIndexService _indexer;
        private ulong _sizecache;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new asynchronous file copy service.
		/// </summary>
		public QueuedDiskCopier(SyncProfile profile, IIndexService indexer)
		{
			CopyActions = new List<IAsyncResult>();
			PendingActions = new Queue<SyncOperation>();
			MaxCopies = 2;
			_errors = new List<Exception>();

            _profile = profile;
            _indexer = indexer;
		}
		#endregion

		#region Methods
        /// <summary>
        /// Checks whether a file name matches any exclusion pattern.
        /// </summary>
        /// <param name="file">The file name to test.</param>
        /// <returns>True if any of the exclusion patterns match the given file name, false otherwise.</returns>
        bool IFileManager.Exclude(string file)
        {
            bool result = false;
            string testfile = Path.GetFileName(file);

            foreach (string x in _profile.ExcludePatterns)
            {
                Regex r = new Regex(x);
                if (r.IsMatch(testfile))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        /// <summary>
		/// Starts an asynchronous file copy operation.
		/// </summary>
		/// <param name="source">The source file to copy.</param>
		/// <param name="target">The destination where the file will be copied to.</param>
		void IFileManager.CopyFile(string source, string target)
		{
			lock (CopyActions)
			{
                if (!source.Equals(target) && !File.Exists(target))
                {
                    if (CopyActions.Count < MaxCopies)
                    {
                        CopyFileDelegate cf = new CopyFileDelegate(File.Copy);
                        CopyActions.Add(cf.BeginInvoke(source, target, FinishCopy, cf));
                    }
                    else
                    {
                        PendingActions.Enqueue(new SyncOperation(source, target, SyncOperation.SyncAction.Copy));
                    }
                }
			}
		}
		/// <summary>
		/// Tidies up after a copy operation is complete.
		/// </summary>
		/// <param name="result">The asynchronous details of the copy operation.</param>
		public void FinishCopy(IAsyncResult result)
		{
			lock (CopyActions)
			{
				CopyActions.Remove(result);
				while (CopyActions.Count < MaxCopies && PendingActions.Count > 0)
				{
					SyncOperation op = PendingActions.Dequeue();
					((IFileManager)this).CopyFile(op.SourceFile, op.TargetFile);
				}
			}
			try
			{
				CopyFileDelegate cf = (CopyFileDelegate)result.AsyncState;
				cf.EndInvoke(result);
			}
			catch (Exception e)
			{
				_errors.Add(e);
			}
		}
        void IFileManager.CreateIndex(IIndexService indexer)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(_profile.SourcePath);
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
                foreach (string file in Directory.GetFiles(folder, "*.*"))
                {
                    if (!((IFileManager)this).Exclude(file))
                    {
                        // Remove the base path.
                        string trunc_file = file.Remove(0, _profile.SourcePath.Length + 1).Replace("/", "\\");
                        _indexer.Add(trunc_file);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the size of all files in the watch path combined.
        /// </summary>
        /// <returns>A size, in bytes, representing all files combined.</returns>
        ulong IFileManager.WatchPathSize()
        {
            if (_sizecache == 0)
            {
                ulong total = 0;

                foreach (string filename in Directory.GetFiles(_profile.SourcePath, "*.*", SearchOption.AllDirectories))
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
        /// Determines whether the given file should be copied, according to the exclusion filter rules.
        /// </summary>
        /// <param name="filename">The file name to check.</param>
        /// <returns>True if the file should be copied, false otherwise.</returns>
        bool IFileManager.ShouldCopy(string filename)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Deletes a file or directory.
        /// </summary>
        /// <param name="dir">The full path to the file or directory to remove.</param>
        void IFileManager.Delete(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir);
            }
            else if (File.Exists(dir))
            {
                File.Delete(dir);
            }
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
        #endregion

		#region Properties
		/// <summary>
		/// Gets the number of pending copy operations.
		/// </summary>
		int IFileManager.Count
		{
			get
			{
				return CopyActions.Count + PendingActions.Count;
			}
		}
		#endregion
    }
}

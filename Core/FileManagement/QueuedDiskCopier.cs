using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using System.Diagnostics;

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
		private Queue<SyncOperation> PendingFileActions;

		/// <summary>
		/// The maximum number of simultaneous copies to perform.
		/// </summary>
		private int MaxActions;

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
        private ProfileParticipant _localSettings;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new asynchronous file copy service.
		/// </summary>
		public QueuedDiskCopier(SyncProfile profile, IIndexMapper indexer, string participant)
		{
			InProgressActions = new List<IAsyncResult>();
			PendingFileActions = new Queue<SyncOperation>();
			MaxActions = 2;

			_errors = new List<Exception>();

            _profile = profile;
            _localSettings = profile.GetParticipant(participant);
		}
		#endregion

		#region Methods
        /// <summary>
		/// Starts an asynchronous file copy operation.
		/// </summary>
		/// <param name="source">The source file to copy.</param>
		/// <param name="target">The destination where the file will be copied to.</param>
		void IFileManager.CopyFile(string source, string target)
        {
            if (!source.Equals(target) && !File.Exists(target))
            {
                lock (PendingFileActions)
                {
                    PendingFileActions.Enqueue(new SyncOperation(source, target, SyncOperation.SyncAction.Copy));
                }
                BeginThreads();
            }
        }

        void IFileManager.MoveFile(string source, string target)
        {
            if (!source.Equals(target) && !File.Exists(target))
            {
                lock (PendingFileActions)
                {
                    PendingFileActions.Enqueue(new SyncOperation(source, target, SyncOperation.SyncAction.Move));
                }
                BeginThreads();
            }
        }

        private void BeginThreads()
        {
            while (InProgressActions.Count < MaxActions)
            {
                // Pop a pending action off the queue.
                SyncOperation op;
                lock (PendingFileActions)
                {
                    op = PendingFileActions.Dequeue();
                }
                // Kick off a thread.
                switch (op.Action)
                {
                    case SyncOperation.SyncAction.Copy:
                        CopyFileDelegate cf = new CopyFileDelegate(File.Copy);
                        lock (InProgressActions)
                        {
                            InProgressActions.Add(cf.BeginInvoke(op.SourceFile, op.TargetFile, FinishAction, cf));
                        }
                        break;
                    case SyncOperation.SyncAction.Delete:
                        if (Directory.Exists(op.SourceFile))
                        {
                            DeleteDirectoryDelegate dd = new DeleteDirectoryDelegate(Directory.Delete);
                            lock (InProgressActions)
                            {
                                InProgressActions.Add(dd.BeginInvoke(op.SourceFile, FinishAction, dd));
                            }
                        }
                        else if (File.Exists(op.SourceFile))
                        {
                            DeleteFileDelegate df = new DeleteFileDelegate(File.Delete);
                            lock (InProgressActions)
                            {
                                InProgressActions.Add(df.BeginInvoke(op.SourceFile, FinishAction, df));
                            }
                        }
                        break;
                    case SyncOperation.SyncAction.Move:
                        MoveFileDelegate mf = new MoveFileDelegate(File.Move);
                        lock (InProgressActions)
                        {
                            InProgressActions.Add(mf.BeginInvoke(op.SourceFile, op.TargetFile, FinishAction, mf));
                        }
                        break;
                    default:
                        break;
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

        public string[] ListLocalFiles()
        {
            List<string> result = new List<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(_localSettings.LocalPath);
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
                foreach (string search in _profile.SearchPatterns)
                {
                    foreach (string file in Directory.GetFiles(folder, search))
                    {
                        // Remove the base path.
                        string trunc_file = file.Remove(0, _localSettings.LocalPath.Length + 1).Replace("/", "\\");
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
        ulong IFileManager.SharedPathSize()
        {
            // Calculate the actual size of the shared path, plus the anticipated size of files yet to be copied in.
            ulong total = 0;
            // TODO: Search for all files, not just matching ones?
            // If some other files get mixed in, it could overrun the reserve space.
            foreach (string search in _profile.SearchPatterns)
            {
                foreach (string filename in Directory.GetFiles(_localSettings.SharedPath, search, SearchOption.AllDirectories))
                {
                    total += (ulong)new FileInfo(filename).Length;
                }
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
                PendingFileActions.Enqueue(new SyncOperation(dir));
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
        void IFileManager.SetNormalAttributes()
        {
            foreach (string file in Directory.GetFiles(_localSettings.SharedPath, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
        }

        public FileIndex CreateIndex()
        {
            FileIndex index = new FileIndex();
            index.MachineName = _localSettings.MachineName;
            index.ProfileName = _profile.Name;
            index.TimeStamp = DateTime.Now;
            index.LocalBasePath = _localSettings.LocalPath;
            IFileHashProvider hasher = new MockHasher();

            foreach (string file in ListLocalFiles())
            {
                try
                {
                    index.Files.Add(new FileHeader(file, index.LocalBasePath, hasher));
                }
                catch (Exception e)
                {
                    _errors.Add(e);
                }
            }
            return index;
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

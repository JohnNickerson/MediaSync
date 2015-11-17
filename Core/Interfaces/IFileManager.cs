using System;
using System.Collections.Generic;
using AssimilationSoftware.MediaSync.Core.Model;
namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
	/// <summary>
	/// An interface for components that handle file copying.
	/// </summary>
	public interface IFileManager
	{
		#region Methods
		/// <summary>
		/// Requests a file copy.
		/// </summary>
		/// <param name="source">The file to copy.</param>
		/// <param name="target">The new location to copy to.</param>
		void CopyFile(string source, string target);

        void MoveFile(string source, string target);

        ulong SharedPathSize(string path);

        bool ShouldCopy(string filename);

        void Delete(string dir);

        void EnsureFolder(string targetdir);

        void SetNormalAttributes(string path);

        string[] ListLocalFiles(string path, string[] searchpatterns);

        FileIndex CreateIndex(string path, string[] searchpatterns);
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of file copies still pending.
		/// </summary>
		int Count { get; }

        List<Exception> Errors { get; }

        bool FilesMatch(string literalFilePath, FileHeader indexFile);
        bool FilesMatch(FileHeader masterfile, FileHeader localIndexFile);
        string GetConflictFileName(string localPath, string relativePath, string machineId, DateTime now);
        string ComputeHash(string localFile);
        FileHeader CreateFileHeader(string localPath, string relativePath);
        string GetRelativePath(string sharedfile, string sharedPath);
        #endregion
    }
}

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
        /// Copies a file from one location to another.
        /// </summary>
        /// <param name="localPath">The base path where the file exists now.</param>
        /// <param name="relativePath">The relative path to the file.</param>
        /// <param name="sharedPath">The target path where the file should be copied.</param>
        FileCommandResult CopyFile(string localPath, string relativePath, string sharedPath);

        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// <param name="source">The source location to move from.</param>
        /// <param name="target">The target location to move the file to.</param>
        /// <param name="overwrite">True to allow the method to overwrite an existing file, if present.</param>
        /// <returns>A value indicating whether the move was successful.</returns>
        FileCommandResult MoveFile(string source, string target, bool overwrite);

        ulong SharedPathSize(string path);

        FileCommandResult Delete(string file);

        void EnsureFolder(string targetdir);

        string[] ListLocalFiles(string path, params string[] searchPatterns);

        FileSystemEntry CreateFileHeader(string localPath, string relativePath);
		
		/// <summary>
		/// Attempts to create a file header record for a given local file, if present.
		/// </summary>
		/// <returns>A FileHeader instance if possible, or null if the file does not exist.</returns>
		FileSystemEntry TryCreateFileHeader(string localPath, string relativePath);
		
        string GetRelativePath(string fullPath, string basePath);
        bool DirectoryExists(string sharedPath);
        bool FileExists(string localPath, string relativePath);
        string GetConflictFileName(string localFile, string machineId, DateTime now);
        #endregion

        #region Properties

        List<Exception> Errors { get; }
        #endregion
    }

    public enum FileCommandResult
    {
        Success,
        Failure,
        Async
    }
}

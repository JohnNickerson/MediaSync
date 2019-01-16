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
        FileCommandResult CopyFile(string source, string target);

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

        bool ShouldCopy(string filename);

        FileCommandResult Delete(string file);

        void EnsureFolder(string targetdir);

        void SetNormalAttributes(string path);

        string[] ListLocalFiles(string path, params string[] searchPatterns);

        FileIndex CreateIndex(string path, params string[] searchpatterns);

        bool FilesMatch(string literalFilePath, FileHeader indexFile);
        bool FilesMatch(FileHeader masterfile, FileHeader localIndexFile);
        string ComputeHash(string localFile);
        
		FileHeader CreateFileHeader(string localPath, string relativePath);
		
		/// <summary>
		/// Attempts to create a file header record for a given local file, if present.
		/// </summary>
		/// <returns>A FileHeader instance if possible, or null if the file does not exist.</returns>
		FileHeader TryCreateFileHeader(string localPath, string relativePath);
		
        string GetRelativePath(string fullPath, string basePath);
        bool DirectoryExists(string sharedPath);
        string[] GetDirectories(string parentFolder);
        bool FileExists(string file);
        bool FileExists(string localPath, string relativePath);
        string GetConflictFileName(string localFile, string machineId, DateTime now);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of file copies still pending.
        /// </summary>
        int Count { get; }

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

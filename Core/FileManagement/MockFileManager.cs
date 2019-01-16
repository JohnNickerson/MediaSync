using System;
using System.Collections.Generic;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.FileManagement
{
    class MockFileManager : IFileManager
    {
        FileCommandResult IFileManager.CopyFile(string source, string target)
        {
            return FileCommandResult.Success;
        }

        FileCommandResult IFileManager.CopyFile(string basePath, string relativePath, string targetPath)
        {
            return FileCommandResult.Success;
        }

        FileCommandResult IFileManager.MoveFile(string source, string target, bool overwrite)
        {
            return FileCommandResult.Success;
        }

        int IFileManager.Count => 0;

        string[] IFileManager.ListLocalFiles(string path, string[] searchPatterns)
        {
            return new string[] { };
        }

        List<Exception> IFileManager.Errors => new List<Exception>();

        ulong IFileManager.SharedPathSize(string path)
        {
            return 0;
        }

        bool IFileManager.ShouldCopy(string filename)
        {
            return true;
        }


        FileCommandResult IFileManager.Delete(string dir)
        {
            return FileCommandResult.Success;
        }


        void IFileManager.EnsureFolder(string targetdir)
        {
        }


        void IFileManager.SetNormalAttributes(string path)
        {
        }


        public FileIndex CreateIndex(string path, string[] searchpatterns)
        {
            throw new NotImplementedException();
        }

        public bool FilesMatch(string literalFilePath, FileHeader indexFile)
        {
            throw new NotImplementedException();
        }

        public bool FilesMatch(FileHeader masterfile, FileHeader localIndexFile)
        {
            throw new NotImplementedException();
        }

        public string GetConflictFileName(string localFile, string machineId, DateTime now)
        {
            throw new NotImplementedException();
        }

        public string ComputeHash(string localFile)
        {
            throw new NotImplementedException();
        }

        public FileHeader CreateFileHeader(string localPath, string relativePath)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Attempts to create a file header record for a given local file, if present.
		/// </summary>
		/// <returns>A FileHeader instance if possible, or null if the file does not exist.</returns>
		public FileHeader TryCreateFileHeader(string localPath, string relativePath)
		{
			try
			{
				return CreateFileHeader(localPath, relativePath);
			}
			catch
			{
				return null;
			}
		}

        public string GetRelativePath(string sharedfile, string sharedPath)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(string sharedPath)
        {
            throw new NotImplementedException();
        }

        public string[] GetDirectories(string parentFolder)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string file)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string basepath, string relativePath)
        {
            return FileExists(Path.Combine(basepath, relativePath));
        }
    }
}

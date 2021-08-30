using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Model;
using System.IO;

namespace UnitTesting.Mocks
{
    class MockFileManager : IFileManager
    {
        private readonly List<FileHeader> _fakeFiles;

        public MockFileManager(params FileHeader[] files)
        {
            _fakeFiles = files != null ? files.ToList() : new List<FileHeader>();
        }

        public int Count => 0;

        public List<Exception> Errors => throw new NotImplementedException();

        public string ComputeHash(string localFile)
        {
            throw new NotImplementedException();
        }

        public FileCommandResult CopyFile(string source, string target)
        {
            throw new NotImplementedException();
        }

        public FileCommandResult CopyFile(string sourceBasePath, string relativePath, string targetBasePath)
        {
            return CopyFile(Path.Combine(sourceBasePath, relativePath), Path.Combine(targetBasePath, relativePath));
        }

        public FileSystemEntry CreateFileHeader(string localPath, string relativePath)
        {
            throw new NotImplementedException();
        }

        public FileSystemEntry TryCreateFileHeader(string localPath, string relativePath)
        {
            throw new NotImplementedException();
        }

        public FileCommandResult Delete(string dir)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryExists(string sharedPath)
        {
            throw new NotImplementedException();
        }

        public void EnsureFolder(string targetdir)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string file)
        {
            return _fakeFiles.Any(f => f.FileName == file);
        }

        public bool FileExists(string basepath, string relativePath)
        {
            return FileExists(Path.Combine(basepath, relativePath));
        }

        public bool FilesMatch(FileHeader primaryfile, FileHeader localIndexFile)
        {
            return primaryfile.ContentsHash == localIndexFile.ContentsHash;
        }

        public bool FilesMatch(string literalFilePath, FileHeader indexFile)
        {
            throw new NotImplementedException();
        }

        public string GetConflictFileName(string localFile, string machineId, DateTime now)
        {
            throw new NotImplementedException();
        }

        public string[] GetDirectories(string parentFolder)
        {
            throw new NotImplementedException();
        }

        public string GetRelativePath(string sharedfile, string sharedPath)
        {
            throw new NotImplementedException();
        }

        public string[] ListLocalFiles(string path, string[] searchPatterns)
        {
            throw new NotImplementedException();
        }

        public FileCommandResult MoveFile(string source, string target, bool overwrite)
        {
            throw new NotImplementedException();
        }

        public void SetNormalAttributes(string path)
        {
            throw new NotImplementedException();
        }

        public ulong SharedPathSize(string path)
        {
            throw new NotImplementedException();
        }

        public bool ShouldCopy(string filename)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core
{
    class MockFileManager : IFileManager
    {
        void IFileManager.CopyFile(string source, string target)
        {
        }

        void IFileManager.MoveFile(string source, string target)
        {
        }

        int IFileManager.Count
        {
            get
            {
                return 0;
            }
        }

        string[] IFileManager.ListLocalFiles(string path, string[] search)
        {
            return new string[] { };
        }

        List<Exception> IFileManager.Errors
        {
            get
            {
                return new List<Exception>();
            }
        }

        ulong IFileManager.SharedPathSize(string path)
        {
            return 0;
        }

        bool IFileManager.ShouldCopy(string filename)
        {
            return true;
        }


        void IFileManager.Delete(string dir)
        {
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

        public string GetConflictFileName(string localPath, string relativePath, string machineId, DateTime now)
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

        public string GetRelativePath(string sharedfile, string sharedPath)
        {
            throw new NotImplementedException();
        }
    }
}

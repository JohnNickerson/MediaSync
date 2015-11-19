using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    class MockFileManager : IFileManager
    {
        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<Exception> Errors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ComputeHash(string localFile)
        {
            throw new NotImplementedException();
        }

        public void CopyFile(string source, string target)
        {
            throw new NotImplementedException();
        }

        public FileHeader CreateFileHeader(string localPath, string relativePath)
        {
            throw new NotImplementedException();
        }

        public FileIndex CreateIndex(string path, string[] searchpatterns)
        {
            return new FileIndex
            {
                IsPull = true,
                IsPush = true,
                LocalPath = path,
                MachineName = "testmachine",
                SharedPath = ".",
                TimeStamp = DateTime.Now,
                Files = new List<FileHeader>
                {
                    new FileHeader
                    {
                        FileName = "oldversion.txt",
                        IsDeleted = false,
                        RelativePath = ".",
                        ContentsHash = "aaaa",
                        Size = 1000,
                    }
                }
            };
        }

        public void Delete(string dir)
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
            throw new NotImplementedException();
        }

        public bool FilesMatch(FileHeader masterfile, FileHeader localIndexFile)
        {
            throw new NotImplementedException();
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

        public string[] ListLocalFiles(string path, string[] searchpatterns)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string source, string target, bool overwrite)
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

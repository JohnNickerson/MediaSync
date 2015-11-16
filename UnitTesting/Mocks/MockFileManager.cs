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

        public void CopyFile(string source, string target)
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

        public void EnsureFolder(string targetdir)
        {
            throw new NotImplementedException();
        }

        public string[] ListLocalFiles(string path, string[] searchpatterns)
        {
            throw new NotImplementedException();
        }

        public void MoveFile(string source, string target)
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

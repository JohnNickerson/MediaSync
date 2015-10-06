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
    }
}

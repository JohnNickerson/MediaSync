using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Model;

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

        string[] IFileManager.ListLocalFiles()
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

        ulong IFileManager.SharedPathSize()
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


        void IFileManager.SetNormalAttributes()
        {
        }


        public FileIndex CreateIndex()
        {
            throw new NotImplementedException();
        }
    }
}

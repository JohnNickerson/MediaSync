using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core
{
    class MockFileManager : IFileManager
    {
        void IFileManager.CopyFile(string source, string target)
        {
        }

        int IFileManager.Count
        {
            get
            {
                return 0;
            }
        }

        void IFileManager.CreateIndex(Indexing.IIndexService _indexer)
        {
        }

        List<Exception> IFileManager.Errors
        {
            get
            {
                return new List<Exception>();
            }
        }

        ulong IFileManager.WatchPathSize()
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
    }
}

using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    public class MockDataContext : IDataStore
    {
        private List<SyncSet> _syncSets;

        public MockDataContext(params SyncSet[] ss)
        {
            if (ss == null)
            {
                _syncSets = new List<SyncSet>();
            }
            else
            {
                _syncSets = ss.ToList();
            }
        }

        public void CreateFileHeader(FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void CreateFileIndex(FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void CreateSyncProfile(SyncSet obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileHeader(FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileIndex(FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteSyncProfile(SyncSet obj)
        {
            throw new NotImplementedException();
        }

        public FileHeader[] GetAllFileHeader()
        {
            throw new NotImplementedException();
        }

        public FileIndex[] GetAllFileIndex()
        {
            throw new NotImplementedException();
        }

        public SyncSet[] GetAllSyncProfile()
        {
            return _syncSets.ToArray();
        }

        public FileHeader GetFileHeaderById(int id)
        {
            throw new NotImplementedException();
        }

        public FileIndex GetFileIndexById(int id)
        {
            throw new NotImplementedException();
        }

        public SyncSet GetSyncProfileById(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            // Ignore.
        }

        public void UpdateFileHeader(FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateFileIndex(FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateSyncProfile(SyncSet obj)
        {
            throw new NotImplementedException();
        }
    }
}

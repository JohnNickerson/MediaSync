using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Mock
{
    /// <summary>
    /// An in-memory data store that doesn't persist changes anywhere.
    /// </summary>
    public class MockDataStore :IDataStore
    {
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void CreateFileHeader(Model.FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void CreateFileIndex(Model.FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void CreateProfileParticipant(Model.ProfileParticipant obj)
        {
            throw new NotImplementedException();
        }

        public void CreateSyncProfile(Model.SyncProfile obj)
        {
            throw new NotImplementedException();
        }

        public Model.FileHeader GetFileHeaderById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.FileIndex GetFileIndexById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.ProfileParticipant GetProfileParticipantById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.SyncProfile GetSyncProfileById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.FileHeader[] GetAllFileHeader()
        {
            throw new NotImplementedException();
        }

        public Model.FileIndex[] GetAllFileIndex()
        {
            throw new NotImplementedException();
        }

        public Model.ProfileParticipant[] GetAllProfileParticipant()
        {
            throw new NotImplementedException();
        }

        public Model.SyncProfile[] GetAllSyncProfile()
        {
            throw new NotImplementedException();
        }

        public void UpdateFileHeader(Model.FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateFileIndex(Model.FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfileParticipant(Model.ProfileParticipant obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateSyncProfile(Model.SyncProfile obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileHeader(Model.FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteFileIndex(Model.FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteProfileParticipant(Model.ProfileParticipant obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteSyncProfile(Model.SyncProfile obj)
        {
            throw new NotImplementedException();
        }
    }
}

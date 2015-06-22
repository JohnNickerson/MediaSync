using AssimilationSoftware.MediaSync.Core.Interfaces;
using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.XML
{
    /// <summary>
    /// An XML-backed data store.
    /// </summary>
    public class XmlDataStore : IDataStore
    {
        private string _profilesLocation;
        private string _indexesLocation;
        private SharpSerializer _serialiser;


        public XmlDataStore(string profilesFilename, string indexesFilename)
        {
            _profilesLocation = profilesFilename;
            _indexesLocation = indexesFilename;
            _serialiser = new SharpSerializer();
        }

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

        public void CreateProfileParticipant(Model.Repository obj)
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

        public Model.Repository GetProfileParticipantById(int id)
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

        public Model.Repository[] GetAllProfileParticipant()
        {
            throw new NotImplementedException();
        }

        public Model.SyncProfile[] GetAllSyncProfile()
        {
            throw new NotImplementedException();
        }

        public Model.Machine[] GetAllMachines()
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

        public void UpdateProfileParticipant(Model.Repository obj)
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

        public void DeleteProfileParticipant(Model.Repository obj)
        {
            throw new NotImplementedException();
        }

        public void DeleteSyncProfile(Model.SyncProfile obj)
        {
            throw new NotImplementedException();
        }
    }
}

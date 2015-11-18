using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
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
        private SharpSerializer _serialiser;
        private List<SyncSet> _syncSets;

        public XmlDataStore(string profilesFilename)
        {
            _profilesLocation = profilesFilename;
            _serialiser = new SharpSerializer();
            if (!File.Exists(_profilesLocation))
            {
                _serialiser.Serialize(new List<SyncSet>(), _profilesLocation);
            }
            _syncSets = (List<SyncSet>)_serialiser.Deserialize(_profilesLocation);
        }

        public void SaveChanges()
        {
            _serialiser.Serialize(_syncSets, _profilesLocation);
        }

        public void CreateFileHeader(Model.FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void CreateFileIndex(Model.FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void CreateSyncProfile(Model.SyncSet obj)
        {
            _syncSets.Add(obj);
        }

        public Model.FileHeader GetFileHeaderById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.FileIndex GetFileIndexById(int id)
        {
            throw new NotImplementedException();
        }

        public Model.SyncSet GetSyncProfileById(int id)
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

        public Model.SyncSet[] GetAllSyncProfile()
        {
            return _syncSets.ToArray();
        }

        public void UpdateFileHeader(Model.FileHeader obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateFileIndex(Model.FileIndex obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateSyncProfile(Model.SyncSet obj)
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

        public void DeleteSyncProfile(Model.SyncSet obj)
        {
            throw new NotImplementedException();
        }
    }
}

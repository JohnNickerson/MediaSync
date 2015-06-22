using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    /// <summary>
    /// An interface for classes that provide unified access to model data storage repositories.
    /// </summary>
    public interface IDataStore
    {
        void SaveChanges();

        void CreateFileHeader(FileHeader obj);
        void CreateFileIndex(FileIndex obj);
        void CreateProfileParticipant(Repository obj);
        void CreateSyncProfile(SyncProfile obj);

        FileHeader GetFileHeaderById(int id);
        FileIndex GetFileIndexById(int id);
        Repository GetProfileParticipantById(int id);
        SyncProfile GetSyncProfileById(int id);

        FileHeader[] GetAllFileHeader();
        FileIndex[] GetAllFileIndex();
        Repository[] GetAllProfileParticipant();
        SyncProfile[] GetAllSyncProfile();
        Machine[] GetAllMachines();

        void UpdateFileHeader(FileHeader obj);
        void UpdateFileIndex(FileIndex obj);
        void UpdateProfileParticipant(Repository obj);
        void UpdateSyncProfile(SyncProfile obj);

        void DeleteFileHeader(FileHeader obj);
        void DeleteFileIndex(FileIndex obj);
        void DeleteProfileParticipant(Repository obj);
        void DeleteSyncProfile(SyncProfile obj);

    }
}

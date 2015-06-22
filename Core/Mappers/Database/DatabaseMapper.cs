using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    public class DatabaseMapper : IDataStore
    {
        public void SaveChanges()
        {
            DatabaseContext.Default.SaveChanges();
        }

        public void CreateFileHeader(Model.FileHeader obj)
        {
            DatabaseContext.Default.FileHeaders.Add(obj);
        }

        public void CreateFileIndex(Model.FileIndex obj)
        {
            DatabaseContext.Default.FileIndexes.Add(obj);
        }

        public void CreateProfileParticipant(Model.Repository obj)
        {
            DatabaseContext.Default.ProfileParticipants.Add(obj);
        }

        public void CreateSyncProfile(Model.SyncProfile obj)
        {
            DatabaseContext.Default.SyncProfiles.Add(obj);
        }

        public Model.FileHeader GetFileHeaderById(int id)
        {
            return DatabaseContext.Default.FileHeaders.Find(id);
        }

        public Model.FileIndex GetFileIndexById(int id)
        {
            return DatabaseContext.Default.FileIndexes.Find(id);
        }

        public Model.Repository GetProfileParticipantById(int id)
        {
            return DatabaseContext.Default.ProfileParticipants.Find(id);
        }

        public Model.SyncProfile GetSyncProfileById(int id)
        {
            return DatabaseContext.Default.SyncProfiles.Find(id);
        }

        public Model.FileHeader[] GetAllFileHeader()
        {
            return DatabaseContext.Default.FileHeaders.ToArray();
        }

        public Model.FileIndex[] GetAllFileIndex()
        {
            GetAllSyncProfile();
            return DatabaseContext.Default.FileIndexes.ToArray();
        }

        public Model.Repository[] GetAllProfileParticipant()
        {
            return DatabaseContext.Default.ProfileParticipants.ToArray();
        }

        public Model.SyncProfile[] GetAllSyncProfile()
        {
            GetAllProfileParticipant();
            return DatabaseContext.Default.SyncProfiles.ToArray();
        }

        public void UpdateFileHeader(Model.FileHeader obj)
        {
            // Automatically handled by Entity Framework.
        }

        public void UpdateFileIndex(Model.FileIndex obj)
        {
            // Automatically handled by Entity Framework.
        }

        public void UpdateProfileParticipant(Model.Repository obj)
        {
            // Automatically handled by Entity Framework.
        }

        public void UpdateSyncProfile(Model.SyncProfile obj)
        {
            // Automatically handled by Entity Framework.
        }

        public void DeleteFileHeader(Model.FileHeader obj)
        {
            DatabaseContext.Default.FileHeaders.Remove(obj);
        }

        public void DeleteFileIndex(Model.FileIndex obj)
        {
            DatabaseContext.Default.FileIndexes.Remove(obj);
        }

        public void DeleteProfileParticipant(Model.Repository obj)
        {
            DatabaseContext.Default.ProfileParticipants.Remove(obj);
        }

        public void DeleteSyncProfile(Model.SyncProfile obj)
        {
            DatabaseContext.Default.SyncProfiles.Remove(obj);
        }

        public Model.Machine[] GetAllMachines()
        {
            DatabaseContext.Default.Machines.ToArray();
        }
    }
}

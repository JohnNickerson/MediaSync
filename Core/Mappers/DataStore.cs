using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssimilationSoftware.Maroon.Repositories;
using AssimilationSoftware.MediaSync.Core.Mappers.CSV;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.Mappers
{
    public class DataStore
    {
        private readonly string _path;
        private SingleOriginRepository<FileSystemEntry> Files;
        private SingleOriginRepository<FileIndex> Indexes;
        private SingleOriginRepository<Library> Libraries;
        private SingleOriginRepository<Machine> Machines;
        private SingleOriginRepository<Replica> Replicas;

        public DataStore(string path)
        {
            _path = path;
            Files = new SingleOriginRepository<FileSystemEntry>(new CsvFileSystemEntryMapper(), Path.Combine(_path, "Files.csv"));
            Indexes = new SingleOriginRepository<FileIndex>(new CsvFileIndexMapper(), Path.Combine(_path, "Indexes.csv"));
            Libraries = new SingleOriginRepository<Library>(new CsvLibraryMapper(), Path.Combine(_path, "Libraries.csv"));
            Machines = new SingleOriginRepository<Machine>(new CsvMachineMapper(), Path.Combine(_path, "Machines.csv"));
            Replicas = new SingleOriginRepository<Replica>(new CsvReplicaMapper(), Path.Combine(_path, "Replicas.csv"));
            LoadAll();
        }

        private void LoadAll()
        {
            Files.FindAll();
            Indexes.FindAll();
            Libraries.FindAll();
            Machines.FindAll();
            Replicas.FindAll();
        }

        public FileSystemEntry GetFileSystemEntryById(Guid id)
        {
            return Files.Find(id);
        }

        public FileIndex GetFileIndexById(Guid id)
        {
            return Indexes.Find(id);
        }

        public Library GetLibraryById(Guid id)
        {
            return Libraries.Find(id);
        }

        public Machine GetMachineById(Guid id)
        {
            return Machines.Find(id);
        }

        public Replica GetReplicaById(Guid? id)
        {
            return id.HasValue ? Replicas.Find(id.Value) : null;
        }

        public virtual IEnumerable<FileSystemEntry> ListFileSystemEntries()
        {
            return Files.Items;
        }

        public virtual IEnumerable<FileIndex> ListIndexes()
        {
            return Indexes.Items;
        }

        public virtual IEnumerable<Library> ListLibraries()
        {
            return Libraries.Items;
        }

        public virtual IEnumerable<Machine> ListMachines()
        {
            return Machines.Items;
        }

        public virtual IEnumerable<Replica> ListReplicas()
        {
            return Replicas.Items;
        }

        public virtual IEnumerable<FileSystemEntry> ListFileSystemEntries(System.Linq.Expressions.Expression<Func<FileSystemEntry, bool>> predicate)
        {
            return Files.Items.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual FileSystemEntry GetFileByPath(Guid indexId, string relativePath)
        {
            return Files.Items
                .FirstOrDefault(f => f.IndexId == indexId &&
                                     f.RelativePath.Equals(relativePath, StringComparison.CurrentCultureIgnoreCase));
        }

        public virtual IEnumerable<FileIndex> ListIndexes(System.Linq.Expressions.Expression<Func<FileIndex, bool>> predicate)
        {
            return Indexes.Items.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Library> ListLibraries(System.Linq.Expressions.Expression<Func<Library, bool>> predicate)
        {
            return Libraries.Items.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Machine> ListMachines(System.Linq.Expressions.Expression<Func<Machine, bool>> predicate)
        {
            return Machines.Items.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Replica> ListReplicas(System.Linq.Expressions.Expression<Func<Replica, bool>> predicate)
        {
            return Replicas.Items.AsQueryable().Where(predicate).AsEnumerable();
        }

        public void SaveChanges()
        {
            PurgeOrphanedData();
            Files.SaveChanges();
            Indexes.SaveChanges();
            Libraries.SaveChanges();
            Machines.SaveChanges();
            Replicas.SaveChanges();
        }

        public void Insert(FileSystemEntry entity)
        {
            Files.Create(entity);
        }

        public void Insert(FileIndex entity)
        {
            Indexes.Create(entity);
        }

        public void Insert(Library entity)
        {
            Libraries.Create(entity);
        }

        public void Insert(Machine entity)
        {
            Machines.Create(entity);
        }

        public void Insert(Replica entity)
        {
            Replicas.Create(entity);
        }

        public void Update(FileSystemEntry entity)
        {
            // "Update" for file system entries may be used for creation as well.
            if (Files.Find(entity.ID) != null)
            {
                Files.Update(entity);
            }
            else
            {
                Files.Create(entity);
            }
        }

        public void Update(FileIndex entity)
        {
            Indexes.Update(entity);
        }

        public void Update(Library entity)
        {
            Libraries.Update(entity);
        }

        public void Update(Machine entity)
        {
            Machines.Update(entity);
        }

        public void Update(Replica entity)
        {
            Replicas.Update(entity);
        }

        public void Delete(FileSystemEntry entity)
        {
            Files.Delete(entity);
        }

        public void Delete(FileIndex entity)
        {
            Indexes.Delete(entity);
        }

        public void Delete(Library entity)
        {
            Libraries.Delete(entity);
        }

        public void Delete(Machine entity)
        {
            Machines.Delete(entity);
        }

        public void Delete(Replica entity)
        {
            Replicas.Delete(entity);
        }

        public Machine GetMachineByName(string name)
        {
            return Machines.Items.FirstOrDefault(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public void PurgeOrphanedData()
        {
            // Replicas without a Machine
            foreach (var r in Replicas.Items)
            {
                if (GetMachineById(r.MachineId) == null || GetLibraryById(r.LibraryId) == null)
                {
                    Delete(r);
                }
            }

            // Indexes without a replica or library
            foreach (var i in Indexes.Items)
            {
                var replica = GetReplicaById(i.ReplicaId);
                if (GetLibraryById(i.LibraryId) == null || i.ReplicaId.HasValue && replica == null)
                {
                    Delete(i);
                }
            }

            foreach (var f in Files.Items)
            {
                // Files without an index.
                var index = GetFileIndexById(f.IndexId);
                if (index == null || index.IsDeleted)
                {
                    Delete(f);
                }
                // TODO: Expired library files with no replicas.
            }
        }

        public void CopyFileSystemEntry(FileSystemEntry fLocalFileHeader, Guid primaryIndexId)
        {
            // Add a new copy of the given file system entry to the given primary index.
            FileSystemEntry theNewOne = null;
            if (fLocalFileHeader is FileHeader file)
            {
                theNewOne = new FileHeader
                {
                    ID = Guid.NewGuid(),
                    IndexId = primaryIndexId,
                    BasePath = file.BasePath,
                    ContentsHash = fLocalFileHeader.ContentsHash,
                    ImportHash = file.ImportHash,
                    IsDeleted = file.IsDeleted,
                    IsFolder = file.IsFolder,
                    LastModified = DateTime.Now,
                    LastWriteTime = file.LastWriteTime,
                    PrevRevision = null,
                    RelativePath = file.RelativePath,
                    RevisionGuid = Guid.NewGuid(),
                    Size = file.Size,
                    State = file.State
                };
            }
            else if (fLocalFileHeader is FolderHeader folder)
            {
                theNewOne = new FolderHeader
                {
                    ContentsHash = folder.ContentsHash,
                    ID = Guid.NewGuid(),
                    IndexId = primaryIndexId,
                    IsDeleted = folder.IsDeleted,
                    ImportHash = folder.ImportHash,
                    RelativePath = folder.RelativePath,
                    State = folder.State,
                    LastModified = DateTime.Now,
                    PrevRevision = null,
                    RevisionGuid = Guid.NewGuid()
                };
            }
            if (theNewOne != null)
                Insert(theNewOne);
        }

        public IEnumerable<FileIndex> GetIndexesByReplica(Replica replica)
        {
            return ListIndexes(i => i.ReplicaId == replica.ID);
        }

        public IEnumerable<FileSystemEntry> GetFilesByIndex(FileIndex index)
        {
            return ListFileSystemEntries(f => f.IndexId == index.ID);
        }

        public IEnumerable<FileIndex> GetIndexesByLibraryId(Guid libraryId, bool includePrimary = false)
        {
            return includePrimary ? ListIndexes(i => !i.IsDeleted && i.LibraryId == libraryId) : ListIndexes(i => !i.IsDeleted && i.LibraryId == libraryId && i.ReplicaId != null);
        }
    }
}

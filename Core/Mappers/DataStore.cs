using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using Polenter.Serialization;

namespace AssimilationSoftware.MediaSync.Core.Mappers
{
    public class DataStore
    {
        private readonly string _path;
        private Dictionary<int, IFileSystemEntry> Files;
        private Dictionary<int, FileIndex> Indexes;
        private Dictionary<int, Library> Libraries;
        private Dictionary<int, Machine> Machines;
        private Dictionary<int, Replica> Replicas;
        private SharpSerializer _mapper;

        public DataStore(string path)
        {
            _path = path;
            _mapper = new SharpSerializer();
            LoadAll();
        }

        private void LoadAll()
        {
            Files = (Dictionary<int, IFileSystemEntry>) _mapper.Deserialize(Path.Combine(_path, "Files.xml"));
            Indexes = (Dictionary<int, FileIndex>)_mapper.Deserialize(Path.Combine(_path, "Indexes.xml"));
            Libraries = (Dictionary<int, Library>)_mapper.Deserialize(Path.Combine(_path, "Libraries.xml"));
            Machines = (Dictionary<int, Machine>)_mapper.Deserialize(Path.Combine(_path, "Machines.xml"));
            Replicas = (Dictionary<int, Replica>)_mapper.Deserialize(Path.Combine(_path, "Replicas.xml"));
        }

        public IFileSystemEntry GetFileSystemEntryById(int id)
        {
            return Files.TryGetValue(id, out var result) ? result : null;
        }

        public FileIndex GetFileIndexById(int id)
        {
            return Indexes.TryGetValue(id, out var result) ? result : null;
        }

        public Library GetLibraryById(int id)
        {
            return Libraries.TryGetValue(id, out var result) ? result : null;
        }

        public Machine GetMachineById(int id)
        {
            return Machines.TryGetValue(id, out var result) ? result : null;
        }

        public Replica GetReplicaById(int id)
        {
            return Replicas.TryGetValue(id, out var result) ? result : null;
        }

        public virtual IEnumerable<IFileSystemEntry> ListFileSystemEntries()
        {
            return Files.Values.AsEnumerable();
        }

        public virtual IEnumerable<FileIndex> ListIndexes()
        {
            return Indexes.Values.AsEnumerable();
        }

        public virtual IEnumerable<Library> ListLibraries()
        {
            return Libraries.Values.AsEnumerable();
        }

        public virtual IEnumerable<Machine> ListMachines()
        {
            return Machines.Values.AsEnumerable();
        }

        public virtual IEnumerable<Replica> ListReplicas()
        {
            return Replicas.Values.AsEnumerable();
        }

        public virtual IEnumerable<IFileSystemEntry> ListFileSystemEntries(System.Linq.Expressions.Expression<Func<IFileSystemEntry, bool>> predicate)
        {
            return Files.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<FileIndex> ListIndexes(System.Linq.Expressions.Expression<Func<FileIndex, bool>> predicate)
        {
            return Indexes.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Library> ListLibraries(System.Linq.Expressions.Expression<Func<Library, bool>> predicate)
        {
            return Libraries.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Machine> ListMachines(System.Linq.Expressions.Expression<Func<Machine, bool>> predicate)
        {
            return Machines.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<Replica> ListReplicas(System.Linq.Expressions.Expression<Func<Replica, bool>> predicate)
        {
            return Replicas.Values.AsQueryable().Where(predicate).AsEnumerable();
        }

        public void SaveChanges()
        {
            _mapper.Serialize(Files, Path.Combine(_path, "Files.xml"));
            _mapper.Serialize(Indexes, Path.Combine(_path, "Indexes.xml"));
            _mapper.Serialize(Libraries, Path.Combine(_path, "Libraries.xml"));
            _mapper.Serialize(Machines, Path.Combine(_path, "Machines.xml"));
            _mapper.Serialize(Replicas, Path.Combine(_path, "Replicas.xml"));
        }

        public void Insert(IFileSystemEntry entity)
        {
            Files[entity.Id] = (entity);
        }

        public void Insert(FileIndex entity)
        {
            Indexes[entity.Id] = (entity);
        }

        public void Insert(Library entity)
        {
            Libraries[entity.Id] = (entity);
        }

        public void Insert(Machine entity)
        {
            Machines[entity.Id] = (entity);
        }

        public void Insert(Replica entity)
        {
            Replicas[entity.Id] = (entity);
        }

        public void Update(IFileSystemEntry entity)
        {
            Files[entity.Id] = entity;
        }

        public void Update(FileIndex entity)
        {
            Indexes[entity.Id] = entity;
        }

        public void Update(Library entity)
        {
            Libraries[entity.Id] = entity;
        }

        public void Update(Machine entity)
        {
            Machines[entity.Id] = entity;
        }

        public void Update(Replica entity)
        {
            Replicas[entity.Id] = entity;
        }

        public void Delete(IFileSystemEntry entity)
        {
            Files.Remove(entity.Id);
        }

        public void Delete(FileIndex entity)
        {
            Indexes.Remove(entity.Id);
        }

        public void Delete(Library entity)
        {
            Libraries.Remove(entity.Id);
        }

        public void Delete(Machine entity)
        {
            Machines.Remove(entity.Id);
        }

        public void Delete(Replica entity)
        {
            Replicas.Remove(entity.Id);
        }
    }
}

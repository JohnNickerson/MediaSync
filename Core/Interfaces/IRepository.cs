using System;
using System.Collections.Generic;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IRepository<T>
    {
        IEnumerable<T> Items { get; }

        T Find(string name);

        IEnumerable<T> FindAll();

        void Create(T entity);

        void Delete(T entity);

        void Update(T entity);

        void SaveChanges();
    }
}

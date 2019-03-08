using System;
using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.Mappers
{
    public class SyncSetRepository : IRepository<SyncSet>
    {
        private readonly ISyncSetMapper _mapper;
        private List<SyncSet> _updated;
        private List<SyncSet> _deleted;
        private List<SyncSet> _items;

        public SyncSetRepository(ISyncSetMapper mapper)
        {
            _mapper = mapper;
            _items = new List<SyncSet>();
            _updated = new List<SyncSet>();
            _deleted = new List<SyncSet>();
        }

        public IEnumerable<SyncSet> Items => _updated.Union(_items).Except(_deleted);

        public void Create(SyncSet entity)
        {
            _updated.RemoveAll(t => t.Name == entity.Name);
            _updated.Add(entity);
        }

        public void Delete(SyncSet entity)
        {
            _deleted.RemoveAll(t => t.Name == entity.Name);
            _deleted.Add(entity);
        }

        public SyncSet Find(string name)
        {
            if (Items.Any(v => string.Equals(v.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return Items.First(t => string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
            }
            var i = _mapper.Read(name);
            if (i != null)
            {
                _items.Add(i);
            }
            return i;
        }

        public IEnumerable<SyncSet> FindAll()
        {
            _items = _mapper.ReadAll();
            return Items;
        }

        public void SaveChanges()
        {
			// Only write changes if there are any to write.
			if (_updated.Count > 0 || _deleted.Count > 0)
			{
				// Load all.
				FindAll();
				// Apply changes.
				// Save all.
				_mapper.UpdateAll(Items.ToList());
                // Clear the lists.
                _items = Items.ToList();
				_updated = new List<SyncSet>();
				_deleted = new List<SyncSet>();
			}
        }

        public void Update(SyncSet entity)
        {
            _updated.RemoveAll(t => t.Name == entity.Name);
            _updated.Add(entity);
        }
    }
}

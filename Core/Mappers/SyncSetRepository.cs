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
        private Dictionary<string, SyncSet> _updated;
        private Dictionary<string, SyncSet> _deleted;
        private List<SyncSet> _items;

        public SyncSetRepository(ISyncSetMapper mapper)
        {
            _mapper = mapper;
            _items = new List<SyncSet>();
            _updated = new Dictionary<string, SyncSet>(StringComparer.CurrentCultureIgnoreCase);
            _deleted = new Dictionary<string, SyncSet>(StringComparer.CurrentCultureIgnoreCase);
        }

        public IEnumerable<SyncSet> Items
        {
            get
            {
                foreach (var k in _updated.Keys.Union(_items.Select(i => i.Name)).Except(_deleted.Keys))
                {
                    if (_updated.ContainsKey(k))
                    {
                        yield return _updated[k];
                    }
                    else
                    {
                        yield return _items.FirstOrDefault(i => string.Equals(i.Name, k, StringComparison.CurrentCultureIgnoreCase));
                    }
                }
            }
        }

        public void Create(SyncSet entity)
        {
            _updated[entity.Name] = entity;
        }

        public void Delete(SyncSet entity)
        {
            _deleted[entity.Name] = entity;
        }

        public SyncSet Find(string name)
        {
            SyncSet result;
            if (_deleted.ContainsKey(name)) result = null;
            else if (_updated.ContainsKey(name)) result = _updated[name];
            else
                result = _items.FirstOrDefault(t =>
                    string.Equals(t.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (result != null) return result;

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
				_updated.Clear();
				_deleted.Clear();
			}
        }

        public void Update(SyncSet entity)
        {
            _updated[entity.Name] = entity;
        }
    }
}

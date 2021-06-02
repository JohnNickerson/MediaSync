using System;
using System.Collections.Generic;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IMapper<T>
    {
        /// <summary>
        /// Load all items in the collection.
        /// </summary>
        /// <returns>An enumerable list of every item in the collection.</returns>
        IEnumerable<T> LoadAll();

        /// <summary>
        /// Save a specific item, whether new or updated.
        /// </summary>
        /// <param name="item">The item to save.</param>
        void Save(T item);

        /// <summary>
        /// Save the collection as specified by a list.
        /// </summary>
        /// <param name="libraries">The list of items to save.</param>
        void SaveAll(IEnumerable<T> libraries);

        /// <summary>
        /// Delete one particular item.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        void Delete(T item);
    }
}

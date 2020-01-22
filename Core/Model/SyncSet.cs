using System;
using System.Collections.Generic;
using System.Linq;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class SyncSet
    {
        private FileIndex _masterIndex;

        #region Properties
        public int Id { get; set; }

        /// <summary>
        /// Bytes reserved for this profile on shared storage.
        /// </summary>
        public ulong ReserveSpace { get; set; }
        
        /// <summary>
        /// The name of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All the participants in this profile.
        /// </summary>
        public List<FileIndex> Indexes { get; set; }

        /// <summary>
        /// The master index, containing info on the latest versions of files from all participants.
        /// </summary>
        public FileIndex MasterIndex
        {
            get => _masterIndex ?? (_masterIndex = new FileIndex());
            set => _masterIndex = value;
        }

        #endregion

		#region Constructors

        #endregion

        #region Methods
        public FileIndex GetIndex(string machine)
        {
            var result = Indexes.FirstOrDefault(p => string.Equals(p.MachineName, machine, StringComparison.CurrentCultureIgnoreCase));
            return result;
        }

        public bool ContainsParticipant(string machine)
        {
            return Indexes.Any(p => String.Equals(p.MachineName, machine, StringComparison.CurrentCultureIgnoreCase));
        }

        internal void UpdateIndex(FileIndex localIndex)
        {
            Indexes.Remove(GetIndex(localIndex.MachineName));
            Indexes.Add(localIndex);
        }

        #endregion
    }
}

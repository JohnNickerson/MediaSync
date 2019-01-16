using System;
using System.Collections.Generic;
using System.Linq;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class SyncSet
    {
        #region Properties
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
        public FileIndex MasterIndex { get; set; }

        /// <summary>
        /// File name patterns to exclude from the profile.
        /// </summary>
        public List<string> IgnorePatterns { get; set; }
        #endregion

		#region Constructors

        #endregion

        #region Methods
        public FileIndex GetIndex(string machine)
        {
            var localsettings = Indexes.Where(p => p.MachineName.ToLower() == machine.ToLower());
            if (localsettings.Any())
            {
                return localsettings.First();
            }
            else
            {
                throw new Exception(string.Format("Participant not found: {0}", machine));
            }
        }

        public bool ContainsParticipant(string machine)
        {
            return Indexes.Any(p => p.MachineName.ToLower() == machine.ToLower());
        }

        internal void UpdateIndex(FileIndex localIndex)
        {
            Indexes.Remove(GetIndex(localIndex.MachineName));
            Indexes.Add(localIndex);
        }
        #endregion
    }
}

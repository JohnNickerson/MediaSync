using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.Data;
using AssimilationSoftware.MediaSync.Core.Properties;

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
		public SyncSet()
		{
            Indexes = new List<FileIndex>();
            MasterIndex = new FileIndex();
            IgnorePatterns = new List<string>();
		}
		#endregion

        #region Methods
        public FileIndex GetIndex(string machine)
        {
            var localsettings = Indexes.Where(p => p.MachineName.ToLower() == machine.ToLower());
            if (localsettings.Count() > 0)
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
            return Indexes.Where(p => p.MachineName.ToLower() == machine.ToLower()).Count() > 0;
        }
        #endregion
    }
}

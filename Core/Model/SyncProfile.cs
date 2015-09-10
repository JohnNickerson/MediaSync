using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using AssimilationSoftware.MediaSync.Core.Properties;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class SyncSet
    {
        #region Properties
        /// <summary>
        /// A unique ID for the profile.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bytes reserved for this profile on shared storage.
        /// </summary>
        public ulong ReserveSpace { get; set; }

        /// <summary>
        /// A search pattern for files to include.
        /// </summary>
        public List<string> SearchPatterns { get; set; }
        
        /// <summary>
        /// The name of the profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All the participants in this profile.
        /// </summary>
        public List<FileIndex> Participants { get; set; }

        public FileIndex MasterIndex { get; set; }

        public List<string> IgnorePatterns { get; set; }
        #endregion

		#region Constructors
		public SyncSet()
		{
            SearchPatterns = new List<string>();
            Participants = new List<FileIndex>();
		}
		#endregion

        #region Methods
        public FileIndex GetParticipant(string machine)
        {
            var localsettings = Participants.Where(p => p.MachineName.ToLower() == machine.ToLower());
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
            return Participants.Where(p => p.MachineName.ToLower() == machine.ToLower()).Count() > 0;
        }
        #endregion

        // TODO: Move this. I don't think it belongs here.
        public static void SetLocalMachineName(string name)
        {
            Settings.Default.MachineName = name;
            Settings.Default.Save();
        }
    }
}

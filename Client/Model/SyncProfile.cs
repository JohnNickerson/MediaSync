using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;

namespace AssimilationSoftware.MediaSync.Model
{
    public class SyncProfile
    {
        #region Properties
        public ulong ReserveSpace { get; set; }

        /// <summary>
        /// A search pattern for files to include.
        /// </summary>
        public List<string> SearchPatterns { get; set; }
        public string ProfileName { get; set; }

        public List<ProfileParticipant> Participants { get; set; }
        #endregion

		#region Constructors
		public SyncProfile()
		{
            SearchPatterns = new List<string>();
            Participants = new List<ProfileParticipant>();
		}
		#endregion

        internal ProfileParticipant GetParticipant(string machine)
        {
            var localsettings = from p in Participants where p.MachineName == machine select p;
            if (localsettings.Count() > 0)
            {
                return localsettings.First();
            }
            else
            {
                throw new Exception(string.Format("Participant not found: {0}", machine));
            }
        }

        internal bool ContainsParticipant(string machine)
        {
            return (from p in Participants where p.MachineName == machine select p).Count() > 0;
        }
    }
}

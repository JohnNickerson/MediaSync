using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Profile
{
    public interface IProfileManager
    {
        #region Methods
        [Obsolete]
        void Save(string machineName, SyncProfile saveobject);
        
        [Obsolete]
        SyncProfile Load(string machineName, string profile);
        
        [Obsolete]
        SyncProfile[] Load(string machineName);

        /// <summary>
        /// Load all profiles.
        /// </summary>
        /// <returns>A list of profiles.</returns>
        List<SyncProfile> Load();

        /// <summary>
        /// Saves a list of profiles to a configured location.
        /// </summary>
        /// <param name="profiles">The list of profiles to save.</param>
        void Save(List<SyncProfile> profiles);
        #endregion
    }
}

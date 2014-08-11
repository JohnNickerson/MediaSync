﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;

namespace AssimilationSoftware.MediaSync.Interfaces
{
    public interface IProfileMapper
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

        /// <summary>
        /// Saves an individual profile.
        /// </summary>
        /// <param name="profile">The profile to save.</param>
        void Save(SyncProfile profile);

        /// <summary>
        /// Loads a single sync profile by ID.
        /// </summary>
        /// <param name="id">The ID of the profile to load.</param>
        /// <returns>The profile with the given ID, if any.</returns>
        SyncProfile Load(int id);
        #endregion
    }
}

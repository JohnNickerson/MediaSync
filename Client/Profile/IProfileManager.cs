using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Profile
{
    public interface IProfileManager
    {
        #region Methods
        void Save(string machineName, SyncProfile saveobject);
        SyncProfile Load(string machineName, string profile);
        SyncProfile[] Load(string machineName);
        #endregion
    }
}

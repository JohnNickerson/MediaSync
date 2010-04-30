using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PatientSync
{
    [Serializable]
    public class SyncOptions
    {
        #region Fields
        public string SourcePath;
        public string SharedPath;
        public bool Simulate;
        public ulong ReserveSpace;
        #endregion
    }
}

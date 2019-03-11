using AssimilationSoftware.MediaSync.Core.Model;
using System.Collections.Generic;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface ISyncSetMapper
    {
        SyncSet Read(string name);

        List<SyncSet> ReadAll();

        void UpdateAll(List<SyncSet> syncSets);
    }
}

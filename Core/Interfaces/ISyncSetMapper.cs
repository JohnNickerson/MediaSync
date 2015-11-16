using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface ISyncSetMapper
    {
        SyncSet Read(string name);

        void Update(SyncSet syncset);

        void Delete(SyncSet syncset);

        void Delete(string name);

        List<SyncSet> ReadAll();

        void UpdateAll(List<SyncSet> syncsets);
    }
}

using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    [Obsolete("To be removed in version 1.2")]
    public interface IParticipantMapper
    {
        void Save(ProfileParticipant p);
    }
}

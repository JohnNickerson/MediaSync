using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Interfaces
{
    public interface IParticipantMapper
    {
        void Save(ProfileParticipant p);
    }
}

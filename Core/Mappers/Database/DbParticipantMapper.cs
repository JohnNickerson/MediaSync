using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    public class DbParticipantMapper : IParticipantMapper
    {
        public void Save(ProfileParticipant p)
        {
            DatabaseContext.Default.ProfileParticipants.Add(p);
            DatabaseContext.Default.SaveChanges();
        }
    }
}

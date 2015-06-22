using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Database
{
    [Obsolete("To be removed in version 1.2")]
    public class DbParticipantMapper : IParticipantMapper
    {
        public void Save(Repository p)
        {
            DatabaseContext.Default.ProfileParticipants.Add(p);
            DatabaseContext.Default.SaveChanges();
        }
    }
}

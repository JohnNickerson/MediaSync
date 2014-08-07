using AssimilationSoftware.MediaSync.Core.Mappers.Database;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncServiceDbMapperTests
    {
        [Fact]
        public void Test_Create()
        {
            IProfileMapper mapper = new DbSyncProfileMapper();

            SyncProfile p = new SyncProfile
            {
                Id = 1,
                Name = "testprofile",
                ReserveSpace = 500,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<ProfileParticipant>()
            };

            mapper.Save(p);
        }

        [Fact]
        public void Test_Read()
        {
            IProfileMapper mapper = new DbSyncProfileMapper();

            mapper.Load();
        }
    }
}

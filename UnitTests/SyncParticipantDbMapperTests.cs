using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncParticipantDbMapperTests
    {
        [Fact]
        public void Test_Create()
        {
            AssimilationSoftware.MediaSync.Core.Interfaces.IParticipantMapper mapper = new AssimilationSoftware.MediaSync.Core.Mappers.Database.DbParticipantMapper();

            var p = new MediaSync.Core.Model.ProfileParticipant
            {
                Id = 1,
                MachineName = Environment.MachineName,
                Consumer = true,
                Contributor = true,
                LocalPath = @"C:\temp\MediaSync\Local",
                SharedPath = @"C:\temp\MediaSync\Shared"
            };

            mapper.Save(p);
        }

        [Fact]
        public void Test_Read()
        {
        }

        [Fact]
        public void Test_Update()
        {
        }

        [Fact]
        public void Test_Delete()
        {
        }
    }
}

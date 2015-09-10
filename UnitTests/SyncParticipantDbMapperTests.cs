using AssimilationSoftware.MediaSync.Core.Mappers.Mock;
using System;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncParticipantDbMapperTests
    {
        [Fact]
        public void Test_Create()
        {
            AssimilationSoftware.MediaSync.Core.Interfaces.IDataStore mapper = new MockDataStore();

            var p = new MediaSync.Core.Model.Repository
            {
                Id = 1,
                MachineName = Environment.MachineName,
                Consumer = true,
                Contributor = true,
                LocalPath = @"C:\temp\MediaSync\Local",
                SharedPath = @"C:\temp\MediaSync\Shared"
            };

            mapper.CreateProfileParticipant(p);
            mapper.SaveChanges();
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

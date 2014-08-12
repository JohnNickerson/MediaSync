using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncServiceTests
    {
        [Fact]
        public void Test_SyncService_Constructor()
        {
            var testprofile = new SyncProfile
                {
                    Name = "testprofile",
                    ReserveSpace = 5000,
                    SearchPatterns = new List<string>(new string[] { "*.*" }),
                    Participants = new List<ProfileParticipant>(new ProfileParticipant[]{
                        new ProfileParticipant{
                            Consumer=true,
                            Contributor=true,
                            LocalPath=@"C:\temp\local",
                            SharedPath=@"C:\temp\shared",
                            MachineName=Environment.MachineName
                         }})
                };
            var mockindexer = new AssimilationSoftware.MediaSync.Core.Mappers.Mock.MockIndexMapper();
            var s = new SyncService(testprofile, mockindexer,
                         new QueuedDiskCopier(testprofile, mockindexer, Environment.MachineName),
                         true, Environment.MachineName);
        }
    }
}

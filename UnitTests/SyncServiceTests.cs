using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Mappers.Mock;
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
            var testprofile = new SyncSet
                {
                    Name = "testprofile",
                    ReserveSpace = 5000,
                    SearchPatterns = new List<string>(new string[] { "*.*" }),
                    Participants = new List<FileIndex>(new FileIndex[]{
                        new FileIndex{
                            IsPull=true,
                            IsPush=true,
                            LocalPath=@"C:\temp\local",
                            SharedPath=@"C:\temp\shared",
                            MachineName=Environment.MachineName
                         }})
                };
            var mockindexer = new MockDataStore();
            var s = new ViewModel(testprofile, mockindexer,
                         new QueuedDiskCopier(testprofile, Environment.MachineName),
                         true, Environment.MachineName);
        }
    }
}

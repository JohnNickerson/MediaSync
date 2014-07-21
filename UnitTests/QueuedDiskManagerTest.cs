using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core;
using Xunit;
using AssimilationSoftware.MediaSync.Model;

namespace UnitTests
{
    public class QueuedDiskManagerTest
    {
        [Fact]
        public void CreateIndex()
        {
            var mockprofile = new SyncProfile
            {
                ProfileName = "mockprofile",
                ReserveSpace = 5000,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<ProfileParticipant>(new ProfileParticipant[] {
                    new ProfileParticipant{
                        Consumer=true, 
                        Contributor=true, 
                        LocalPath=@"C:\temp", 
                        MachineName=Environment.MachineName, 
                        SharedPath=@"C:\temp\share"}
                })
            };
            var mockindexer = new Mocks.ConsoleIndexer();
            QueuedDiskCopier q = new QueuedDiskCopier(mockprofile, mockindexer, "frank");
            var f = q.CreateIndex();

            Assert.Equal(5, f.Files.Count);
        }
    }
}

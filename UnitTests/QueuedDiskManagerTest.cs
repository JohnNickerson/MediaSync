﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core;
using Xunit;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Mappers.Mock;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class QueuedDiskManagerTest
    {
        [Fact]
        public void CreateIndex()
        {
            var mockprofile = new SyncProfile
            {
                Name = "mockprofile",
                ReserveSpace = 5000,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<Repository>(new Repository[] {
                    new Repository{
                        Consumer=true, 
                        Contributor=true, 
                        LocalPath=@"C:\temp", 
                        MachineName="frank", 
                        SharedPath=@"C:\temp\share"}
                })
            };
            var mockindexer = new MockDataStore();
            QueuedDiskCopier q = new QueuedDiskCopier(mockprofile, mockindexer, "frank");
            var f = q.CreateIndex();

            // Can't really compare contents to anything without generating another index. Just make sure there are no exceptions.
        }
    }
}

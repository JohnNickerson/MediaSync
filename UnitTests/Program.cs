using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Mappers.Xml;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Core;

namespace UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // For different types of profiles,
            // Test the sync operations with mock indexing and mock file moving.

            // Test the text indexer.
            // Test the database indexer.
            
            // Test the file manager.
            var o = new SyncProfile { ProfileName = "TestProfile", SearchPatterns = new List<string>(new string[] { "*.*" }) };
            SyncProfile.SetLocalMachineName("UnitTests");
            o.Participants.Add(new ProfileParticipant { MachineName = "UnitTests", LocalPath = @"C:\Temp", SharedPath = @"D:\Temp" });
            var x = new XmlIndexMapper(o);
            var q = new QueuedDiskCopier(o, x);
            x.Save(q.CreateIndex());
        }
    }
}

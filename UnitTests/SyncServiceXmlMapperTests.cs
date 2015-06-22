using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Xml;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncServiceXmlMapperTests
    {
        [Fact]
        public void Test_Create()
        {
            string filename = @"C:\temp\mediasync.profiles.xml";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            IDataStore mapper = new XmlDataStore(filename, null);

            SyncProfile p = new SyncProfile
            {
                Id = 1,
                Name = "testprofile",
                ReserveSpace = 500,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<Repository>()
            };

            mapper.CreateSyncProfile(p);
            mapper.SaveChanges();
        }
    }
}

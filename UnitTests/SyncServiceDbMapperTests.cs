using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Mappers.Mock;
using AssimilationSoftware.MediaSync.Core.Model;
using System.Collections.Generic;
using Xunit;

namespace AssimilationSoftware.MediaSync.UnitTests
{
    public class SyncServiceDbMapperTests
    {
        [Fact]
        public void Test_Create()
        {
            IDataStore mapper = new MockDataStore();

            SyncSet p = new SyncSet
            {
                Id = 1,
                Name = "testprofile",
                ReserveSpace = 300,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<FileIndex>()
            };

            mapper.CreateSyncProfile(p);
            mapper.SaveChanges();
        }

        [Fact]
        public void Test_Read()
        {
            IDataStore mapper = new MockDataStore();

            mapper.GetAllSyncProfile();
        }

        [Fact]
        public void Test_Update()
        {
            IDataStore mapper = new MockDataStore();

            SyncSet p = new SyncSet
            {
                Id = 1,
                Name = "testprofile",
                ReserveSpace = 300,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<FileIndex>()
            };

            mapper.CreateSyncProfile(p);
            mapper.SaveChanges();

            p.Name = "updatedname";
            p.ReserveSpace = 1000;
            p.SearchPatterns.Add("*.jpg");

            mapper.SaveChanges();

            SyncSet s = mapper.GetSyncProfileById(p.Id);

            Assert.Equal(p.Id, s.Id);
            Assert.Equal(p.Name, s.Name);
            Assert.Equal(p.ReserveSpace, s.ReserveSpace);
            Assert.Equal(p.SearchPatterns.Count, s.SearchPatterns.Count);
            foreach (var search in p.SearchPatterns)
            {
                Assert.True(s.SearchPatterns.Contains(search));
            }
        }

        [Fact]
        public void Test_Delete()
        {
            IDataStore mapper = new MockDataStore();

            SyncSet p = new SyncSet
            {
                Id = 1,
                Name = "testprofile",
                ReserveSpace = 300,
                SearchPatterns = new List<string>(new string[] { "*.*" }),
                Participants = new List<FileIndex>()
            };

            mapper.CreateSyncProfile(p);
            mapper.SaveChanges();
            mapper.DeleteSyncProfile(p);
        }
    }
}

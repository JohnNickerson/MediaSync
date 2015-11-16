using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssimilationSoftware.MediaSync.Core.Mappers.XML;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting
{
    [TestClass]
    public class XmlSyncSetMapperTests
    {
        [TestMethod]
        public void ReadAllShouldNotBeNull()
        {
            // Arrange
            var mapper = new XmlSyncSetMapper("SyncData.xml");

            // Act
            var all = mapper.ReadAll();

            // Assert
            Assert.IsNotNull(all);
        }

        [TestMethod]
        public void AddNewSyncSet()
        {
            var mapper = new XmlSyncSetMapper("SyncData.xml");

            var nss = new SyncSet
            {
                Id = 1,
                IgnorePatterns = new System.Collections.Generic.List<string> { ".git" },
                MasterIndex = new FileIndex(),
                Name = "TestSyncSet",
                Participants = new System.Collections.Generic.List<FileIndex>(),
                ReserveSpace = 10000,
                SearchPatterns = new System.Collections.Generic.List<string>()
            };
            mapper.Update(nss);
        }
    }
}

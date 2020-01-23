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
                MasterIndex = new FileIndex(),
                Name = "TestSyncSet",
                ReserveSpace = 10000,
            };
            mapper.Update(nss);
        }
    }
}

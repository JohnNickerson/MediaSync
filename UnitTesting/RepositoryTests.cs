using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.Maroon.Repositories;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTesting.Mocks;

namespace UnitTesting
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void AddFileToIndex()
        {
            IDiskMapper<FileSystemEntry> mockfilemapper = new MockFileMapper();
            IDiskMapper<FileIndex> mockIndexMapper = new MockIndexMapper();
            IDiskMapper<Library> mockLibMapper = new MockLibraryMapper();
            IDiskMapper<Machine> mockMechMapper = new MockMachineMapper();
            IDiskMapper<Replica> mockRepMapper = new MockReplicaMapper();
            var repository = new DataStore(new SingleOriginRepository<FileSystemEntry>(mockfilemapper, "toots.poop"),
                new SingleOriginRepository<FileIndex>(mockIndexMapper, "ugh.no"),
                new SingleOriginRepository<Library>(mockLibMapper, "libraries.notarealfile"),
                new SingleOriginRepository<Machine>(mockMechMapper, "thing.it"),
                new SingleOriginRepository<Replica>(mockRepMapper, "itsame.filio"));
            // Start with a library index with a file in transit.
            var lib = new Library()
            {
                Name = "TestLib"
            };
            repository.Insert(lib);
            // And a replica with a local index.
            var rep = new Replica()
            {
                LibraryId = lib.ID
            };
            repository.Insert(rep);
            var dex = new FileIndex()
            {
                LibraryId = lib.ID,
                ReplicaId = rep.ID
            };
            repository.Insert(dex);
            // Add the file to a local index.
            var fil = new FileHeader()
            {
                BasePath = @"C:\Temp",
                ContentsHash = "123",
                IndexId = dex.ID,
                RelativePath = "pickles.io",
                Size = 3,
                State = FileSyncState.Transit
            };
            repository.Insert(fil);
            // Get the file from the index listing.
            var indexes = repository.GetIndexesByLibraryId(lib.ID);
            Assert.IsTrue(indexes.Any());
            foreach (var idex in indexes)
            {
                var dexfiles = repository.GetFilesByIndex(idex);
                Assert.IsTrue(dexfiles.Any());
            }
        }
    }
}

using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestClass]
    public class ViewModelTests
    {
        [TestMethod]
        public void DeletedMasterFileShouldDeleteLocally()
        {
            // Arrange.
            // I need a file in the master index marked as deleted.
            // It needs also to exist on the local file system and in the local index.
            var thefile = new FileHeader { ContentsHash = "1A", IsDeleted = false, LastModified = DateTime.Now, RelativePath = "ToBeDeleted.txt", Size = 30, FileName = @"C:\Temp\ToBeDeleted.txt" };
            var filesystem = new Mocks.MockFileManager(thefile);
            var ss = new SyncSet
            {
                Name = "TestSync",
                ReserveSpace = 1000,
                MasterIndex = new FileIndex
                {
                    Files = new List<FileHeader>
                    {
                        new FileHeader
                        {
                            ContentsHash = "1A",
                            IsDeleted = true,
                            LastModified = DateTime.Now,
                            RelativePath = "ToBeDeleted.txt",
                            Size=30,
                            FileName=@"C:\Temp\ToBeDeleted.txt"
                        }
                    }
                },
                Indexes = new List<FileIndex>
                {
                    new FileIndex
                    {
                        MachineName = "Home",
                        IsPush = true,
                        IsPull = true,
                        LocalPath = @"C:\Temp",
                        SharedPath = @"E:\Temp",
                        Files = new List<FileHeader>
                        {
                            thefile
                        }
                    }
                }
            };
            var mockIndexes = new Mocks.MockDataContext(ss);
            
            var vm = new ViewModel(mockIndexes, "Home", filesystem);

            // Act.
            vm.RunSync(true, false, null);

            // Assert.
            // Local file system should no longer have the file.
            Assert.IsFalse(filesystem.FileExists(Path.Combine("C:\\temp", thefile.RelativePath)));
            // Local index should also be missing the file now.
            Assert.IsFalse(ss.GetIndex("Home").Files.Any(f => f.RelativePath == thefile.RelativePath));
        }
    }
}

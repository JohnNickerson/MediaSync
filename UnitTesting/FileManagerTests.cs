using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
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
    public class FileManagerTests
    {
        [TestMethod]
        public void CopyFile()
        {
            var filemanager = new QueuedDiskCopier(new MockHasher());

            string targetfile = @"C:\Temp\mscopytesttarget.txt";
            filemanager.CopyFile(@"C:\Temp\mscopytest.txt", targetfile);

            Assert.IsTrue(File.Exists(targetfile));
        }

        [TestMethod]
        public void CreateIndex()
        {
            var filemanager = new QueuedDiskCopier(new MockHasher());

            string folder = @"C:\temp";
            var index = filemanager.CreateIndex(folder, new string[] { "*.txt" });

            Assert.IsNotNull(index);
        }

        [TestMethod]
        public void FilesMatch_HashMismatchShouldBeFalse()
        {
            // Arrange
            var masterFile = new AssimilationSoftware.MediaSync.Core.Model.FileHeader
            {
                ContentsHash = "123",
                FileName = "temp.txt",
                IsDeleted = false,
                LastModified = DateTime.Now,
                RelativePath = "Thoughts\\temp.txt",
                Size = 100
            };
            var indexFile = new FileHeader
            {
                ContentsHash = "456",
                FileName = "temp.txt",
                IsDeleted = false,
                LastModified = DateTime.Now,
                RelativePath = "Thoughts\\temp.txt",
                Size = 100
            };

            // Act
            var match = new QueuedDiskCopier(new MockHasher()).FilesMatch(masterFile, indexFile);


            // Assert
            Assert.IsFalse(match);
        }
        
    }
}

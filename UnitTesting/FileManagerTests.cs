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

            // Arrange:
            // Ensure the source file exists first.
            var sourcefile = @"C:\Temp\mscopytest.txt";
            File.WriteAllText(sourcefile, sourcefile);
            // And ensure the target file does not yet exist.
            string targetfile = @"C:\Temp\mscopytesttarget.txt";
            if (File.Exists(targetfile)) File.Delete(targetfile);

            filemanager.CopyFile(sourcefile, targetfile);

            // Wait for the copy. This file manager runs copies on a background thread.
            System.Threading.Thread.Sleep(1000);

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
                BasePath = @"C:\Users\John\Documents",
                IsDeleted = false,
                LastModified = DateTime.Now,
                RelativePath = "Thoughts\\temp.txt",
                Size = 100
            };
            var indexFile = new FileHeader
            {
                ContentsHash = "456",
                BasePath = @"C:\Users\John\Documents",
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

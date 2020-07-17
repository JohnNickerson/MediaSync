using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using AssimilationSoftware.MediaSync.Core.FileManagement;

namespace UnitTesting
{
    [TestClass]
    public class FileManagerTests
    {
        [TestMethod]
        public void CopyFile()
        {
            var fileManager = new SimpleFileManager(new MockHasher());

            // Arrange:
            // Ensure the source file exists first.
            var sourcefile = @"C:\Temp\mscopytest.txt";
            File.WriteAllText(sourcefile, sourcefile);
            // And ensure the target file does not yet exist.
            string targetfile = @"C:\Temp\mscopytesttarget.txt";
            if (File.Exists(targetfile)) File.Delete(targetfile);

            fileManager.CopyFile(sourcefile, targetfile);

            // Wait for the copy. This file manager runs copies on a background thread.
            System.Threading.Thread.Sleep(1000);

            Assert.IsTrue(File.Exists(targetfile));
        }

        [TestMethod]
        public void CreateIndex()
        {
            var filemanager = new SimpleFileManager(new MockHasher());

            string folder = @"C:\temp";
            var index = filemanager.CreateIndex(folder, new[] { "*.txt" });

            Assert.IsNotNull(index);
        }

        [TestMethod]
        public void FilesMatch_HashMismatchShouldBeFalse()
        {
            // Arrange
            var primaryFile = new FileHeader
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
            var match = new SimpleFileManager(new MockHasher()).FilesMatch(primaryFile, indexFile);


            // Assert
            Assert.IsFalse(match);
        }
        
    }
}

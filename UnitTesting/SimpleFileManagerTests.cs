using AssimilationSoftware.MediaSync.Core.FileManagement;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using AssimilationSoftware.MediaSync.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTesting
{
    [TestClass]
    public class SimpleFileManagerTests
    {
        [TestMethod]
        public void FilesMatch_ReturnsTrueWhenSizeAndHashMatch()
        {
            var primaryFile = new FileHeader
            {
                ContentsHash = "abc",
                Size = 100
            };
            var indexFile = new FileHeader
            {
                ContentsHash = "abc",
                Size = 100
            };

            var actual = new SimpleFileManager(new MockHasher()).FilesMatch(primaryFile, indexFile);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void GetRelativePath_RemovesBasePathAndNormalisesSeparator()
        {
            var basePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(basePath);
            try
            {
                var absolutePath = Path.Combine(basePath, "folder", "file.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
                File.WriteAllText(absolutePath, "test");

                var manager = new SimpleFileManager(new MockHasher());
                var relative = manager.GetRelativePath(absolutePath, basePath);

                Assert.AreEqual(Path.Combine("folder", "file.txt"), relative);
            }
            finally
            {
                Directory.Delete(basePath, true);
            }
        }

        [TestMethod]
        public void CreateFileHeader_ReturnsFileHeaderForExistingFile()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var fileName = "test.txt";
                var filePath = Path.Combine(tempDir, fileName);
                File.WriteAllText(filePath, "hello world");

                var manager = new SimpleFileManager(new MockHasher());
                var entry = manager.CreateFileHeader(tempDir, fileName);

                Assert.IsInstanceOfType(entry, typeof(FileHeader));
                var fileHeader = (FileHeader)entry;
                Assert.AreEqual(fileName, fileHeader.RelativePath);
                Assert.AreEqual(tempDir, fileHeader.BasePath);
                Assert.AreEqual(0, fileHeader.ContentsHash.Length);
                Assert.AreEqual(new FileInfo(filePath).Length, fileHeader.Size);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void CreateFileHeader_ReturnsFolderHeaderForExistingDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var subDir = Path.Combine(tempDir, "subfolder");
            Directory.CreateDirectory(subDir);
            try
            {
                var manager = new SimpleFileManager(new MockHasher());
                var entry = manager.CreateFileHeader(tempDir, "subfolder");

                Assert.IsInstanceOfType(entry, typeof(FolderHeader));
                var folderHeader = (FolderHeader)entry;
                Assert.AreEqual("subfolder", folderHeader.RelativePath);
                Assert.AreEqual(tempDir, folderHeader.BasePath);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void TryCreateFileHeader_ReturnsNullForMissingPath()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var manager = new SimpleFileManager(new MockHasher());
                var entry = manager.TryCreateFileHeader(tempDir, "missing.txt");

                Assert.IsNull(entry);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void GetConflictFileName_AppendsVersionIfConflictExists()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var localFile = Path.Combine(tempDir, "hello.txt");
                File.WriteAllText(localFile, "hello");
                var now = new DateTime(2026, 4, 15);
                var machineId = "me";
                var firstConflict = Path.Combine(tempDir, "hello (me-s conflicted copy 2026-04-15).txt");
                File.WriteAllText(firstConflict, "conflict");

                var manager = new SimpleFileManager(new MockHasher());
                var name = manager.GetConflictFileName(localFile, machineId, now);

                Assert.IsTrue(name.EndsWith(" (1).txt", StringComparison.Ordinal));
                Assert.IsFalse(File.Exists(name));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void ListLocalFiles_ReturnsRelativeDirectoriesAndFiles()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var childDir = Path.Combine(tempDir, "child");
            Directory.CreateDirectory(childDir);
            try
            {
                var filePath = Path.Combine(childDir, "one.txt");
                File.WriteAllText(filePath, "content");

                var manager = new SimpleFileManager(new MockHasher());
                var items = manager.ListLocalFiles(tempDir, "*.txt");

                CollectionAssert.Contains(items, "child");
                CollectionAssert.Contains(items, Path.Combine("child", "one.txt").Replace(Path.DirectorySeparatorChar, '\\'));
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public void SharedPathSize_ReturnsSumOfAllFilesInSubdirectories()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var childDir = Path.Combine(tempDir, "child");
            Directory.CreateDirectory(childDir);
            try
            {
                var file1 = Path.Combine(tempDir, "first.txt");
                var file2 = Path.Combine(childDir, "second.txt");
                File.WriteAllText(file1, "one");
                File.WriteAllText(file2, "two more" );

                var manager = new SimpleFileManager(new MockHasher());
                var size = manager.SharedPathSize(tempDir);

                Assert.AreEqual((ulong)new FileInfo(file1).Length + (ulong)new FileInfo(file2).Length, size);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}

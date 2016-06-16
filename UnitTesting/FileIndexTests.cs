using AssimilationSoftware.MediaSync.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    [TestClass]
    public class FileIndexTests
    {
        [TestMethod]
        public void GetFileHeaderSuccess()
        {
            // Arrange
            var findex = GetSimpleTestIndex();

            // Act
            var f = findex.GetFile("Test.txt");

            // Assert
            Assert.IsNotNull(f);
        }

        [TestMethod]
        public void GetFileHeaderCaseInsensitive()
        {
            var findex = GetSimpleTestIndex();

            var f = findex.GetFile("test.TXT");

            Assert.IsNotNull(f);
        }

        [TestMethod]
        public void RemoveFileHeader()
        {
            var findex = GetSimpleTestIndex();
            var f = findex.GetFile("Test.txt");

            findex.Remove(f);

            Assert.IsNull(findex.GetFile("Test.txt"));
        }

        [TestMethod]
        public void UpdateFileHeader()
        {
            var findex = GetSimpleTestIndex();
            var f = findex.GetFile("Test.txt");

            f.ContentsHash = "2B";
            findex.UpdateFile(f);

            var g = findex.GetFile("Test.txt");
            Assert.AreEqual(f.ContentsHash, g.ContentsHash);
        }

        private FileIndex GetSimpleTestIndex()
        {
            return new FileIndex
            {
                IsPull = false,
                IsPush = true,
                LocalPath = @"C:\Users\John\Downloads",
                MachineName = "yoga",
                SharedPath = @"E:\MediaSync\Shared\Downloads",
                TimeStamp = DateTime.Now,
                Files = new List<FileHeader>
                {
                    new FileHeader
                    {
                        ContentsHash = "1A",
                        FileName=@"C:\Users\John\Downloads\Test.txt",
                        IsDeleted=false,
                        LastModified=DateTime.Now,
                        RelativePath="Test.txt",
                        Size=42
                    }
                }
            };
        }
    }
}

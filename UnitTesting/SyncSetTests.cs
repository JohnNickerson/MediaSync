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
    public class SyncSetTests
    {
        [TestMethod]
        public void AddIndex()
        {
            // Arrange
            var ss = GetTestSyncSet();

            // Act
            var fh = new FileHeader
            {
                ContentsHash = "2B",
                BasePath = @"C:\Users\John\Downloads",
                IsDeleted = false,
                LastModified = DateTime.Now,
                RelativePath = @"Thingy.txt", 
                Size = 23
            };

            // Assert
        }

        private SyncSet GetTestSyncSet()
        {
            return null;
        }
    }
}

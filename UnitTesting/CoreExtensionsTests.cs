using AssimilationSoftware.MediaSync.Core.Extensions;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTesting
{
    [TestClass]
    public class CoreExtensionsTests
    {
        [TestMethod]
        public void TimeSpanExtensions_Verbalise_ReturnsWeeksAndDays()
        {
            var duration = TimeSpan.FromDays(15);
            var result = duration.Verbalise();
            Assert.AreEqual("2w 1d", result);
        }

        [TestMethod]
        public void TimeSpanExtensions_Verbalise_ReturnsDaysAndHours()
        {
            var duration = TimeSpan.FromHours(30);
            var result = duration.Verbalise();
            Assert.AreEqual("1d 6h", result);
        }

        [TestMethod]
        public void TimeSpanExtensions_Verbalise_ReturnsHoursAndMinutes()
        {
            var duration = TimeSpan.FromMinutes(90);
            var result = duration.Verbalise();
            Assert.AreEqual("1h 30m", result);
        }

        [TestMethod]
        public void TimeSpanExtensions_Verbalise_ReturnsMinutesAndSeconds()
        {
            var duration = TimeSpan.FromSeconds(90);
            var result = duration.Verbalise();
            Assert.AreEqual("1m 30s", result);
        }

        [TestMethod]
        public void TimeSpanExtensions_Verbalise_ReturnsSecondsForShortDurations()
        {
            var duration = TimeSpan.FromSeconds(5);
            var result = duration.Verbalise();
            Assert.AreEqual("5s", result);
        }

        [TestMethod]
        public void TimeSpanExtensions_Verbalise_PreservesNegativeDurations()
        {
            var duration = TimeSpan.FromSeconds(-65);
            var result = duration.Verbalise();
            Assert.AreEqual("-1m 5s", result);
        }

        [TestMethod]
        public void DirectoryInfoExtensions_IsSubPathOf_ReturnsTrueForChildren()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            var child = new DirectoryInfo(Path.Combine(root.FullName, "sub"));
            Directory.CreateDirectory(child.FullName);
            try
            {
                Assert.IsTrue(child.IsSubPathOf(root));
            }
            finally
            {
                if (root.Exists)
                {
                    root.Delete(true);
                }
            }
        }

        [TestMethod]
        public void DirectoryInfoExtensions_IsSubPathOf_ReturnsTrueForSamePath()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(root.FullName);
            try
            {
                Assert.IsTrue(root.IsSubPathOf(root));
            }
            finally
            {
                if (root.Exists)
                {
                    root.Delete(true);
                }
            }
        }

        [TestMethod]
        public void DirectoryInfoExtensions_IsSubPathOf_ReturnsFalseForDifferentRoots()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            var other = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            Directory.CreateDirectory(root.FullName);
            Directory.CreateDirectory(other.FullName);
            try
            {
                Assert.IsFalse(root.IsSubPathOf(other));
            }
            finally
            {
                if (root.Exists)
                {
                    root.Delete(true);
                }
                if (other.Exists)
                {
                    other.Delete(true);
                }
            }
        }

        [TestMethod]
        public void Sha1Calculator_ComputeHash_ReturnsNonEmptyHash_ForExistingFile()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(tempFile, "hash test");
            try
            {
                var calculator = new Sha1Calculator();
                var hash = calculator.ComputeHash(tempFile);
                Assert.IsFalse(string.IsNullOrEmpty(hash));
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [TestMethod]
        public void Sha1Calculator_ComputeHash_ReturnsEmptyStringForMissingFile()
        {
            var calculator = new Sha1Calculator();
            var hash = calculator.ComputeHash(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "doesnotexist.txt"));
            Assert.AreEqual(string.Empty, hash);
        }

        [TestMethod]
        public void Sha1Calculator_ClearCache_DoesNotThrowAndAllowsRecompute()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(tempFile, "cached content");
            try
            {
                var calculator = new Sha1Calculator();
                var first = calculator.ComputeHash(tempFile);
                calculator.ClearCache(tempFile);
                var second = calculator.ComputeHash(tempFile);
                Assert.AreEqual(first, second);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}

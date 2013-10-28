using AssimilationSoftware.MediaSync.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for QueuedDiskCopierTest and is intended
    ///to contain all QueuedDiskCopierTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QueuedDiskCopierTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for QueuedDiskCopier Constructor
        ///</summary>
        [TestMethod()]
        public void QueuedDiskCopierConstructorTest()
        {
            SyncProfile profile = new SyncProfile { ProfileName = "TestProfile", ReserveSpace = 1000, SearchPatterns = new List<string>() };
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            QueuedDiskCopier target = new QueuedDiskCopier(profile, indexer);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.CopyFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void CopyFileTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            string source = string.Empty; // TODO: Initialize to an appropriate value
            string target1 = string.Empty; // TODO: Initialize to an appropriate value
            target.CopyFile(source, target1);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.Delete
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void DeleteTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            string dir = string.Empty; // TODO: Initialize to an appropriate value
            target.Delete(dir);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.EnsureFolder
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void EnsureFolderTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            string targetdir = string.Empty; // TODO: Initialize to an appropriate value
            target.EnsureFolder(targetdir);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.SetNormalAttributes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void SetNormalAttributesTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            target.SetNormalAttributes();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.SharedPathSize
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void SharedPathSizeTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            ulong expected = 0; // TODO: Initialize to an appropriate value
            ulong actual;
            actual = target.SharedPathSize();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.ShouldCopy
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void ShouldCopyTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ShouldCopy(filename);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateIndex
        ///</summary>
        [TestMethod()]
        public void CreateIndexTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            QueuedDiskCopier target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            FileIndex expected = null; // TODO: Initialize to an appropriate value
            FileIndex actual;
            actual = target.CreateIndex();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FinishCopy
        ///</summary>
        [TestMethod()]
        public void FinishCopyTest()
        {
            SyncProfile profile = SyncProfileTest.GetMockSyncProfile();
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            QueuedDiskCopier target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            IAsyncResult result = null; // TODO: Initialize to an appropriate value
            target.FinishCopy(result);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ListLocalFiles
        ///</summary>
        [TestMethod()]
        public void ListLocalFilesTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            QueuedDiskCopier target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.ListLocalFiles();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.Count
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void CountTest()
        {
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.Count;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IFileManager.Errors
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void ErrorsTest()
        {
            SyncProfile profile = SyncProfileTest.GetMockSyncProfile();
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager target = new QueuedDiskCopier(profile, indexer); // TODO: Initialize to an appropriate value
            List<Exception> actual;
            actual = target.Errors;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

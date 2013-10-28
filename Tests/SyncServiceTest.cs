using AssimilationSoftware.MediaSync.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for SyncServiceTest and is intended
    ///to contain all SyncServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SyncServiceTest
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
        ///A test for SyncService Constructor
        ///</summary>
        [TestMethod()]
        public void SyncServiceConstructorTest()
        {
            SyncProfile opts = null; // TODO: Initialize to an appropriate value
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ClearEmptyFolders
        ///</summary>
        [TestMethod()]
        public void ClearEmptyFoldersTest()
        {
            SyncProfile opts = null; // TODO: Initialize to an appropriate value
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            target.ClearEmptyFolders();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for IndexFiles
        ///</summary>
        [TestMethod()]
        public void IndexFilesTest()
        {
            SyncProfile opts = new SyncProfile { ProfileName = "TestProfile", ReserveSpace = 1000, SearchPatterns = new System.Collections.Generic.List<string>() };
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            target.IndexFiles();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PruneFiles
        ///</summary>
        [TestMethod()]
        public void PruneFilesTest()
        {
            SyncProfile opts = null; // TODO: Initialize to an appropriate value
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PruneFiles();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PullFiles
        ///</summary>
        [TestMethod()]
        public void PullFilesTest()
        {
            SyncProfile opts = null; // TODO: Initialize to an appropriate value
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PullFiles();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PushFiles
        ///</summary>
        [TestMethod()]
        public void PushFilesTest()
        {
            SyncProfile opts = null; // TODO: Initialize to an appropriate value
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PushFiles();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Sync
        ///</summary>
        [TestMethod()]
        public void SyncTest()
        {
            SyncProfile opts = SyncProfileTest.GetMockSyncProfile();
            IOutputView view = null; // TODO: Initialize to an appropriate value
            IIndexMapper indexer = null; // TODO: Initialize to an appropriate value
            IFileManager filemanager = null; // TODO: Initialize to an appropriate value
            bool simulate = false; // TODO: Initialize to an appropriate value
            SyncService target = new SyncService(opts, view, indexer, filemanager, simulate); // TODO: Initialize to an appropriate value
            target.Sync();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for VerbaliseBytes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void VerbaliseBytesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SyncService_Accessor target = new SyncService_Accessor(param0); // TODO: Initialize to an appropriate value
            ulong bytes = 0; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.VerbaliseBytes(bytes);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WaitForCopies
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void WaitForCopiesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SyncService_Accessor target = new SyncService_Accessor(param0); // TODO: Initialize to an appropriate value
            target.WaitForCopies();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}

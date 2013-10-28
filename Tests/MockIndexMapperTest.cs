using AssimilationSoftware.MediaSync.Mappers.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Interfaces;
using System.Collections.Generic;
using AssimilationSoftware.MediaSync.Model;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for MockIndexMapperTest and is intended
    ///to contain all MockIndexMapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MockIndexMapperTest
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
        ///A test for MockIndexMapper Constructor
        ///</summary>
        [TestMethod()]
        public void MockIndexMapperConstructorTest()
        {
            MockIndexMapper target = new MockIndexMapper();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IIndexMapper.CompareCounts
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void CompareCountsTest()
        {
            IIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            Dictionary<string, int> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, int> actual;
            actual = target.CompareCounts();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IIndexMapper.LoadLatest
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void LoadLatestTest()
        {
            IIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            string profile = string.Empty; // TODO: Initialize to an appropriate value
            FileIndex expected = null; // TODO: Initialize to an appropriate value
            FileIndex actual;
            actual = target.LoadLatest(machine, profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IIndexMapper.Save
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void SaveTest()
        {
            IIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            FileIndex index = null; // TODO: Initialize to an appropriate value
            target.Save(index);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CreateIndex
        ///</summary>
        [TestMethod()]
        public void CreateIndexTest()
        {
            MockIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            IFileManager file_manager = null; // TODO: Initialize to an appropriate value
            target.CreateIndex(file_manager);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            MockIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.Load(machine, profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest1()
        {
            MockIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.Load(machine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest2()
        {
            MockIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.Load(profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LoadAll
        ///</summary>
        [TestMethod()]
        public void LoadAllTest()
        {
            MockIndexMapper target = new MockIndexMapper(); // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.LoadAll();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

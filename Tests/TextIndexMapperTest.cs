using AssimilationSoftware.MediaSync.Mappers.PlainText;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for TextIndexMapperTest and is intended
    ///to contain all TextIndexMapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TextIndexMapperTest
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
        ///A test for TextIndexMapper Constructor
        ///</summary>
        [TestMethod()]
        public void TextIndexMapperConstructorTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IIndexMapper.CompareCounts
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void CompareCountsTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            IIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            Dictionary<string, int> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, int> actual;
            actual = target.CompareCounts();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.Load(profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest1()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
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
            SyncProfile _options = SyncProfileTest.GetMockSyncProfile();
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile profile = null; // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.Load(machine, profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LoadAll
        ///</summary>
        [TestMethod()]
        public void LoadAllTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            List<FileIndex> expected = null; // TODO: Initialize to an appropriate value
            List<FileIndex> actual;
            actual = target.LoadAll();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LoadLatest
        ///</summary>
        [TestMethod()]
        public void LoadLatestTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            string profile = string.Empty; // TODO: Initialize to an appropriate value
            FileIndex expected = null; // TODO: Initialize to an appropriate value
            FileIndex actual;
            actual = target.LoadLatest(machine, profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            SyncProfile _options = null; // TODO: Initialize to an appropriate value
            TextIndexMapper target = new TextIndexMapper(_options); // TODO: Initialize to an appropriate value
            FileIndex index = null; // TODO: Initialize to an appropriate value
            target.Save(index);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for IndexFileName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void IndexFileNameTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            TextIndexMapper_Accessor target = new TextIndexMapper_Accessor(param0); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.IndexFileName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

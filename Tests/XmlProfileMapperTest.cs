using AssimilationSoftware.MediaSync.Mappers.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Model;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for XmlProfileMapperTest and is intended
    ///to contain all XmlProfileMapperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XmlProfileMapperTest
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
        ///A test for XmlProfileMapper Constructor
        ///</summary>
        [TestMethod()]
        public void XmlProfileMapperConstructorTest()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            XmlProfileMapper target = new XmlProfileMapper(profileListFilename);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IProfileMapper.Load
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void LoadTest()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            IProfileMapper target = new XmlProfileMapper(profileListFilename); // TODO: Initialize to an appropriate value
            string machineName = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile[] expected = null; // TODO: Initialize to an appropriate value
            SyncProfile[] actual;
            actual = target.Load(machineName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IProfileMapper.Load
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void LoadTest1()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            IProfileMapper target = new XmlProfileMapper(profileListFilename); // TODO: Initialize to an appropriate value
            List<SyncProfile> expected = null; // TODO: Initialize to an appropriate value
            List<SyncProfile> actual;
            actual = target.Load();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IProfileMapper.Load
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void LoadTest2()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            IProfileMapper target = new XmlProfileMapper(profileListFilename); // TODO: Initialize to an appropriate value
            string machineName = string.Empty; // TODO: Initialize to an appropriate value
            string profile = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile expected = null; // TODO: Initialize to an appropriate value
            SyncProfile actual;
            actual = target.Load(machineName, profile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IProfileMapper.Save
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void SaveTest()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            IProfileMapper target = new XmlProfileMapper(profileListFilename); // TODO: Initialize to an appropriate value
            List<SyncProfile> profiles = null; // TODO: Initialize to an appropriate value
            target.Save(profiles);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IProfileMapper.Save
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void SaveTest1()
        {
            string profileListFilename = string.Empty; // TODO: Initialize to an appropriate value
            IProfileMapper target = new XmlProfileMapper(profileListFilename); // TODO: Initialize to an appropriate value
            string machineName = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile saveobject = null; // TODO: Initialize to an appropriate value
            target.Save(machineName, saveobject);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}

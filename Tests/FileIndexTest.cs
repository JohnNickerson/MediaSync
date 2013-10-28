using AssimilationSoftware.MediaSync.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for FileIndexTest and is intended
    ///to contain all FileIndexTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileIndexTest
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
        ///A test for FileIndex Constructor
        ///</summary>
        [TestMethod()]
        public void FileIndexConstructorTest()
        {
            FileIndex target = new FileIndex();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Files
        ///</summary>
        [TestMethod()]
        public void FilesTest()
        {
            FileIndex target = new FileIndex(); // TODO: Initialize to an appropriate value
            List<FileHeader> expected = null; // TODO: Initialize to an appropriate value
            List<FileHeader> actual;
            target.Files = expected;
            actual = target.Files;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for LocalBasePath
        ///</summary>
        [TestMethod()]
        public void LocalBasePathTest()
        {
            FileIndex target = new FileIndex(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.LocalBasePath = expected;
            actual = target.LocalBasePath;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MachineName
        ///</summary>
        [TestMethod()]
        public void MachineNameTest()
        {
            FileIndex target = new FileIndex(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.MachineName = expected;
            actual = target.MachineName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProfileName
        ///</summary>
        [TestMethod()]
        public void ProfileNameTest()
        {
            FileIndex target = new FileIndex(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ProfileName = expected;
            actual = target.ProfileName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TimeStamp
        ///</summary>
        [TestMethod()]
        public void TimeStampTest()
        {
            FileIndex target = new FileIndex(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.TimeStamp = expected;
            actual = target.TimeStamp;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

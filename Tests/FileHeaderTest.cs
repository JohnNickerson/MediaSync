using AssimilationSoftware.MediaSync.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.FileManagement.Hashing;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for FileHeaderTest and is intended
    ///to contain all FileHeaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileHeaderTest
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
        ///A test for FileHeader Constructor
        ///</summary>
        [TestMethod()]
        public void FileHeaderConstructorTest()
        {
            FileHeader target = new FileHeader();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for FileHeader Constructor
        ///</summary>
        [TestMethod()]
        public void FileHeaderConstructorTest1()
        {
            string filename = string.Empty; // TODO: Initialize to an appropriate value
            string basepath = string.Empty; // TODO: Initialize to an appropriate value
            IFileHashProvider hash = new MockHasher();
            FileHeader target = new FileHeader(filename, basepath, hash);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ContentsHash
        ///</summary>
        [TestMethod()]
        public void ContentsHashTest()
        {
            FileHeader target = new FileHeader(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ContentsHash = expected;
            actual = target.ContentsHash;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FileName
        ///</summary>
        [TestMethod()]
        public void FileNameTest()
        {
            FileHeader target = new FileHeader(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.FileName = expected;
            actual = target.FileName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FileSize
        ///</summary>
        [TestMethod()]
        public void FileSizeTest()
        {
            FileHeader target = new FileHeader(); // TODO: Initialize to an appropriate value
            long expected = 0; // TODO: Initialize to an appropriate value
            long actual;
            target.FileSize = expected;
            actual = target.FileSize;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RelativePath
        ///</summary>
        [TestMethod()]
        public void RelativePathTest()
        {
            FileHeader target = new FileHeader(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.RelativePath = expected;
            actual = target.RelativePath;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

using AssimilationSoftware.MediaSync.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for SyncOperationTest and is intended
    ///to contain all SyncOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SyncOperationTest
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
        ///A test for SyncOperation Constructor
        ///</summary>
        [TestMethod()]
        public void SyncOperationConstructorTest()
        {
            string deletetarget = string.Empty; // TODO: Initialize to an appropriate value
            SyncOperation target = new SyncOperation(deletetarget);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for SyncOperation Constructor
        ///</summary>
        [TestMethod()]
        public void SyncOperationConstructorTest1()
        {
            string source = string.Empty; // TODO: Initialize to an appropriate value
            string target1 = string.Empty; // TODO: Initialize to an appropriate value
            SyncOperation.SyncAction action = new SyncOperation.SyncAction(); // TODO: Initialize to an appropriate value
            SyncOperation target = new SyncOperation(source, target1, action);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}

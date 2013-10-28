using AssimilationSoftware.MediaSync.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for SyncProfileTest and is intended
    ///to contain all SyncProfileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SyncProfileTest
    {
        public static SyncProfile GetMockSyncProfile()
        {
            return new SyncProfile { ProfileName = "TestProfile", ReserveSpace = 1000, SearchPatterns = new List<string>(new string[] { "*.csv" }) };
        }

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
        ///A test for SyncProfile Constructor
        ///</summary>
        [TestMethod()]
        public void SyncProfileConstructorTest()
        {
            SyncProfile target = new SyncProfile();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ContainsParticipant
        ///</summary>
        [TestMethod()]
        public void ContainsParticipantTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ContainsParticipant(machine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetParticipant
        ///</summary>
        [TestMethod()]
        public void GetParticipantTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            string machine = string.Empty; // TODO: Initialize to an appropriate value
            ProfileParticipant expected = null; // TODO: Initialize to an appropriate value
            ProfileParticipant actual;
            actual = target.GetParticipant(machine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetLocalMachineName
        ///</summary>
        [TestMethod()]
        public void SetLocalMachineNameTest()
        {
            string name = string.Empty; // TODO: Initialize to an appropriate value
            SyncProfile.SetLocalMachineName(name);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Participants
        ///</summary>
        [TestMethod()]
        public void ParticipantsTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            List<ProfileParticipant> expected = null; // TODO: Initialize to an appropriate value
            List<ProfileParticipant> actual;
            target.Participants = expected;
            actual = target.Participants;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProfileName
        ///</summary>
        [TestMethod()]
        public void ProfileNameTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ProfileName = expected;
            actual = target.ProfileName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReserveSpace
        ///</summary>
        [TestMethod()]
        public void ReserveSpaceTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            ulong expected = 0; // TODO: Initialize to an appropriate value
            ulong actual;
            target.ReserveSpace = expected;
            actual = target.ReserveSpace;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SearchPatterns
        ///</summary>
        [TestMethod()]
        public void SearchPatternsTest()
        {
            SyncProfile target = new SyncProfile(); // TODO: Initialize to an appropriate value
            List<string> expected = null; // TODO: Initialize to an appropriate value
            List<string> actual;
            target.SearchPatterns = expected;
            actual = target.SearchPatterns;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

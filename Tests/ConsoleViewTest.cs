using AssimilationSoftware.MediaSync.Core.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Model;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for ConsoleViewTest and is intended
    ///to contain all ConsoleViewTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConsoleViewTest
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
        ///A test for ConsoleView Constructor
        ///</summary>
        [TestMethod()]
        public void ConsoleViewConstructorTest()
        {
            ConsoleView target = new ConsoleView();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IOutputView.Report
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void ReportTest()
        {
            IOutputView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            SyncOperation op = null; // TODO: Initialize to an appropriate value
            target.Report(op);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IOutputView.WriteLine
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void WriteLineTest()
        {
            IOutputView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            string format = string.Empty; // TODO: Initialize to an appropriate value
            object[] args = null; // TODO: Initialize to an appropriate value
            target.WriteLine(format, args);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IOutputView.WriteLine
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void WriteLineTest1()
        {
            IOutputView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            target.WriteLine();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ConfigurePath
        ///</summary>
        [TestMethod()]
        public void ConfigurePathTest()
        {
            ConsoleView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            string path = string.Empty; // TODO: Initialize to an appropriate value
            string prompt = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ConfigurePath(path, prompt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ConfigureString
        ///</summary>
        [TestMethod()]
        public void ConfigureStringTest()
        {
            ConsoleView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            string value = string.Empty; // TODO: Initialize to an appropriate value
            string prompt = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ConfigureString(value, prompt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ConfigureULong
        ///</summary>
        [TestMethod()]
        public void ConfigureULongTest()
        {
            ConsoleView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            ulong value = 0; // TODO: Initialize to an appropriate value
            string prompt = string.Empty; // TODO: Initialize to an appropriate value
            ulong expected = 0; // TODO: Initialize to an appropriate value
            ulong actual;
            actual = target.ConfigureULong(value, prompt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WaitForKey
        ///</summary>
        [TestMethod()]
        public void WaitForKeyTest()
        {
            ConsoleView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            target.WaitForKey();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for AssimilationSoftware.MediaSync.Interfaces.IOutputView.Status
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Client.exe")]
        public void StatusTest()
        {
            IOutputView target = new ConsoleView(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            target.Status = expected;
            Assert.Inconclusive("Write-only properties cannot be verified.");
        }
    }
}

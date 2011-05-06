using Humpback.Parts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Humpback.ConfigurationOptions;

namespace Humpback.Tests
{
    
    
    /// <summary>
    ///This is a test class for SQLServerFormatterTest and is intended
    ///to contain all SQLServerFormatterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SQLServerFormatterTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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


        private dynamic CreateTable() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic DropTable() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic AddColumns() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic RemoveColumn() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic ModifyColumn() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic AddIndex() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic RemoveIndex() {
            string json = @"";
            return Helpers.DeserializeMigration(json);
        }

        /// <summary>
        ///A test for GetCommand
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Hump.exe")]
        public void GetCommandTest() {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SQLServerFormatter_Accessor target = new SQLServerFormatter_Accessor(param0); // TODO: Initialize to an appropriate value
            object op = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetCommand(op);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReadMinds
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Hump.exe")]
        public void ReadMindsTest() {
            object up = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = SQLServerFormatter_Accessor.ReadMinds(up);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GenerateSQLDown
        ///</summary>
        [TestMethod()]
        public void GenerateSQLDownTest() {
            Configuration configuration = null; // TODO: Initialize to an appropriate value
            SQLServerFormatter target = new SQLServerFormatter(configuration); // TODO: Initialize to an appropriate value
            object operation = null; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.GenerateSQLDown(operation);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GenerateSQLUp
        ///</summary>
        [TestMethod()]
        public void GenerateSQLUpTest() {
            Configuration configuration = null; // TODO: Initialize to an appropriate value
            SQLServerFormatter target = new SQLServerFormatter(configuration); // TODO: Initialize to an appropriate value
            object operation = null; // TODO: Initialize to an appropriate value
            string[] expected = null; // TODO: Initialize to an appropriate value
            string[] actual;
            actual = target.GenerateSQLUp(operation);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}

using System.Linq;
using Humpback.ConfigurationOptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Humpback.Tests
{
    
    
    /// <summary>
    ///This is a test class for ConfigurationTest and is intended
    ///to contain all ConfigurationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationTest {


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


        /// <summary>
        ///A test for Configuration Constructor
        ///</summary>
        [TestMethod()]
        public void ConfigurationConstructorTestNullConstructorParam() {
            string[] options = null; // TODO: Initialize to an appropriate value
            Configuration target = new Configuration(options);
            Assert.IsTrue(target.WriteHelp);
            Assert.AreEqual(0,target.MigrateToVersion);
        }


        /// <summary>
        ///A test for Configuration Constructor
        ///</summary>
        [TestMethod()]
        public void ConfigurationConstructorTestDefaultConstructor() {
            Configuration target = new Configuration();
            Assert.IsTrue(target.WriteHelp);
            Assert.AreEqual(0, target.MigrateToVersion);
        }


        /// <summary>
        ///A test for Help
        ///</summary>
        [TestMethod()]
        public void HelpTestDash() {
            var target = new Configuration(new[]{"-?"});
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpTest_h() {
            var target = new Configuration(new[] { "-h" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpTest_H() {
            var target = new Configuration(new[] { "-H" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpTest_HELP() {
            var target = new Configuration(new[] { "-HELP" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpTest_help() {
            var target = new Configuration(new[] { "-help" });
            Assert.IsTrue(target.WriteHelp);
        }

        /// <summary>
        ///A test for HelpModifier
        ///</summary>
        [TestMethod()]
        public void HelpModifierTest_Run() {
            var target = new Configuration(new[] { "-? Migrate" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_RUN() {
            var target = new Configuration(new[] { "-? MIGRATE" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_List() {
            var target = new Configuration(new[] { "-? List" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_Generate() {
            var target = new Configuration(new[] { "-? Generate" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_Sql() {
            var target = new Configuration(new[] { "-? Sql" });
            Assert.IsTrue(target.WriteHelp);
        }

        [TestMethod()]
        public void HelpModifierTest_R() {
            var target = new Configuration(new[] { "-? M" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_L() {
            var target = new Configuration(new[] { "-? L" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_G() {
            var target = new Configuration(new[] { "-? G" });
            Assert.IsTrue(target.WriteHelp);
        }
        [TestMethod()]
        public void HelpModifierTest_S_and_Ensure_Run_Off() {
            var target = new Configuration(new[] { "-? S" });
            Assert.IsTrue(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
        }


        [TestMethod()]
        public void BasicSqlTest() {
            Configuration target = new Configuration(new[] { "-S" });
            Assert.IsTrue(target.Sql);
        }
        [TestMethod()]
        public void BasicSqlTest2() {
            Configuration target = new Configuration(new[] { "--Sql" });
            Assert.IsTrue(target.Sql);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
        }
        [TestMethod()]
        public void BasicSqlTest3() {
            Configuration target = new Configuration(new[] { "/Sql" });
            Assert.IsTrue(target.Sql);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
            Assert.IsFalse(target.List);
            Assert.IsFalse(target.Generate);
        }

        [TestMethod()]
        public void BasicGenerateErrorTest() {
            try {
                Configuration target = new Configuration(new[] {"-G"});
                Assert.IsTrue(false, "Configuration should have failed");
            } catch (Exception ex) {
                Assert.IsNotNull(ex);
            }

        }
        [TestMethod()]
        public void BasicGenerateTest() {
            Configuration target = new Configuration(new[] { "-G","AddMyMigration" });
            Assert.IsTrue(target.Generate);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
            Assert.IsFalse(target.List);
        }

        [TestMethod()]
        public void GenerateTest() {
            Configuration target = new Configuration(new[] { "-G", "table", "first_name:string", "last_name:string" });
            Assert.IsTrue(target.Generate);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
            Assert.IsFalse(target.List);
            Console.WriteLine(String.Join("|",target.Extra.ToArray()));
            Assert.IsNotNull(target.Extra);
            Assert.IsTrue(target.Extra.Count() > 0);
        }

        [TestMethod()]
        public void GenerateTest_CreateTable() {
            Configuration target = new Configuration(new[] { "--G", "users", "first_name:string", "last_name" });
            Assert.IsTrue(target.Generate);
            Assert.AreEqual("users", target.GenerateString); 
        }

        [TestMethod()]
        public void GenerateTest_ModifyTable() {
            Configuration target = new Configuration(new[] { "--G", "AddAgeToUsers", "first_name:string", "last_name" });
            Assert.IsTrue(target.Generate);
            Assert.AreEqual("AddAgeToUsers", target.GenerateString);
        }


        [TestMethod()]
        public void BasicListTest() {
            Configuration target = new Configuration(new[] { "-List" });
            Assert.IsTrue(target.List);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.Migrate);
            Assert.IsFalse(target.Generate);
        }
        [TestMethod()]
        public void BasicRunTest() {
            Configuration target = new Configuration(new[] { "-RUN" });
            Assert.IsTrue(target.Migrate);
            Assert.IsFalse(target.WriteHelp);
            Assert.IsFalse(target.List);
            Assert.IsFalse(target.Generate);
        }
    }
}

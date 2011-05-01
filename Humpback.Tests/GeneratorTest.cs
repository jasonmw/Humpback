using Humpback.Parts;
using Humpback.Tests.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Humpback.ConfigurationOptions;
using Humpback;
using System.Collections.Generic;

namespace Humpback.Tests
{
    
    
    /// <summary>
    ///This is a test class for GeneratorTest and is intended
    ///to contain all GeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeneratorTest {


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
        ///A test for Generator
        ///</summary>
        [TestMethod()]
        public void GeneratorAddTableNoColumnsSpecified() {
            Configuration configuration = new Configuration(new[] { "-g","Users" });
            TestFileWriter file_writer = new TestFileWriter(); 
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileContents.Contains("create_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("drop_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("column_name_here"));
        }

        [TestMethod()]
        public void GeneratorAddTable1Column() {
            Configuration configuration = new Configuration(new[] { "-g", "Users", "first_name:string" });
            TestFileWriter file_writer = new TestFileWriter(); 
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("down"));
            Assert.IsTrue(file_writer.FileContents.Contains("create_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("Users"));
            Assert.IsTrue(file_writer.FileContents.Contains("drop_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
        }

        [TestMethod()]
        public void GeneratorAddTableMultipleColumn() {
            Configuration configuration = new Configuration(new[] { "-g", "Users", "first_name:string", "last_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileContents.Contains("create_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("down"));
            Assert.IsTrue(file_writer.FileContents.Contains("drop_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
            Assert.IsTrue(file_writer.FileContents.Contains("last_name"));
            Assert.IsTrue(file_writer.FileContents.Contains("timestamps"));
        }

        [TestMethod()]
        public void GeneratorDropTable() {
            Configuration configuration = new Configuration(new[] { "-g", "DropUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("DropTable"));
            Assert.IsTrue(file_writer.FileContents.Contains("drop_table"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("Users"));
        }

        [TestMethod()]
        public void GeneratorRemoveColumn() {
            Configuration configuration = new Configuration(new[] { "-g", "Removefirst_nameFromUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("RemoveColumn"));
            Assert.IsTrue(file_writer.FileContents.Contains("remove_column"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
        }
        /// <summary>
        ///A test for AddColumn
        ///</summary>
        [TestMethod()]
        public void GeneratorAddSingleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addfirst_nametoUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("AddColumn"));
            Assert.IsTrue(file_writer.FileContents.Contains("add_column"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
        }
        [TestMethod()]
        public void GeneratorAddMultipleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addfirst_nametoUsers", "first_name:string", "last_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("AddColumn"));
            Assert.IsTrue(file_writer.FileContents.Contains("add_column"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
            Assert.IsTrue(file_writer.FileContents.Contains("last_name"));
        }

        /// <summary>
        ///A test for AddIndex
        ///</summary>
        [TestMethod()]
        public void GeneratorAddIndexTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addindextoUsers", "first_name", "last_name" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("AddIndex"));
            Assert.IsTrue(file_writer.FileContents.Contains("add_index"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
            Assert.IsTrue(file_writer.FileContents.Contains("last_name"));
        }
        [TestMethod()]
        public void GeneratorDropIndexTest() {
            Configuration configuration = new Configuration(new[] { "-g", "removeindexfromUsers", "first_name", "last_name" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("Users"));
            Assert.IsTrue(file_writer.FileName.Contains("RemoveIndex"));
            Assert.IsTrue(file_writer.FileContents.Contains("remove_index"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
            Assert.IsTrue(file_writer.FileContents.Contains("first_name"));
            Assert.IsTrue(file_writer.FileContents.Contains("last_name"));
        }

        /// <summary>
        ///A test for File
        ///</summary>
        [TestMethod()]
        public void GeneratorFileTest() {
            Configuration configuration = new Configuration(new[] { "-g", "File", "mysqlfile" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("File"));
            Assert.IsTrue(file_writer.FileContents.Contains("file"));
            Assert.IsTrue(file_writer.FileContents.Contains("mysqlfile"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
        }


        /// <summary>
        ///A test for Raw SQL
        ///</summary>
        [TestMethod()]
        public void GeneratorSqlTest() {
            Configuration configuration = new Configuration(new[] { "-g", "SQL", "CREATE TABLE Users (ID {pk}, first_name {string} NOT NULL, price_paid {money})" });
            TestFileWriter file_writer = new TestFileWriter();
            IPart target = new Generator(configuration, file_writer);
            target.Execute();
            Assert.IsTrue(file_writer.FileName.Contains("SQL"));
            Assert.IsTrue(file_writer.FileContents.Contains("CREATE TABLE Users (ID {pk}, first_name {string} NOT NULL, price_paid {money})"));
            Assert.IsTrue(file_writer.FileContents.Contains("up"));
        }

    }
}

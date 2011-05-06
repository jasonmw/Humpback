using System.Collections.Generic;
using Humpback.Parts;
using Humpback.Tests.Impl;
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



        private KeyValuePair<string, string> CreateTable() {
            var json = @"{'up':{'create_table':{'name':'tname','columns':[{'name':'first_name','type':'string'},{'name':'last_name','type':'money'}]}},'down':{'drop_table':'tname'}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic DropTable() {
            string json = @"{'up':{'drop_table':'tname'}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic AddColumns() {
            string json = @"{'up':{'add_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}},'down':{'remove_column':{'table':'tname','column':'name'}}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic RemoveColumn() {
            string json = @"{'up':{'remove_column':{'table':'tname','column':'name'}}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic ModifyColumn() {
            string json = @"{'up':{'change_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic AddIndex() {
            string json = @"{'up':{add_index:{table_name:'categories',columns:['title','slug']}}}";
            return Helpers.DeserializeMigration(json);
        }
        private dynamic RemoveIndex() {
            string json = @"{'up':{remove_index:{table_name:'categories',columns:['title','slug']}}}";
            return Helpers.DeserializeMigration(json);
        }

        [TestMethod()]
        public void SqlFormatterAddTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(1); // create table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("1"));
            Assert.IsTrue(file_writer.FileContents.Contains("CREATE TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("[first_name] nvarchar(255)"));
            Assert.IsTrue(file_writer.FileContents.Contains("[last_name] decimal"));
            Assert.IsTrue(file_writer.FileContents.Contains("CreatedOn datetime DEFAULT getdate() NOT NULL"));
            Assert.IsTrue(file_writer.FileContents.Contains("Id"));
            Assert.IsTrue(file_writer.FileContents.Contains("PRIMARY KEY"));
        }

        [TestMethod()]
        public void SqlFormatterDropTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "2" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(2); // drop table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("2"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP TABLE [tname]"));
        }
        [TestMethod()]
        public void SqlFormatterAddColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "3" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(3); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("3"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("ADD [name]"));
            Assert.IsTrue(file_writer.FileContents.Contains("nvarchar(255)"));
        }
        [TestMethod()]
        public void SqlFormatterChangeColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "4" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(3); // change col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("4"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER COLUMN [name]"));
            Assert.IsTrue(file_writer.FileContents.Contains("nvarchar(255)"));
        }
        [TestMethod()]
        public void SqlFormatterDropColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "5" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(4); // change col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("5"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP COLUMN [name]"));
        }
        [TestMethod()]
        public void SqlFormatterAddIndexTest() {
            Configuration configuration = new Configuration(new[] { "-s", "6" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(5); // add index
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("6"));
            Assert.IsTrue(file_writer.FileContents.Contains("CREATE NONCLUSTERED INDEX [IX_categories_title_slug] ON [categories] ( [title] ASC, [slug] ASC )"));
        }
        [TestMethod()]
        public void SqlFormatterDropIndexTest() {
            Configuration configuration = new Configuration(new[] { "-s", "7" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(6); // add index
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("7"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP INDEX [categories].[IX_categories_title_slug]"));
        }
    }

    public class TestMigrationProvider:IMigrationProvider {
        public SortedDictionary<int, string> GetMigrations() {
            var rv = new SortedDictionary<int,string>();
            for(int i = 1; i <= 7; i++) {
                rv.Add(i, i.ToString()+"xxx.sql");
            }
            return rv;
        }

        public SortedDictionary<int, string> GetMigrationsContents() {
            var rv = new SortedDictionary<int, string>();
            for (int i = 1; i <= 7; i++) {
                rv.Add(i, GetMigrationWithContents(i).Value);
            }
            return rv;
        }

        public KeyValuePair<string, string> GetMigrationWithContents(int migration_number) {
            switch(migration_number){
                case 1: return CreateTable();
                case 2: return DropTable();
                case 3: return AddColumns();
                case 4: return ChangeColumns();
                case 5: return RemoveColumn();
                case 6: return AddIndex();
                case 7: return RemoveIndex();
                default:
                    return CreateTable();
            }
        }

        private int migration_number = 0;
        public int DatabaseMigrationNumber() {
            return migration_number;
        }

        public void SetMigrationNumber(int number) {
            migration_number = number;
        }


        private KeyValuePair<string,string> CreateTable() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'create_table':{'name':'tname','timestamps':true,'columns':[{'name':'first_name','type':'string'},{'name':'last_name','type':'money'}]}},'down':{'drop_table':'tname'}}");
        }
        private KeyValuePair<string, string> DropTable() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'drop_table':'tname'}}");
        }
        private KeyValuePair<string, string> AddColumns() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'add_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}},'down':{'remove_column':{'table':'tname','column':'name'}}}");
        }
        private KeyValuePair<string, string> RemoveColumn() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'remove_column':{'table':'tname','column':'name'}}}");
        }
        private KeyValuePair<string, string> ChangeColumns() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'change_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}}}");
        }
        private KeyValuePair<string, string> AddIndex() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{add_index:{table_name:'categories',columns:['title','slug']}}}");
        }
        private KeyValuePair<string, string> RemoveIndex() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{remove_index:{table_name:'categories',columns:['title','slug']}}}");
        }

    }
}

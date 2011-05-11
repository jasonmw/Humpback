using System.Collections.Generic;
using System.IO;
using Humpback.Parts;
using Humpback.Tests.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Humpback.ConfigurationOptions;
using Humpback.Interfaces;

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




        private static Settings Settings {get {
            return TestHelpers.TestSettings;

        }}
        [TestMethod()]
        public void SqlFormatterAddTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(1); // create table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("1"));
            Assert.IsTrue(file_writer.FileContents.Contains("CREATE TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("[first_name] nvarchar(255)"));
            Assert.IsTrue(file_writer.FileContents.Contains("[last_name] decimal"));
            Assert.IsTrue(file_writer.FileContents.Contains("CreatedOn datetime DEFAULT getutcdate() NOT NULL"));
            Assert.IsTrue(file_writer.FileContents.Contains("Id"));
            Assert.IsTrue(file_writer.FileContents.Contains("PRIMARY KEY"));
        }
        [TestMethod()]
        public void SqlFormatterAddTableTestWithNullableAndDefault() {
            Configuration configuration = new Configuration(new[] { "-s", "11" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(11); // create table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("11"));
            Assert.IsTrue(file_writer.FileContents.Contains("CREATE TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("[first_name] nvarchar(255)"));
            Assert.IsTrue(file_writer.FileContents.Contains("[last_name] decimal"));
            Assert.IsTrue(file_writer.FileContents.Contains("CreatedOn datetime DEFAULT getutcdate() NOT NULL"));
            Assert.IsTrue(file_writer.FileContents.Contains("Id"));
            Assert.IsTrue(file_writer.FileContents.Contains("PRIMARY KEY"));
            Assert.IsTrue(file_writer.FileContents.Contains("[first_name] nvarchar(255) NOT NULL  DEFAULT ('JASON')"));
        }
        [TestMethod()]
        public void SqlFormatterDropTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "2" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(2); // drop table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("2"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP TABLE [tname]"));
        }

        [TestMethod()]
        public void SqlFormatterDrop2TableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "8" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(8); // drop table
            target.Execute();
            Console.WriteLine(file_writer.FileName);
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("8"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP TABLE [tname1]"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP TABLE [tname2]"));
        }

        [TestMethod()]
        public void SqlFormatterAddColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "3" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(3); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("3"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.IsTrue(file_writer.FileContents.Contains("ADD [name]"));
            Assert.IsTrue(file_writer.FileContents.Contains("nvarchar(255)"));
        }
        
        [TestMethod()]
        public void SqlFormatterAddColumnReferenceTest() {
            Configuration configuration = new Configuration(new[] { "-s", "9" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(9); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("9"));
            Assert.IsTrue(file_writer.FileContents.Contains("FK_orders_user_userid"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [Orders] ADD [UserId] INT NOT NULL"));
            Assert.IsTrue(file_writer.FileContents.Contains("ALTER TABLE [Orders] ADD CONSTRAINT [FK_orders_user_userid] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION"));
        }
        

        [TestMethod()]
        public void SqlFormatterAddTableReferenceTest() {
            Configuration configuration = new Configuration(new[] { "-s", "10" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(10); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("10"));
            Assert.IsTrue(file_writer.FileContents.Contains("FK_applicationcontrol_applicationpage_applicationpageid"));
            Assert.IsTrue(file_writer.FileContents.Contains("FK_applicationcontrol_application_applicationid"));
            Assert.IsTrue(file_writer.FileContents.Contains("FOREIGN KEY ([ApplicationPageId]) REFERENCES [ApplicationPage] ([Id])"));
            Assert.IsTrue(file_writer.FileContents.Contains("FOREIGN KEY ([ApplicationId]) REFERENCES [Application] ([Id])"));
        }



        [TestMethod()]
        public void SqlFormatterChangeColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "4" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
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
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
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
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
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
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(6); // add index
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.IsTrue(file_writer.FileName.Contains("7"));
            Assert.IsTrue(file_writer.FileContents.Contains("DROP INDEX [categories].[IX_categories_title_slug]"));
        }
    }

    public class TestMigrationProvider:IMigrationProvider {
        public SortedDictionary<int, string> GetMigrations() {
            var rv = new SortedDictionary<int, string>();
            foreach (var key in functions_dictionary.Keys) {
                rv.Add(key, key + "xxx.sql");
            }
            return rv;
        }

        public SortedDictionary<int, string> GetMigrationsContents() {
            var rv = new SortedDictionary<int, string>();
            foreach(var key in functions_dictionary.Keys) {
                rv.Add(key, functions_dictionary[key]().Value);
            }
            return rv;
        }

        public KeyValuePair<string, string> GetMigrationWithContents(int migration_number) {
            if (functions_dictionary.ContainsKey(migration_number)) {
                return functions_dictionary[migration_number]();
            }
            return functions_dictionary[1]();
        }

        private int migration_number = 0;
        public int DatabaseMigrationNumber() {
            return migration_number;
        }

        public void SetMigrationNumber(int number) {
            migration_number = number;
        }

        private readonly Dictionary<int,Func<KeyValuePair<string, string>>> functions_dictionary =
            new Dictionary<int,Func<KeyValuePair<string, string>>> {
                {1, CreateTable},
                {2, DropTable},
                {3, AddColumns},
                {4, ChangeColumns},
                {5, RemoveColumn},
                {6, AddIndex},
                {7, RemoveIndex},
                {8, Drop2Tables},
                {9, AddColumnReference},
                {10, AddTableTwoReference},
                {11, CreateTableWithNullAndDefault}
            };



        private static KeyValuePair<string,string> CreateTable() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'create_table':{'name':'tname','timestamps':true,'columns':[{'name':'first_name','type':'string'},{'name':'last_name','type':'money'}]}},'down':{'drop_table':'tname'}}");
        }
        private static KeyValuePair<string, string> DropTable() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'drop_table':'tname'}}");
        }
        private static KeyValuePair<string, string> Drop2Tables() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':[{'drop_table':'tname1'},{'drop_table':'tname2'}]}");
        }
        private static KeyValuePair<string, string> AddColumns() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'add_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}},'down':{'remove_column':{'table':'tname','column':'name'}}}");
        }
        private static KeyValuePair<string, string> RemoveColumn() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'remove_column':{'table':'tname','column':'name'}}}");
        }
        private static KeyValuePair<string, string> ChangeColumns() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'change_column':{'table':'tname','columns':[{'name':'name','type':'string'}]}}}");
        }
        private static KeyValuePair<string, string> AddIndex() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{add_index:{table_name:'categories',columns:['title','slug']}}}");
        }
        private static KeyValuePair<string, string> RemoveIndex() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{remove_index:{table_name:'categories',columns:['title','slug']}}}");
        }
        private static KeyValuePair<string, string> AddColumnReference() {
            return new KeyValuePair<string, string>("7xxx.sql",
                                                    @"{'up': {'add_column': {'table': 'Orders','columns': [{'name': 'User','type': 'reference'}]}},'down': {'remove_column': {'table': 'Orders','column': 'UserId'}}}");
        }
        private static KeyValuePair<string, string> AddTableTwoReference() {
            return new KeyValuePair<string, string>("7xxx.sql",
                                                    @"{'up': {'create_table': {'name': 'ApplicationControl','timestamps': true,'columns': [{'name': 'name','type': 'string'},{'name': 'ApplicationPage','type': 'reference'},{'name': 'Application','type': 'reference'}]}},'down': {'drop_table': 'ApplicationControl'}}");
        }
        private static KeyValuePair<string, string> CreateTableWithNullAndDefault() {
            return new KeyValuePair<string, string>("7xxx.sql", @"{'up':{'create_table':{'name':'tname','timestamps':true,'columns':[{'name':'first_name','type':'string', 'nullable':false, default:""'JASON'""},{'name':'last_name','type':'money'}]}},'down':{'drop_table':'tname'}}");
        }
        
    }
}

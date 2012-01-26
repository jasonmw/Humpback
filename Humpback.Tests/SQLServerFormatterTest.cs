using System.Collections.Generic;
using System.IO;
using Humpback.Parts;
using Humpback.Tests.Impl;
using System;
using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Xunit;

namespace Humpback.Tests
{
    public class SQLServerFormatterTest {

        private static Settings Settings {get {
            return TestHelpers.TestSettings;

        }}
        [Fact]
        public void SqlFormatterAddTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(1); // create table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("1"));
            Assert.True(file_writer.FileContents.Contains("CREATE TABLE [tname]"));
            Assert.True(file_writer.FileContents.Contains("[first_name] nvarchar(255)"));
            Assert.True(file_writer.FileContents.Contains("[last_name] decimal"));
            Assert.True(file_writer.FileContents.Contains("CreatedOn datetime DEFAULT getutcdate() NOT NULL"));
            Assert.True(file_writer.FileContents.Contains("Id"));
            Assert.True(file_writer.FileContents.Contains("PRIMARY KEY"));
        }
        [Fact]
        public void SqlFormatterAddTableTestWithNullableAndDefault() {
            Configuration configuration = new Configuration(new[] { "-s", "11" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(11); // create table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("11"));
            Assert.True(file_writer.FileContents.Contains("CREATE TABLE [tname]"));
            Assert.True(file_writer.FileContents.Contains("[first_name] nvarchar(255)"));
            Assert.True(file_writer.FileContents.Contains("[last_name] decimal"));
            Assert.True(file_writer.FileContents.Contains("CreatedOn datetime DEFAULT getutcdate() NOT NULL"));
            Assert.True(file_writer.FileContents.Contains("Id"));
            Assert.True(file_writer.FileContents.Contains("PRIMARY KEY"));
            Assert.True(file_writer.FileContents.Contains("[first_name] nvarchar(255) NOT NULL  DEFAULT ('JASON')"));
        }
        [Fact]
        public void SqlFormatterDropTableTest() {
            Configuration configuration = new Configuration(new[] { "-s", "2" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(2); // drop table
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("2"));
            Assert.True(file_writer.FileContents.Contains("DROP TABLE [tname]"));
        }

        [Fact]
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
            Assert.True(file_writer.FileName.Contains("8"));
            Assert.True(file_writer.FileContents.Contains("DROP TABLE [tname1]"));
            Assert.True(file_writer.FileContents.Contains("DROP TABLE [tname2]"));
        }

        [Fact]
        public void SqlFormatterAddColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "3" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(3); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("3"));
            Assert.True(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.True(file_writer.FileContents.Contains("ADD [name]"));
            Assert.True(file_writer.FileContents.Contains("nvarchar(255)"));
        }
        
        [Fact]
        public void SqlFormatterAddColumnReferenceTest() {
            Configuration configuration = new Configuration(new[] { "-s", "9" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(9); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("9"));
            Assert.True(file_writer.FileContents.Contains("FK_orders_user_userid"));
            Assert.True(file_writer.FileContents.Contains("ALTER TABLE [Orders] ADD [UserId] INT NOT NULL"));
            Assert.True(file_writer.FileContents.Contains("ALTER TABLE [Orders] ADD CONSTRAINT [FK_orders_user_userid] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION"));
        }
        

        [Fact]
        public void SqlFormatterAddTableReferenceTest() {
            Configuration configuration = new Configuration(new[] { "-s", "10" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            //migrations.SetMigrationNumber(10); // add col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("10"));
            Assert.True(file_writer.FileContents.Contains("FK_applicationcontrol_applicationpage_applicationpageid"));
            Assert.True(file_writer.FileContents.Contains("FK_applicationcontrol_application_applicationid"));
            Assert.True(file_writer.FileContents.Contains("FOREIGN KEY ([ApplicationPageId]) REFERENCES [ApplicationPage] ([Id])"));
            Assert.True(file_writer.FileContents.Contains("FOREIGN KEY ([ApplicationId]) REFERENCES [Application] ([Id])"));
        }



        [Fact]
        public void SqlFormatterChangeColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "4" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(3); // change col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("4"));
            Assert.True(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.True(file_writer.FileContents.Contains("ALTER COLUMN [name]"));
            Assert.True(file_writer.FileContents.Contains("nvarchar(255)"));
        }
        [Fact]
        public void SqlFormatterDropColumnTest() {
            Configuration configuration = new Configuration(new[] { "-s", "5" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(4); // change col
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("5"));
            Assert.True(file_writer.FileContents.Contains("ALTER TABLE [tname]"));
            Assert.True(file_writer.FileContents.Contains("DROP COLUMN [name]"));
        }
        [Fact]
        public void SqlFormatterAddIndexTest() {
            Configuration configuration = new Configuration(new[] { "-s", "6" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(5); // add index
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("6"));
            Assert.True(file_writer.FileContents.Contains("CREATE NONCLUSTERED INDEX [IX_categories_title_slug] ON [categories] ( [title] ASC, [slug] ASC )"));
        }
        [Fact]
        public void SqlFormatterDropIndexTest() {
            Configuration configuration = new Configuration(new[] { "-s", "7" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            IMigrationProvider migrations = new TestMigrationProvider();
            IHumpbackCommand target = new GenerateSQL(configuration, Settings, formatter, file_writer, migrations);
            migrations.SetMigrationNumber(6); // add index
            target.Execute();
            Console.WriteLine(file_writer.FileContents);
            Assert.True(file_writer.FileName.Contains("7"));
            Assert.True(file_writer.FileContents.Contains("DROP INDEX [categories].[IX_categories_title_slug]"));
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

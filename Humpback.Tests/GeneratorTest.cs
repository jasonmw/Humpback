using Humpback.Parts;
using Humpback.Tests.Impl;
using System;
using Humpback.ConfigurationOptions;
using Humpback;
using System.Collections.Generic;
using Xunit;

namespace Humpback.Tests
{
    
    public class GeneratorTest {

        private static Settings Settings {
            get {
                return TestHelpers.TestSettings;

            }
        }
        /// <summary>
        ///A test for Generator
        ///</summary>
        [Fact]
        public void GeneratorAddTableNoColumnsSpecified() {
            Configuration configuration = new Configuration(new[] { "-g","Users" });
            TestFileWriter file_writer = new TestFileWriter(); 
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileContents.Contains("create_table"));
            Assert.True(file_writer.FileContents.Contains("drop_table"));
        }

        [Fact]
        public void GeneratorAddTable1Column() {
            Configuration configuration = new Configuration(new[] { "-g", "Users", "first_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("down"));
            Assert.True(file_writer.FileContents.Contains("create_table"));
            Assert.True(file_writer.FileContents.Contains("Users"));
            Assert.True(file_writer.FileContents.Contains("drop_table"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
        }

        [Fact]
        public void GeneratorAddTableMultipleColumn() {
            Configuration configuration = new Configuration(new[] { "-g", "Users", "first_name:string", "last_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileContents.Contains("create_table"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("down"));
            Assert.True(file_writer.FileContents.Contains("drop_table"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
            Assert.True(file_writer.FileContents.Contains("full_audit"));
        }
        [Fact]
        public void GeneratorAddTableMultipleColumnWithNullable() {
            Configuration configuration = new Configuration(new[] { "-g", "Users", "first_name:string:false", "last_name:string:true:'welty'" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileContents.Contains("create_table"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("down"));
            Assert.True(file_writer.FileContents.Contains("drop_table"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
            Assert.True(file_writer.FileContents.Contains("\"nullable\":false"));
            Assert.True(file_writer.FileContents.Contains("\"default\":\"'welty'\""));
            Assert.True(file_writer.FileContents.Contains("full_audit"));
        }

        [Fact]
        public void GeneratorDropTable() {
            Configuration configuration = new Configuration(new[] { "-g", "DropUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("DropTable"));
            Assert.True(file_writer.FileContents.Contains("drop_table"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("Users"));
        }

        [Fact]
        public void GeneratorRemoveColumn() {
            Configuration configuration = new Configuration(new[] { "-g", "Removefirst_nameFromUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("RemoveColumn"));
            Assert.True(file_writer.FileContents.Contains("remove_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
        }
        /// <summary>
        ///A test for AddColumn
        ///</summary>
        [Fact]
        public void GeneratorAddSingleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addfirst_nametoUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("AddColumn"));
            Assert.True(file_writer.FileContents.Contains("add_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
        }

        [Fact]
        public void GeneratorAddSingleColumnTest_Escaped_To() {
            Configuration configuration = new Configuration(new[] { "-g", "addcustomer_name_to_Users" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("AddColumn"));
            Assert.True(file_writer.FileContents.Contains("add_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("customer_name"));
        }

        [Fact]
        public void GeneratorAddSingleColumnTestDuplicateTo() {
            Configuration configuration = new Configuration(new[] { "-g", "addcustomernametoUsers" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.Null(file_writer.FileContents);
        }

        [Fact]
        public void GeneratorAddMultipleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addfirst_nametoUsers", "first_name:string", "last_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("AddColumn"));
            Assert.True(file_writer.FileContents.Contains("add_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
        }


        [Fact]
        public void GeneratorAddReferenceColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "AddUserToOrders", "User:reference" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("AddColumn_Orders_UserId"));
            Assert.True(file_writer.FileContents.Contains("add_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("\"name\":\"User\",\"type\":\"reference\"}]"));
            Assert.True(file_writer.FileContents.Contains("remove_column"));
            Assert.True(file_writer.FileContents.Contains("\"column\":\"UserId\""));
        }


        /// <summary>
        ///A test for AddColumn
        ///</summary>
        [Fact]
        public void GeneratorChangeSingleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "ChangeUsers", "first_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("ChangeColumn"));
            Assert.True(file_writer.FileContents.Contains("change_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("down"));
        }
        [Fact]
        public void GeneratorChangeMultipleColumnTest() {
            Configuration configuration = new Configuration(new[] { "-g", "ChangeUsers", "first_name:string", "last_name:string" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("ChangeColumn"));
            Assert.True(file_writer.FileContents.Contains("change_column"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
            Assert.True(file_writer.FileContents.Contains("down"));
        }
        /// <summary>
        ///A test for AddIndex
        ///</summary>
        [Fact]
        public void GeneratorAddIndexTest() {
            Configuration configuration = new Configuration(new[] { "-g", "addindextoUsers", "first_name", "last_name" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("AddIndex"));
            Assert.True(file_writer.FileContents.Contains("add_index"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
        }
        [Fact]
        public void GeneratorDropIndexTest() {
            Configuration configuration = new Configuration(new[] { "-g", "removeindexfromUsers", "first_name", "last_name" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("Users"));
            Assert.True(file_writer.FileName.Contains("RemoveIndex"));
            Assert.True(file_writer.FileContents.Contains("remove_index"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("first_name"));
            Assert.True(file_writer.FileContents.Contains("last_name"));
        }

        /// <summary>
        ///A test for File
        ///</summary>
        [Fact]
        public void GeneratorFileTest() {
            Configuration configuration = new Configuration(new[] { "-g", "File", "mysqlfile" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("File"));
            Assert.True(file_writer.FileContents.Contains("file"));
            Assert.True(file_writer.FileContents.Contains("mysqlfile"));
            Assert.True(file_writer.FileContents.Contains("up"));
            
        }


        /// <summary>
        ///A test for Raw SQL
        ///</summary>
        [Fact]
        public void GeneratorSqlTest() {
            Configuration configuration = new Configuration(new[] { "-g", "SQL", "CREATE TABLE Users (ID {pk}, first_name {string} NOT NULL, price_paid {money})" });
            TestFileWriter file_writer = new TestFileWriter();
            IHumpbackCommand target = new Generator(configuration, Settings, file_writer);
            target.Execute();
            Assert.True(file_writer.FileName.Contains("SQL"));
            Assert.True(file_writer.FileContents.Contains("CREATE TABLE Users (ID {pk}, first_name {string} NOT NULL, price_paid {money})"));
            Assert.True(file_writer.FileContents.Contains("up"));
            Assert.True(file_writer.FileContents.Contains("down"));
            Assert.True(file_writer.FileContents.Contains("\"down\":\"\""));
        }

    }
}

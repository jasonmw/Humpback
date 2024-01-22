using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Humpback.Parts;
using Humpback.Tests.Impl;
using Xunit;

namespace Humpback.Tests
{
    public class SQLGeneratorTests
    {


        private static Settings Settings
        {
            get
            {
                return TestHelpers.TestSettings;

            }
        }


        [Fact]
        public void TestGeneratePlainSQLString()
        {
            var json = "{\"up\":\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\"}";


            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            TestSQLDatabaseProvider _database_provider = new TestSQLDatabaseProvider(configuration, Settings, formatter);

            dynamic migration_object = Helpers.DeserializeMigration(json);
            _database_provider.ExecuteUpCommand(migration_object);
            Assert.Equal("DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'", _database_provider.LastCommand);


        }

        [Fact]
        public void TestGeneratePlainSQLStringArray()
        {
            var json = "{\"up\":[\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\",\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\"]}";


            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            TestSQLDatabaseProvider _database_provider = new TestSQLDatabaseProvider(configuration, Settings, formatter);

            dynamic migration_object = Helpers.DeserializeMigration(json);
            _database_provider.ExecuteUpCommand(migration_object);
            Assert.Equal("DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'", _database_provider.LastCommand);


        }





        [Fact]
        public void TestGeneratePlainSQLStringDown()
        {
            var json = "{\"down\":\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\"}";


            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            TestSQLDatabaseProvider _database_provider = new TestSQLDatabaseProvider(configuration, Settings, formatter);

            dynamic migration_object = Helpers.DeserializeMigration(json);
            _database_provider.ExecuteDownCommand(migration_object);
            Assert.Equal("DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'", _database_provider.LastCommand);


        }

        [Fact]
        public void TestGeneratePlainSQLStringArrayDown()
        {
            var json = "{\"down\":[\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\",\"DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'\"]}";

            Configuration configuration = new Configuration(new[] { "-s", "1" });
            TestFileWriter file_writer = new TestFileWriter();
            ISqlFormatter formatter = new SQLServerFormatter(configuration, Settings);
            TestSQLDatabaseProvider _database_provider = new TestSQLDatabaseProvider(configuration, Settings, formatter);

            dynamic migration_object = Helpers.DeserializeMigration(json);
            _database_provider.ExecuteDownCommand(migration_object);
            Assert.Equal("DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'DELETE FROM [Control] WHERE ControlName LIKE '%Date Label'", _database_provider.LastCommand);

        }



    }
}

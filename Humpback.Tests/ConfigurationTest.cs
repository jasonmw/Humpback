using System.Linq;
using Humpback.ConfigurationOptions;
using System;
using Xunit;

namespace Humpback.Tests
{
    
    public class ConfigurationTest {

        /// <summary>
        ///A test for Configuration Constructor
        ///</summary>
        [Fact]
        public void ConfigurationConstructorTestNullConstructorParam() {
            string[] options = null; // TODO: Initialize to an appropriate value
            Configuration target = new Configuration(options);
            Assert.True(target.WriteHelp);
            Assert.Equal(0,target.MigrateToVersion);
        }


        /// <summary>
        ///A test for Configuration Constructor
        ///</summary>
        [Fact]
        public void ConfigurationConstructorTestDefaultConstructor() {
            Configuration target = new Configuration();
            Assert.True(target.WriteHelp);
            Assert.Equal(0, target.MigrateToVersion);
        }


        /// <summary>
        ///A test for Help
        ///</summary>
        [Fact]
        public void HelpTestDash() {
            var target = new Configuration(new[]{"-?"});
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpTest_h() {
            var target = new Configuration(new[] { "-h" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpTest_H() {
            var target = new Configuration(new[] { "-H" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpTest_HELP() {
            var target = new Configuration(new[] { "-HELP" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpTest_help() {
            var target = new Configuration(new[] { "-help" });
            Assert.True(target.WriteHelp);
        }

        /// <summary>
        ///A test for HelpModifier
        ///</summary>
        [Fact]
        public void HelpModifierTest_Run() {
            var target = new Configuration(new[] { "-?", "Migrate" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_RUN() {
            var target = new Configuration(new[] { "-?", "MIGRATE" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_List() {
            var target = new Configuration(new[] { "-?", "List" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_Generate() {
            var target = new Configuration(new[] { "-?", "Generate" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_Sql() {
            var target = new Configuration(new[] { "-?", "Sql" });
            Assert.True(target.WriteHelp);
        }

        [Fact]
        public void HelpModifierTest_R() {
            var target = new Configuration(new[] { "-?", "M" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_L() {
            var target = new Configuration(new[] { "-?", "L" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_G() {
            var target = new Configuration(new[] { "-?","G" });
            Assert.True(target.WriteHelp);
        }
        [Fact]
        public void HelpModifierTest_S_and_Ensure_Run_Off() {
            var target = new Configuration(new[] { "-?", "S" });
            Assert.True(target.WriteHelp);
            Assert.False(target.Migrate);
        }


        [Fact]
        public void BasicSqlTest() {
            Configuration target = new Configuration(new[] { "-S" });
            Assert.True(target.Sql);
        }
        [Fact]
        public void BasicSqlTest2() {
            Configuration target = new Configuration(new[] { "--Sql" });
            Assert.True(target.Sql);
            Assert.False(target.WriteHelp);
            Assert.False(target.Migrate);
        }
        [Fact]
        public void BasicSqlTest3() {
            Configuration target = new Configuration(new[] { "/Sql" });
            Assert.True(target.Sql);
            Assert.False(target.WriteHelp);
            Assert.False(target.Migrate);
            Assert.False(target.List);
            Assert.False(target.Generate);
        }

        [Fact]
        public void BasicGenerateErrorTest() {
            try {
                Configuration target = new Configuration(new[] {"-G"});
                Assert.True(false, "Configuration should have failed");
            } catch (Exception ex) {
                Assert.NotNull(ex);
            }

        }
        [Fact]
        public void BasicGenerateTest() {
            Configuration target = new Configuration(new[] { "-G","AddMyMigration" });
            Assert.True(target.Generate);
            Assert.False(target.WriteHelp);
            Assert.False(target.Migrate);
            Assert.False(target.List);
        }

        [Fact]
        public void GenerateTest() {
            Configuration target = new Configuration(new[] { "-G", "table", "first_name:string", "last_name:string" });
            Assert.True(target.Generate);
            Assert.False(target.WriteHelp);
            Assert.False(target.Migrate);
            Assert.False(target.List);
            Console.WriteLine(String.Join("|",target.Extra.ToArray()));
            Assert.NotNull(target.Extra);
            Assert.True(target.Extra.Count() > 0);
        }

        [Fact]
        public void GenerateTest_CreateTable() {
            Configuration target = new Configuration(new[] { "--G", "users", "first_name:string", "last_name" });
            Assert.True(target.Generate);
            Assert.Equal("users", target.GenerateString); 
        }

        [Fact]
        public void GenerateTest_ModifyTable() {
            Configuration target = new Configuration(new[] { "--G", "AddAgeToUsers", "first_name:string", "last_name" });
            Assert.True(target.Generate);
            Assert.Equal("AddAgeToUsers", target.GenerateString);
        }


        [Fact]
        public void BasicListTest() {
            Configuration target = new Configuration(new[] { "-List" });
            Assert.True(target.List);
            Assert.False(target.WriteHelp);
            Assert.False(target.Migrate);
            Assert.False(target.Generate);
        }
        [Fact]
        public void BasicRunTest() {
            Configuration target = new Configuration(new[] { "-m" });
            Assert.True(target.Migrate);
            Assert.False(target.WriteHelp);
            Assert.False(target.List);
            Assert.False(target.Generate);
        }


        [Fact]
        public void EnvironmentTest() {
            Configuration target = new Configuration(new[] { "-e" });
            Assert.True(target.Env);
            Assert.False(target.WriteHelp);
            Assert.False(target.List);
            Assert.False(target.Generate);
        }

        [Fact]
        public void EnvironmentTestSet() {
            Configuration target = new Configuration(new[] { "-e", "-set", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.set_current_settings, "name");
        }


        [Fact]
        public void EnvironmentTestAdd() {
            Configuration target = new Configuration(new[] { "-e", "-add", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.new_project_name, "name");
        }

        [Fact]
        public void EnvironmentTestRemove() {
            Configuration target = new Configuration(new[] { "-e", "-remove", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.remove_project_name, "name");
        }


        [Fact]
        public void EnvironmentTestRename() {
            Configuration target = new Configuration(new[] { "-e", "-rename", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.settings_rename, "name");
        }

        [Fact]
        public void EnvironmentTestDir() {
            Configuration target = new Configuration(new[] { "-e", "-dir", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.settings_dir, "name");
        }

        [Fact]
        public void EnvironmentTestConnectionString() {
            Configuration target = new Configuration(new[] { "-e", "-cs", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.settings_cs, "name");
        }

        [Fact]
        public void EnvironmentTestFlavor() {
            Configuration target = new Configuration(new[] { "-e", "-flavor", "name" });
            Assert.True(target.Env);
            Assert.Equal(target.settings_flavor, "name");
        }
    }
}

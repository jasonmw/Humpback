using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Humpback.ConfigurationOptions;
using Humpback.Tools;

namespace Humpback.Parts {
    public class GenerateSQL : IHumpbackCommand {

        private ISqlFormatter _sql_formatter;
        private IFileWriter _file_writer;
        private Configuration _configuration;
        private IMigrationProvider _migration_provider;

        public GenerateSQL(Configuration configuration, ISqlFormatter sql_formatter, IFileWriter file_writer, IMigrationProvider migration_provider) {
            _configuration = configuration;
            _sql_formatter = sql_formatter;
            _file_writer = file_writer;
            _migration_provider = migration_provider;
        }

        // output override acceptable for all options
        // generate sql/files for entire body of migrations     hump -s -all -single -output=folderpath
        // generate sql/file for entire body of migrations      hump -s -all
        // generate sql file for single migration               hump -s 13
        // generate sql file for all migrations deployed        hump -s -dp
        // generate sql file for all migrations not deployed    hump -s -ndp
        // generate sql file for all migrations deployed        hump -s -dp -single
        // generate sql file for all migrations not deployed    hump -s -ndp -single


        public void Execute() {
            if(_configuration.All) {
                SaveAllSql();
            } else if (_configuration.Deployed) {
                SaveDeployed();
            } else if (_configuration.NotDeployed) {
                SaveUnDeployed();
            }
            else if (SingleMigrationID() > 0) {
                SaveSingle(SingleMigrationID());
            } else {
                SaveAllSql();
            }
        }

        private void SaveSingle(int migration_id) {
            var migrations = _migration_provider.GetMigrations();
            var migration_contents = _migration_provider.GetMigrationsContents();
            CreateSql(migrations, migration_contents, v => v.Key == migration_id);
        }
        private void SaveUnDeployed() {
            var migrations = _migration_provider.GetMigrations();
            var migration_contents = _migration_provider.GetMigrationsContents();
            CreateSql(migrations, migration_contents, v => v.Key > _migration_provider.DatabaseMigrationNumber());
        }
        private void SaveDeployed() {
            var migrations = _migration_provider.GetMigrations();
            var migration_contents = _migration_provider.GetMigrationsContents();
            CreateSql(migrations, migration_contents,v => v.Key <= _migration_provider.DatabaseMigrationNumber());
        }
        private void SaveAllSql() {
            var migrations = _migration_provider.GetMigrations();
            var migration_contents = _migration_provider.GetMigrationsContents();
            CreateSql(migrations, migration_contents, null);
        }

        private void CreateSql(SortedDictionary<int, string> migrations, SortedDictionary<int, string> migration_contents, Predicate<KeyValuePair<int,string>> allow_migration_func) {
            var sql_commands = new Dictionary<string, string>();
            int min = migrations.Count;
            int max = 1;
            foreach(var migration in migrations) {
                if(allow_migration_func != null) {
                    if(!allow_migration_func(migration)) {
                        continue;
                    }
                }
                if(migration.Key < min) {
                    min = migration.Key;
                }
                if(migration.Key > max) {
                    max = migration.Key;
                }
                var this_migration_contents = migration_contents[migration.Key];
                dynamic migration_object = Helpers.DeserializeMigration(this_migration_contents);
                sql_commands.Add(_sql_formatter.SqlFileName(migration.Value), _sql_formatter.GenerateSQLUp(migration_object)[0]);
            }
            WriteFile(sql_commands,min,max);
        }

        private void WriteFile(Dictionary<string,string> sql_commands, int from, int to) {
            if(_configuration.Single) {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine().AppendLine("-- BEGIN TRANSACTION");
                sb.AppendLine();
                foreach(var cmd in sql_commands.Values) {
                    sb.AppendLine().AppendLine(cmd).AppendLine().AppendLine("GO").AppendLine();
                }
                string file_name = string.Format("{0}_ALL_SQL_Migrations_{1}_thru_{2}.sql",
                                                 _configuration.NextSerialNumber(),
                                                 from,
                                                 to);
                sb.AppendLine();
                sb.AppendLine().AppendLine("-- COMMIT TRANSACTION"); // TODO: Transactions. Not sure to default or add another switch?
                sb.AppendLine().AppendLine().AppendLine();
                WriteOutput(sb.ToString(), file_name);

            } else {
                foreach (var cmd in sql_commands) {
                    string file_name = _sql_formatter.SqlFileName(cmd.Key);
                    string sql_out = string.Format("{0}{0}{1}{0}{0}{0}", Environment.NewLine, cmd.Value);
                    WriteOutput(sql_out, file_name);
                }
            }
        }

        private void WriteOutput(string sql_out, string file_name) {
            if (_configuration.Screen) {
                Console.WriteLine(sql_out);
            } else {
                if(file_name.Contains("\\")){
                    file_name = new FileInfo(file_name).Name;
                }
                _file_writer.WriteFile(Path.Combine(_configuration.OutputFolder, file_name), sql_out);
            }
        }


        private int SingleMigrationID() {
            int migration_id = 0;
            bool has_extra = _configuration.Extra.Any();
            bool extra_is_int = has_extra && int.TryParse(_configuration.Extra.First(), out migration_id);
            bool migration_exists = extra_is_int && _migration_provider.GetMigrations().ContainsKey(migration_id);
            return migration_exists ? migration_id : 0;
        }
    }
}

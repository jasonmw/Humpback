using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;
using Humpback.Interfaces;

namespace Humpback.Parts {
    public class SQLCEDatabaseProvider : IDatabaseProvider {

        private Configuration _configuration;
        private Settings _settings;
        private ISqlFormatter _sql_formatter;

        private SqlCeConnection GetOpenConnection() {
            var rv = new SqlCeConnection(_settings.ConnectionString());
            rv.Open();
            return rv;
        }
        public SQLCEDatabaseProvider(Configuration configuration, Settings settings, ISqlFormatter sql_formatter) {
            _configuration = configuration;
            _settings = settings;
            _sql_formatter = sql_formatter;
        }

        public void UpdateMigrationVersion(int number) {
                ExecuteCommand(_sql_formatter.sql_update_schema_info(number));
        }

        public int ExecuteUpCommand(dynamic up) {
            var sql = _sql_formatter.GenerateSQLUp(up);
            using (var connection = GetOpenConnection()) {
                var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);

                var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                try {
                    foreach (var s in sql) {

                        if (_configuration.Verbose) {
                            Console.WriteLine("Executing SQL: " + s);
                        }
                        cmd.CommandText = s;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                } catch {
                    transaction.Rollback();
                    throw;
                } finally {
                    connection.Close();
                }
            }
            return sql.Length;
        }
        public int ExecuteDownCommand(dynamic down) {
            var sql = _sql_formatter.GenerateSQLDown(down);
            using (var connection = GetOpenConnection()) {
                var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                try {
                    foreach (var s in sql) {

                        if (_configuration.Verbose) {
                            Console.WriteLine("Executing SQL: " + s);
                        }
                        cmd.CommandText = s;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                } catch {
                    transaction.Rollback();
                    throw;
                } finally {
                    connection.Close();
                }
            }
            return sql.Length;
        }

        private int ExecuteCommand(string command) {
            if (string.IsNullOrWhiteSpace(command)) {
                return 0;
            }
            using (var connection = GetOpenConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    cmd.CommandText = command;
                    if (_configuration.Verbose) {
                        Console.WriteLine("Executing SQL: " + command);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        public int GetMigrationVersion() {
            try {
                using (var connection = GetOpenConnection()) {
                    using (var cmd = connection.CreateCommand()) {
                        cmd.CommandText = _sql_formatter.sql_get_schema_info;
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows && reader.Read()) {
                            var rv = reader.GetInt32(0);
                            reader.Close();
                            return rv;
                        }

                        reader.Close();
                        using (var cmd_init = connection.CreateCommand()) {
                            cmd_init.CommandText = _sql_formatter.sql_initialize_schema_info;
                            cmd.ExecuteNonQuery();
                        }
                        return 0;

                    }
                }
            } catch {
                EnsureSchemaInfo();
                return 0;
            }
        }


        private void EnsureSchemaInfo() {
            try {
                ExecuteCommand(_sql_formatter.sql_get_schema_info);
            } catch {
                ExecuteCommand(_sql_formatter.sql_create_schema_info_table);
                ExecuteCommand(_sql_formatter.sql_initialize_schema_info);
            }
        }


    }
}

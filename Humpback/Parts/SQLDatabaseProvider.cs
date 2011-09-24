using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using Humpback.Interfaces;
using Configuration = Humpback.ConfigurationOptions.Configuration;
using Settings = Humpback.ConfigurationOptions.Settings;

namespace Humpback.Parts {
    public class SQLDatabaseProvider : IDatabaseProvider {

        private Configuration _configuration;
        private Settings _settings;
        protected ISqlFormatter _sql_formatter;

        private SqlConnection GetOpenConnection() {
            var rv = new SqlConnection(_settings.ConnectionString());
            rv.Open();
            return rv;
        }
        public SQLDatabaseProvider(Configuration configuration, Settings settings, ISqlFormatter sql_formatter) {
            _configuration = configuration;
            _settings = settings;
            _sql_formatter = sql_formatter;
        }

        public void UpdateMigrationVersion(int number) {
                ExecuteCommand(_sql_formatter.sql_update_schema_info(number));
        }

        public virtual int ExecuteUpCommand(dynamic up) {

            var sql = _sql_formatter.GenerateSQLUp(up);
            // test for file
            bool has_filesmo = false;
            try {
                bool fsmo = up.up.filesmo != null;
                has_filesmo = fsmo;
            } catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) {
                // intentionally let thru, no smo object
            }
            if (has_filesmo) {
                ExecuteSmo(_settings.ConnectionString(), sql[0]);
                return 1;
            } else {
                using (var connection = GetOpenConnection()) {
                    var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);

                    var cmd = connection.CreateCommand();
                    cmd.Transaction = transaction;
                    try {
                        foreach (var s in sql) {

                            if (s.Contains("DROP TABLE")) {

                                if (_configuration.Verbose) {
                                    Console.WriteLine("DROPPING CONSTRAINTS BEFORE Executing SQL: " + s);
                                }

                                string table_name = s.Replace("DROP TABLE ", "").Trim();
                                foreach (var drop_string in DropTableConstraints(table_name, cmd)) {
                                    cmd.CommandText = drop_string;
                                    cmd.ExecuteNonQuery();
                                }

                            }


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
        }

        // due to dependency, kinda pulling this out on an as needed basis
        // this way if someone doenst' have smo, and doesnt try to call, never has an issue.
        private int ExecuteSmo(string connection_string, string sql) {
            // Smo.Executor.Execute(_settings.ConnectionString(), sql[0]);
            if(smo_assembly == null) {
                load_smo_assembly();
            }
            var Executor = smo_assembly.GetType("Humpback.Smo.Executor");
            var Execute = Executor.GetMethod("Execute");
            return (int)Execute.Invoke(null, new[] { connection_string, sql });
        }

        private void load_smo_assembly() {
            var _assembly = Assembly.GetExecutingAssembly();
            using(var resource_stream = _assembly.GetManifestResourceStream("Humpback.Artifacts.Humpback.Smo.dll")) {
                smo_assembly = Assembly.Load(ReadFully(resource_stream));
            }
        }

        // Thank you Jon Skeet!  http://stackoverflow.com/questions/221925/creating-a-byte-array-from-a-stream/221941#221941
        public static byte[] ReadFully(Stream input) {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream()) {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        private Assembly smo_assembly;

        public virtual int ExecuteDownCommand(dynamic down) {
            if(down.down == null) {
                return 0;
            }
            var sql = _sql_formatter.GenerateSQLDown(down);
            bool has_filesmo = false;
            try {
                bool fsmo = down.down.filesmo != null;
                has_filesmo = fsmo;
            } catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) {
                // intentionally let thru, no smo object
            }
            if (has_filesmo) {
                ExecuteSmo(_settings.ConnectionString(), sql[0]);
                return 1;
            }
            using (var connection = GetOpenConnection()) {
                var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
                var cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                try {
                    foreach (var s in sql) {
                        if (s.Contains("DROP TABLE")) {

                            if (_configuration.Verbose) {
                                Console.WriteLine("DROPPING CONSTRAINTS BEFORE Executing SQL: " + s);
                            }

                            string table_name = s.Replace("DROP TABLE ", "").Trim();
                            foreach (var drop_string in DropTableConstraints(table_name, cmd)) {
                                cmd.CommandText = drop_string;
                                cmd.ExecuteNonQuery();
                            }

                        }
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

        private IEnumerable<string> DropTableConstraints(string table_name, SqlCommand cmd) {
            string constraint_select =
                string.Format(
                    "SELECT a.name FROM sys.objects a INNER JOIN sys.objects b ON a.parent_object_id = b.object_id WHERE b.name = '{0}'",
                    table_name.Replace("[","").Replace("]",""));

            cmd.CommandText = constraint_select;
            var rv = new List<string>();
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    rv.Add(string.Format("ALTER TABLE {0} DROP CONSTRAINT [{1}]", table_name, reader[0]));
                }
                reader.Close();
            }
            return rv;
        }

        protected virtual int ExecuteCommand(string command) {
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;
using Humpback.Interfaces;

namespace Humpback.Parts {
    public class SQLServerFormatter : ISqlFormatter {

        private Configuration _configuration;
        private Settings _settings;

        private List<string> _commands_to_add;

        public SQLServerFormatter(Configuration configuration, Settings settings) {
            _configuration = configuration;
            _settings = settings;
            _commands_to_add = new List<string>();
        }

        public string[] GenerateSQLUp(dynamic operation) {
            if (operation.up != null) {
                return ((IEnumerable<string>)GetCommands(operation.up)).ToArray();
            }
            return new string[0];
        }

        public string[] GenerateSQLDown(dynamic operation) {
            if (operation.down != null) {
                return ((IEnumerable<string>)GetCommands(operation.down)).ToArray();
            }
            if (operation.up != null) {
                return ((IEnumerable<string>)GetReadMinds(operation.up)).Reverse().ToArray();
            }
            return new string[0];
        }


        public string sql_file_name(string p) {
            return p.Substring(0, p.Length - 3) + ".sql";
        }


        public string sql_create_schema_info_table {
            get { return "CREATE TABLE SchemaInfo (Version INT)"; }
        }

        public string sql_initialize_schema_info {
            get { return "INSERT INTO SchemaInfo(Version) VALUES(0)"; }
        }

        public string sql_update_schema_info(int version) {
            return String.Format("UPDATE SchemaInfo SET Version = {0}", version);
        }
        public string sql_get_schema_info {
            get { return "SELECT Version FROM SchemaInfo"; }
        }

        public string sql_fs_foreign_key {
            get {
                return "ALTER TABLE [{1}] ADD CONSTRAINT [{0}] FOREIGN KEY ([{2}]) REFERENCES [{3}] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;";
            }
        }

        public string sql_fs_foreign_key_name {
            get {
                return "FK_{0}_{1}_{2}";
            }
        }
        public string sql_fs_Timestamps {
            get {
                return "\n\t, CreatedOn datetime DEFAULT getutcdate() NOT NULL\n\t, UpdatedOn datetime DEFAULT getutcdate() NOT NULL";
            }
        }
        public string sql_fs_primary_key {
            get {
                return "Id int PRIMARY KEY IDENTITY(1,1) NOT NULL \n\t";
            }
        }

        public string sql_fs_create_table {
            get {
                return "CREATE TABLE [{0}]\r\n\t ({1}) ";
            }
        }

        public string sql_fs_drop_table {
            get {
                return "DROP TABLE [{0}]";
            }
        }

        public string sql_fs_alter_table_add_column {
            get {
                return "ALTER TABLE [{0}] ADD {1} ";
            }
        }
        public string sql_fs_alter_table_drop_column {
            get {
                return "ALTER TABLE [{0}] DROP COLUMN [{1}]";
            }
        }
        public string sql_fs_alter_table_alter_column {
            get {
                return "ALTER TABLE [{0}] ALTER COLUMN {1}";
            }
        }

        public string sql_fs_add_index {
            get {
                return "CREATE NONCLUSTERED INDEX [{0}] ON [{1}] ({2} )";
            }
        }
        public string sql_fs_drop_index {
            get {
                return "DROP INDEX [{0}].[{1}]";
            }
        }


        /// <summary>
        /// This is where the shorthand types are deciphered. Fix/love/tweak as you will
        /// </summary>
        static string SetColumnType(string colType) {

            if (!colType.Equals("datetime")) {
                colType = colType.Replace("date", "datetime");
            }

            return colType.Replace("pk", "int PRIMARY KEY NOT NULL IDENTITY(1,1)")
                .Replace("money", "decimal(16,2)")
                .Replace("long", "bigint")
                .Replace("string", "nvarchar(255)")
                .Replace("boolean", "bit")
                .Replace("text", "nvarchar(MAX)")
                .Replace("image", "varbinary(MAX)")
                .Replace("guid", "uniqueidentifier");
        }

        /// <summary>
        /// Build a list of columns from the past-in array in the JSON file
        /// </summary>
        private string BuildColumnList(string table_name, dynamic columns) {
            //holds the output
            var sb = new StringBuilder();
            var counter = 0;
            foreach (dynamic col in columns) {
                //name

                string column_name = col.name;
                string column_type = col.type;
                bool nullable = col.nullable ?? true;
                string default_value = col.@default ?? "";
                string extra_col_def = col.extra ?? "";

                if (col.type != null && col.type.ToLower() == "reference") {
                    string id_col_name = col.name + "Id";
                    column_name = id_col_name;
                    column_type = "INT";
                    string fk_name = string.Format(sql_fs_foreign_key_name, table_name.ToLower(), col.name.ToLower(), id_col_name.ToLower()); // FK_Orders_User_UserId
                    string fk = string.Format(sql_fs_foreign_key, fk_name, table_name, id_col_name, col.name);
                    _commands_to_add.Add(fk);
                    nullable = false;
                }


                sb.AppendFormat(", [{0}] ", column_name);

                //append on the type. Don't do this in the formatter since the replacer might return no change at all
                sb.Append(SetColumnType(column_type));

                //nullability - don't set if this is the Primary Key
                if (column_type != "pk") {
                    if (nullable) {
                        sb.Append(" NULL ");
                    } else {
                        sb.Append(" NOT NULL ");
                    }
                    if (!string.IsNullOrWhiteSpace(default_value)) {
                        sb.Append(" DEFAULT (" + default_value + ") ");
                    }
                    if (!string.IsNullOrWhiteSpace(extra_col_def)) {
                        sb.Append(extra_col_def);
                    }
                }

                counter++;
                //this format will indent the column
                if (counter < columns.Count) {
                    sb.Append("\r\n\t");
                }

            }
            return sb.ToString();
        }

        /// <summary>
        /// Strip out the leading comma. Wish there was a more elegant way to do this 
        /// and no, Regex doesn't count
        /// </summary>
        static string StripLeadingComma(string columns) {
            if (columns.StartsWith(", "))
                return columns.Substring(2, columns.Length - 2);
            return columns;
        }

        /// <summary>
        /// create unique name for index based on table and columns specified
        /// </summary>
        public string CreateIndexName(dynamic ix) {
            var sb = new StringBuilder();
            foreach (dynamic c in ix.columns) {
                sb.AppendFormat("{1}{0}", c.Replace(" ", "_"), (sb.Length == 0 ? "" : "_")); // ternary to only add underscore if not first iteration
            }
            return string.Format("IX_{0}_{1}", ix.table_name, sb.ToString());
        }

        /// <summary>
        /// create string for columns
        /// </summary>
        public string CreateIndexColumnString(dynamic columns) {
            var sb = new StringBuilder();
            foreach (dynamic c in columns) {
                sb.AppendFormat("{1} [{0}] ASC", c, (sb.Length == 0 ? "" : ",")); // ternary to only add comma if not first iteration
            }
            return sb.ToString();
        }


        // wrapped the get command to accomodate optional arrays of operations OR just a single op
        private IEnumerable<string> GetCommands(dynamic op) {
            _commands_to_add = new List<string>();
            if (op.Count != null) {
                foreach (var iter_op in op) {
                    yield return GetCommand(iter_op);
                }
            } else {
                yield return GetCommand(op);
            }
            if (_commands_to_add.Count > 0) { // post main commands here, i.e. FK's
                foreach (var cmd in _commands_to_add) {
                    yield return cmd;
                }
            }
        }


        private string GetCommand(dynamic op) {
            //the "op" here is an "up" or a "down". It's dynamic as that's what the JSON parser
            //will return. The neat thing about this parser is that the dynamic result will
            //return null if the key isn't present - so it's a simple null check for the operations/keys we need.
            //this will allow you to expand and tweak this migration stuff as you like

            var result = "";
            var pkName = "Id";
            //what are we doing?

            if (op == null)
                return "-- no DOWN specified. If this is a CREATE table or ADD COLUMN - it will be generated for you";

            if (op.GetType() == typeof(string))
                return SetColumnType(op).Replace("{", "").Replace("}", "");

            //CREATE
            if (op.create_table != null) {
                var columns = BuildColumnList(op.create_table.name, op.create_table.columns);

                //add some timestamps?
                if (op.create_table.timestamps != null) {
                    columns += sql_fs_Timestamps;
                }

                //make sure we have a PK :)
                if (!columns.Contains("PRIMARY KEY") & !columns.Contains("IDENTITY")) {
                    columns = sql_fs_primary_key + columns;
                } else {
                    foreach (var col in op.create_table.columns) {
                        if (col.type.ToString() == "pk") {
                            pkName = col.name;
                            break;
                        }
                    }
                }
                columns = StripLeadingComma(columns);
                result = string.Format(sql_fs_create_table, op.create_table.name, columns);

                //DROP 
            } else if (op.drop_table != null) {
                return string.Format(sql_fs_drop_table,op.drop_table);
                //ADD COLUMN
            } else if (op.add_column != null) {
                result = string.Format(sql_fs_alter_table_add_column, op.add_column.table, StripLeadingComma(BuildColumnList(op.add_column.table, op.add_column.columns)));
                //DROP COLUMN
            } else if (op.remove_column != null) {
                result = string.Format(sql_fs_alter_table_drop_column, op.remove_column.table, op.remove_column.column);
                //CHANGE
            } else if (op.change_column != null) {
                result = string.Format(sql_fs_alter_table_alter_column, op.change_column.table, StripLeadingComma(BuildColumnList(op.change_column.table, op.change_column.columns)));
                //ADD INDEX
            } else if (op.add_index != null) {
                result = string.Format(sql_fs_add_index, CreateIndexName(op.add_index), op.add_index.table_name, CreateIndexColumnString(op.add_index.columns));
                //REMOVE INDEX
            } else if (op.remove_index != null) {
                result = string.Format(sql_fs_drop_index, op.remove_index.table_name, CreateIndexName(op.remove_index));
            } else if (op.execute != null) {
                result = op.execute;
            } else if (op.file != null) {
                result = File.ReadAllText(Path.Combine(_settings.SqlFileFolder(), op.file));
            } else if (op.filesmo != null) {
                result = File.ReadAllText(Path.Combine(_settings.SqlFileFolder(), op.filesmo));
            }

            return result;
        }



        private IEnumerable<string> GetReadMinds(dynamic op) {
            if (op.Count != null) {
                foreach (var iter_op in op) {
                    yield return ReadMinds(iter_op);
                }
            } else {
                yield return ReadMinds(op);
            }
        }


        /// <summary>
        /// If a "down" isn't declared, this handy function will try and figure it out for you
        /// </summary>
        public string ReadMinds(dynamic up) {
            //CREATE
            if (up.create_table != null) {
                return string.Format(sql_fs_drop_table, up.create_table.name);
                //DROP COLUMN
            }
            if (up.add_column != null) {
                return string.Format(sql_fs_alter_table_drop_column, up.add_column.table, up.add_column.columns[0].name);
            }
            if (up.add_index != null) {
                // DROP INDEX
                return string.Format(sql_fs_drop_index, up.add_index.table_name, CreateIndexName(up.add_index));
            }
            return "";

        }
    }
}

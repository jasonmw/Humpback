using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class SQLServerFormatter : ISqlFormatter {

        private Configuration _configuration;

        public SQLServerFormatter(Configuration configuration) {
            _configuration = configuration;
        }

        public string[] GenerateSQLUp(dynamic operation) {
            if(operation.up != null) {
                return new string[] {GetCommand(operation.up)};
            }
            return new string[0];
        }

        public string[] GenerateSQLDown(dynamic operation) {
            if (operation.down != null) {
                return new string[] { GetCommand(operation.down) };
            }
            if (operation.up != null) {
                return new string[] { ReadMinds(operation.up) };
            }
            return new string[0];
        }


        public string SqlFileName(string p) {
            return p.Substring(0,p.Length - 3) + ".sql";
        }


        public string sqlCreateSchemaInfoTable {
            get { return "CREATE TABLE SchemaInfo (Version INT)"; }
        }

        public string sqlInitializeSchemaInfo {
            get { return "INSERT INTO SchemaInfo(Version) VALUES(0)"; }
        }

        public string sqlUpdateSchemaInfo(int version) {
            return String.Format("UPDATE SchemaInfo SET Version = {0}",version); 
        }
        public string sqlGetSchemaInfo {
            get { return "SELECT Version FROM SchemaInfo"; }
        }


        /// <summary>
        /// This is where the shorthand types are deciphered. Fix/love/tweak as you will
        /// </summary>
        static string SetColumnType(string colType) {

            return colType.Replace("pk", "int PRIMARY KEY NOT NULL IDENTITY(1,1)")
                .Replace("money", "decimal(16,2)")
                .Replace("date", "datetime")
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
        static string BuildColumnList(dynamic columns) {
            //holds the output
            var sb = new System.Text.StringBuilder();
            var counter = 0;
            foreach (dynamic col in columns) {
                //name
                sb.AppendFormat(", [{0}] ", col.name);

                //append on the type. Don't do this in the formatter since the replacer might return no change at all
                sb.Append(SetColumnType(col.type));

                //nullability - don't set if this is the Primary Key
                if (col.type != "pk") {
                    if (col.nullable != null) {
                        if (col.nullable) {
                            sb.Append(" NULL ");
                        } else {
                            sb.Append(" NOT NULL ");
                        }
                    } else {
                        sb.Append(" NULL ");
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
        static string CreateIndexName(dynamic ix) {
            var sb = new System.Text.StringBuilder();
            foreach (dynamic c in ix.columns) {
                sb.AppendFormat("{1}{0}", c.Replace(" ", "_"), (sb.Length == 0 ? "" : "_")); // ternary to only add underscore if not first iteration
            }
            return string.Format("IX_{0}_{1}", ix.table_name, sb.ToString());
        }

        /// <summary>
        /// create string for columns
        /// </summary>
        static string CreateIndexColumnString(dynamic columns) {
            var sb = new System.Text.StringBuilder();
            foreach (dynamic c in columns) {
                sb.AppendFormat("{1} [{0}] ASC", c, (sb.Length == 0 ? "" : ",")); // ternary to only add comma if not first iteration
            }
            return sb.ToString();
        }

        string GetCommand(dynamic op) {

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
                var columns = BuildColumnList(op.create_table.columns);

                //add some timestamps?
                if (op.create_table.timestamps != null) {
                    columns += "\n\t, CreatedOn datetime DEFAULT getdate() NOT NULL\n\t, UpdatedOn datetime DEFAULT getdate() NOT NULL";
                }

                //make sure we have a PK :)
                if (!columns.Contains("PRIMARY KEY") & !columns.Contains("IDENTITY")) {
                    columns = "Id int PRIMARY KEY IDENTITY(1,1) NOT NULL \n\t" + columns;
                } else {
                    foreach (var col in op.create_table.columns) {
                        if (col.type.ToString() == "pk") {
                            pkName = col.name;
                            break;
                        }
                    }
                }
                columns = StripLeadingComma(columns);
                result = string.Format("CREATE TABLE [{0}]\r\n\t ({1}) ", op.create_table.name, columns);

                //DROP 
            } else if (op.drop_table != null) {
                return "DROP TABLE " + op.drop_table;
                //ADD COLUMN
            } else if (op.add_column != null) {
                result = string.Format("ALTER TABLE [{0}] ADD {1} ", op.add_column.table, StripLeadingComma(BuildColumnList(op.add_column.columns)));
                //DROP COLUMN
            } else if (op.remove_column != null) {
                result = string.Format("ALTER TABLE [{0}] DROP COLUMN [{1}]", op.remove_column.table, op.remove_column.column);
                //CHANGE
            } else if (op.change_column != null) {
                result = string.Format("ALTER TABLE [{0}] ALTER COLUMN {1}", op.change_column.table, StripLeadingComma(BuildColumnList(op.change_column.columns)));
                //ADD INDEX
            } else if (op.add_index != null) {
                result = string.Format("CREATE NONCLUSTERED INDEX [{0}] ON [{1}] ({2} )", CreateIndexName(op.add_index), op.add_index.table_name, CreateIndexColumnString(op.add_index.columns));
                //REMOVE INDEX
            } else if (op.remove_index != null) {
                result = string.Format("DROP INDEX {0}.{1}", op.remove_index.table_name, CreateIndexName(op.remove_index));
            } else if (op.execute != null) {
                result = op.execute;
            } else if (op.file != null) {
                result = File.ReadAllText(Path.Combine(_configuration.SqlFolder, op.file));
            }

            return result;
        }

        /// <summary>
        /// If a "down" isn't declared, this handy function will try and figure it out for you
        /// </summary>
        static string ReadMinds(dynamic up) {
            //CREATE
            if (up.create_table != null) {
                return string.Format("DROP TABLE [{0}]", up.create_table.name);
                //DROP COLUMN
            }
            if (up.add_column != null) {
                return string.Format("ALTER TABLE [{0}] DROP COLUMN {1}", up.add_column, up.add_column.columns[0].name);
            }
            if (up.add_index != null) {
                // DROP INDEX
                return string.Format("DROP INDEX {0}.{1}", up.add_index.table_name, CreateIndexName(up.add_index));
            }
            return "";

        }
    }
}

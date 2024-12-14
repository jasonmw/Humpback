using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Humpback.Parts
{
    public class Generator : IHumpbackCommand
    {

        private readonly Configuration _configuration;
        private readonly IFileWriter _file_writer;
        private Settings _settings;

        public Generator(Configuration configuration, Settings settings, IFileWriter file_writer)
        {
            if (!configuration.Generate)
            {
                throw new InvalidOperationException("Configuration not correctly set for Generator");
            }
            _file_writer = file_writer;
            _configuration = configuration;
            _settings = settings;
            set_generation_action();
        }


        void set_generation_action()
        {

            var generate_string = _configuration.GenerateString;
            var generate_string_upper = generate_string.ToUpperInvariant();
            var ignore_case = StringComparison.InvariantCultureIgnoreCase; // sorry this was just big and ugly repeated below

            if (generate_string.Equals("SQL", ignore_case))
            {
                GenerateAction = GenerateActionType.Sql;
            }
            else if (generate_string.Equals("FILE", ignore_case))
            {
                GenerateAction = GenerateActionType.File;
            }
            else if (generate_string.Equals("SMO", ignore_case) || generate_string.Equals("FILESMO", ignore_case))
            {
                GenerateAction = GenerateActionType.FileSmo;
            }
            else if (generate_string_upper.StartsWith("ADD"))
            {


                // check for keywords
                if (generate_string_upper.StartsWith("ADDINDEXTO"))
                {
                    GenerateAction = GenerateActionType.AddIndex;
                }
                else
                {
                    GenerateAction = GenerateActionType.AddColumn;
                }
            }
            else if (generate_string_upper.StartsWith("REMOVE"))
            {

                // check for keywords
                if (generate_string_upper.StartsWith("REMOVEINDEXFROM"))
                {
                    GenerateAction = GenerateActionType.RemoveIndex;
                }
                else
                {
                    GenerateAction = GenerateActionType.RemoveColumn;
                }

            }
            else if (generate_string_upper.StartsWith("DROP"))
            {
                GenerateAction = GenerateActionType.RemoveTable;

            }
            else if (generate_string_upper.StartsWith("CHANGE"))
            {
                GenerateAction = GenerateActionType.ChangeColumn;

            }
            else
            {
                GenerateAction = GenerateActionType.AddTable;
            }
        }
        public GenerateActionType GenerateAction
        {
            get;
            private set;
        }


        public void Execute()
        {
            switch (GenerateAction)
            {
                case GenerateActionType.AddTable:
                    AddTable();
                    break;
                case GenerateActionType.RemoveTable:
                    RemoveTable();
                    break;
                case GenerateActionType.AddColumn:
                    AddColumn();
                    break;
                case GenerateActionType.ChangeColumn:
                    ChangeColumn();
                    break;
                case GenerateActionType.RemoveColumn:
                    RemoveColumn();
                    break;
                case GenerateActionType.AddIndex:
                    AddIndex();
                    break;
                case GenerateActionType.RemoveIndex:
                    RemoveIndex();
                    break;
                case GenerateActionType.Sql:
                    Sql();
                    break;
                case GenerateActionType.File:
                    File();
                    break;
                case GenerateActionType.FileSmo:
                    FileSmo();
                    break;
            }
        }

        private void ChangeColumn()
        {
            var nullables = new Dictionary<string, bool>();
            var defaults = new Dictionary<string, string>();
            var column_dictionary = GenerateColumns(nullables, defaults);

            IList<dynamic> columns = new List<dynamic>();

            foreach (var column in column_dictionary)
            {
                if (nullables.ContainsKey(column.Key) && defaults.ContainsKey(column.Key))
                {

                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key], @default = defaults[column.Key] });
                }

                else if (nullables.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key] });
                }
                else if (defaults.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, @default = defaults[column.Key] });
                }
                else
                {
                    columns.Add(new { name = column.Key, @type = column.Value });
                }

            }
            if (!columns.Any())
            {
                columns.Add(new { name = "column_name_here", @type = "string" });
            }

            var name = _configuration.GenerateString;
            dynamic up = new { change_column = new { name, columns } };
            dynamic down = new { change_column = new { name, columns } };
            dynamic output_object = new { up, down };
            CreateFile("ChangeColumn_" + name, output_object);
            Console.WriteLine("Generating Migration " + "ChangeColumn_" + name);
            Console.WriteLine(Helpers.Json(output_object));
        }


        private string CreateFile(string action, dynamic obj)
        {
            string serialized = Helpers.Json(obj);
            action = action.Replace(" ", "_");
            action = Regex.Replace(action, @"[^\w\-_]", "");
            string file_name = string.Format("{0}_{1}.js", _configuration.NextSerialNumber(), action);
            var path = Path.Combine(_settings.MigrationsFolder(), file_name);
            _file_writer.WriteFile(path, serialized);
            return path;
        }

        public void AddTable()
        {


            var nullables = new Dictionary<string, bool>();
            var defaults = new Dictionary<string, string>();
            var column_dictionary = GenerateColumns(nullables, defaults);

            IList<dynamic> columns = new List<dynamic>();

            foreach (var column in column_dictionary)
            {
                if (nullables.ContainsKey(column.Key) && defaults.ContainsKey(column.Key))
                {

                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key], @default = defaults[column.Key] });
                }
                else if (nullables.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key] });
                }
                else if (defaults.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, @default = defaults[column.Key] });
                }
                else
                {
                    columns.Add(new { name = column.Key, @type = column.Value });
                }

            }

            var name = _configuration.GenerateString;
            dynamic up = new { create_table = new { name, full_audit = true, columns } };
            dynamic down = new { drop_table = name };
            dynamic output_object = new { up, down };
            CreateFile("AddTable_" + name, output_object);
            Console.WriteLine("Generating Migration " + "AddTable_" + name);
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void RemoveTable()
        {
            var table_name = _configuration.GenerateString;
            if (table_name.ToUpperInvariant().StartsWith("DROP"))
            {
                table_name = table_name.Substring(4);
            }
            dynamic up = new { drop_table = table_name };
            dynamic output_object = new { up };
            CreateFile("DropTable_" + table_name, output_object);
            Console.WriteLine("Generating Migration " + "DropTable_" + table_name);
            Console.WriteLine(Helpers.Json(output_object));

        }

        public void AddColumn()
        {
            string table_name = "";
            string generate_string_upper = _configuration.GenerateString.ToUpperInvariant();
            int esc_to_count = Regex.Matches(generate_string_upper, "_TO_").Count;
            int to_count = Regex.Matches(generate_string_upper, "TO").Count;
            string column_name = "";
            if (esc_to_count == 1)
            {
                int ix = generate_string_upper.IndexOf("_TO_"); // TODO: substring matching issue: could be an issue here when this sequence is in the tablename, could go from end, but then column name has same issue.
                table_name = _configuration.GenerateString.Substring(ix + 4);
                column_name = _configuration.GenerateString.Substring(3, ix - 3);
            }
            else if (to_count > 1)
            {
                Console.WriteLine("Humpback is trying to help you.");
                Console.WriteLine(
                    "You Are attempting to add a column to a table, but the letters 'TO' occur in the column name or the table name");
                Console.WriteLine(
                    "You need to use _TO_ with underscores to explicitly specify which to to split the column and table names");
                Console.WriteLine("For instance AddCustomerName_TO_Orders");
                Console.WriteLine("Migration NOT Created");
                return;
            }
            else if (generate_string_upper.StartsWith("ADD") && generate_string_upper.Contains("TO"))
            {
                int ix = generate_string_upper.IndexOf("TO");
                table_name = _configuration.GenerateString.Substring(ix + 2);
                column_name = _configuration.GenerateString.Substring(3, ix - 3);
            }


            var nullables = new Dictionary<string, bool>();
            var defaults = new Dictionary<string, string>();
            var column_dictionary = GenerateColumns(nullables, defaults);

            IList<dynamic> columns = new List<dynamic>();

            foreach (var column in column_dictionary)
            {
                if (nullables.ContainsKey(column.Key) && defaults.ContainsKey(column.Key))
                {

                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key], @default = defaults[column.Key] });
                }
                else if (nullables.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, nullable = nullables[column.Key] });
                }
                else if (defaults.ContainsKey(column.Key))
                {
                    columns.Add(new { name = column.Key, @type = column.Value, @default = defaults[column.Key] });
                }
                else
                {
                    columns.Add(new { name = column.Key, @type = column.Value });
                }

            }
            if (!columns.Any())
            {
                if (generate_string_upper.StartsWith("ADD") && generate_string_upper.Contains("TO"))
                {
                    string s = _configuration.GenerateString.Substring(3);
                    int ix = s.ToUpperInvariant().IndexOf("TO");
                    columns.Add(new { name = column_name, @type = "string" });
                }
                else
                {
                    columns.Add(new { name = "add_column_name_here", @type = "string" });
                }
            }
            string drop_col_name = columns.First().name;
            if (columns.First().type == "reference")
            {
                drop_col_name += "Id";
            }
            var table = table_name; // used for auto property naming in anon object below
            dynamic up = new { add_column = new { table, columns } };
            dynamic down = new { remove_column = new { table, column = drop_col_name } }; // TODO: need to allow for dropping multiple columns
            dynamic output_object = new { up, down };
            CreateFile("AddColumn_" + table_name + "_" + drop_col_name, output_object);
            Console.WriteLine("Generating Migration " + "AddColumn_" + table_name + "_" + drop_col_name);
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void RemoveColumn()
        {
            string table_name = "";
            string column_name = "";
            if (_configuration.GenerateString.ToUpperInvariant().StartsWith("REMOVE")
                && _configuration.GenerateString.ToUpperInvariant().Contains("FROM"))
            {
                string s = _configuration.GenerateString.Substring(6);
                int ix = s.ToUpperInvariant().IndexOf("FROM");
                column_name = s.Substring(0, ix);
                table_name = s.Substring(ix + 4);
            }

            var table = table_name;// used for auto property naming in anon object below
            dynamic up = new { remove_column = new { table, column = column_name } }; // TODO: need to allow for dropping multiple columns
            dynamic output_object = new { up };
            CreateFile("RemoveColumn_" + table_name + "_" + column_name, output_object);
            Console.WriteLine("Generating Migration " + "RemoveColumn_" + table_name + "_" + column_name);
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void AddIndex()
        {
            string table_name = "";
            if (_configuration.GenerateString.ToUpperInvariant().StartsWith("ADDINDEXTO")
                && _configuration.GenerateString.Length > 10)
            {
                table_name = _configuration.GenerateString.Substring(10);
            }

            var column_dictionary = GenerateColumns();
            IList<string> columns = column_dictionary.Select(column => column.Key).ToList();
            if (!columns.Any())
            {
                columns.Add("column_name_here");
            }
            dynamic up = new { add_index = new { table_name, columns } };
            dynamic output_object = new { up };
            CreateFile("AddIndex_" + table_name + "_" + String.Join("_", columns), output_object);
            Console.WriteLine("Generating Migration " + "AddIndex_" + table_name + "_" + String.Join("_", columns));
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void RemoveIndex()
        {
            string table_name = "";
            string generate_string_upper = _configuration.GenerateString.ToUpperInvariant();
            if (generate_string_upper.StartsWith("REMOVEINDEXFROM") && generate_string_upper.Length > 15)
            {
                table_name = _configuration.GenerateString.Substring(15);
            }
            Dictionary<string, string> column_dictionary = GenerateColumns();
            IList<string> columns = column_dictionary.Select(column => column.Key).ToList();
            if (!columns.Any())
            {
                columns.Add("add_column_name_here");
            }
            dynamic up = new { remove_index = new { table_name, columns } };
            dynamic output_object = new { up };
            CreateFile("RemoveIndex_" + table_name + "_" + String.Join("_", columns), output_object);
            Console.WriteLine("Generating Migration " + "RemoveIndex_" + table_name + "_" + String.Join("_", columns));
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void Sql()
        {
            dynamic up = String.Join(" ", GenerateColumns().Keys);
            dynamic down = "";
            dynamic output_object = new { up, down };

            string sql_file_path = CreateFile("ExecuteSQL", output_object);

            Console.WriteLine("Generating Migration ExecuteSQL");
            Console.WriteLine(Helpers.Json(output_object));
        }

        public void File()
        {

            string sql_file_path_relative = "..\\sql\\" + String.Join("_", GenerateColumns().Keys);
            string sql_file_path = Path.Combine(_settings.MigrationsFolder(), sql_file_path_relative);

            if (!sql_file_path.ToUpperInvariant().EndsWith(".SQL"))
            {
                sql_file_path += ".sql";
                sql_file_path_relative += ".sql";
            }
            int counter = 0;
            while (_file_writer.FileExists(sql_file_path))
            { // TODO: This is messy and unreadable, come back and make nice.
                if (counter == 0)
                {
                    sql_file_path = sql_file_path.Replace(".sql", ++counter + ".sql");
                    sql_file_path_relative = sql_file_path_relative.Replace(".sql", counter + ".sql");
                }
                else
                {
                    sql_file_path = sql_file_path.Replace(counter++ + ".sql", counter + ".sql");
                    sql_file_path_relative = sql_file_path_relative.Replace((counter - 1) + ".sql", counter + ".sql");
                }
            }
            string text_path = sql_file_path_relative.Replace("..\\", "");
            dynamic up = new { file = sql_file_path_relative };
            dynamic output_object = new { up };
            _file_writer.WriteFile(sql_file_path, "-- Execute SQLFile Migration " + text_path);
            CreateFile("SQLFile_" + String.Join("_", GenerateColumns().Keys), output_object);
            SourceEditor.OpenFileInDefaultEditor(sql_file_path); // open sql file to edit right away.
            Console.WriteLine("Generating Migration " + text_path);
            Console.WriteLine(Helpers.Json(output_object));
        }
        public void FileSmo()
        {

            string sql_file_path_relative = "..\\sql\\" + String.Join("_", GenerateColumns().Keys);
            string sql_file_path = Path.Combine(_settings.MigrationsFolder(), sql_file_path_relative);

            if (!sql_file_path.ToUpperInvariant().EndsWith(".SQL"))
            {
                sql_file_path += ".sql";
                sql_file_path_relative += ".sql";
            }
            int counter = 0;
            while (_file_writer.FileExists(sql_file_path))
            { // TODO: This is messy and unreadable, come back and make nice.
                if (counter == 0)
                {
                    sql_file_path = sql_file_path.Replace(".sql", ++counter + ".sql");
                    sql_file_path_relative = sql_file_path_relative.Replace(".sql", counter + ".sql");
                }
                else
                {
                    sql_file_path = sql_file_path.Replace(counter++ + ".sql", counter + ".sql");
                    sql_file_path_relative = sql_file_path_relative.Replace((counter - 1) + ".sql", counter + ".sql");
                }
            }
            string text_path = sql_file_path_relative.Replace("..\\", "");
            dynamic up = new { filesmo = sql_file_path_relative };
            dynamic output_object = new { up };
            _file_writer.WriteFile(sql_file_path, "-- Execute SQLFile Migration " + text_path);
            CreateFile("SQLFile_" + String.Join("_", GenerateColumns().Keys), output_object);
            Console.WriteLine("Generating Migration " + text_path);
            Console.WriteLine(Helpers.Json(output_object));
        }



        /// <summary>
        /// Gets column references from the end of the cmd string
        /// sometimes used just for the names, and not the value (type)
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GenerateColumns(Dictionary<string, bool> nullable_values = null, Dictionary<string, string> default_values = null)
        {
            var rv = new Dictionary<string, string>(); // rv is return value.  don't hate for the acronym
            nullable_values = nullable_values ?? new Dictionary<string, bool>();
            default_values = default_values ?? new Dictionary<string, string>();

            var extra_array = _configuration.Extra.Except(new[] { _configuration.GenerateString });
            foreach (var item in extra_array)
            {
                if (item.Contains(":"))
                {
                    var parts = item.Trim().Split(":".ToCharArray());
                    rv.Add(parts[0].TrimEnd(), parts[1].TrimStart());
                    if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2]))
                    {
                        bool nullable = false;
                        if (bool.TryParse(parts[2].Trim(), out nullable))
                        {

                        }
                        else
                        {
                            nullable = true;
                        }
                        nullable_values.Add(parts[0].TrimEnd(), nullable);
                    }
                    if (parts.Length > 3 && !string.IsNullOrWhiteSpace(parts[3]))
                    {
                        default_values.Add(parts[0].TrimEnd(), parts[3].Trim());
                    }
                }
                else
                {
                    // default to string if they dont' specify type
                    rv.Add(item.Trim(), "string");
                }
            }
            return rv;
        }





    }
}
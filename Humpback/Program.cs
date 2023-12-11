using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Humpback.Parts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Humpback
{
    class Program
    {
        private static Configuration _configuration;
        private static IHumpbackCommand _humpback_command;
        private static IFileWriter _file_writer;
        private static IMigrationProvider _migration_provider;
        private static ISqlFormatter _sql_formatter;
        private static IDatabaseProvider _database_provider;
        private static Settings _settings;

        static int Main(string[] args)
        {
            try
            {
                _settings = Settings.Load();
                _file_writer = new FileWriter();
                _configuration = new Configuration(args);
                _sql_formatter = new SQLServerFormatter(_configuration, _settings);
                _database_provider = new SQLDatabaseProvider(_configuration, _settings, _sql_formatter);
                _migration_provider = new JsonFileMigrationProvider(_configuration, _settings, _database_provider);


                if (_configuration.WriteHelp)
                {
                    _humpback_command = new Help(_configuration);
                }
                else if (_configuration.Generate)
                {
                    Console.WriteLine("current project: " + _settings.CurrentProject);
                    _settings.EnsureDirectories();
                    _humpback_command = new Generator(_configuration, _settings, _file_writer);
                }
                else if (_configuration.List)
                {
                    _settings.EnsureDirectories();
                    _humpback_command = new MigrationViewer(_configuration, _migration_provider);
                }
                else if (_configuration.File)
                {
                    _settings.EnsureDirectories();
                    _humpback_command = new SourceEditor(_configuration, _migration_provider, new MigrationViewer(_configuration, _migration_provider));
                }
                else if (_configuration.Migrate)
                {
                    Console.WriteLine("current project: " + _settings.CurrentProject);
                    _settings.EnsureDirectories();
                    _humpback_command = new Migrate(_configuration, _database_provider, _migration_provider);
                }
                else if (_configuration.Sql)
                {
                    Console.WriteLine("current project: " + _settings.CurrentProject);
                    _settings.EnsureDirectories();
                    _humpback_command = new GenerateSQL(_configuration, _settings, _sql_formatter, _file_writer, _migration_provider);
                }
                else if (_configuration.Env)
                {
                    _humpback_command = new SettingsActions(_configuration, _settings);
                }

                _humpback_command.Execute();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                var inner = e.InnerException;
                while(inner != null)
                {
                    Console.WriteLine(inner.Message);
                    inner = inner.InnerException;
                }

                if (_configuration != null && _configuration.Verbose)
                {
                    Console.WriteLine(e.ToString());
                }

                if (Debugger.IsAttached)
                {
                    Console.ReadLine();
                }
                return -1;
            }
            if (Debugger.IsAttached && Environment.UserInteractive)
            {
                string[] inputs = ParseUserString(Console.ReadLine());
                if (inputs.Length > 0)
                {
                    Main(inputs);
                }
            }

            return 0;
        }

        private static string[] ParseUserString(string userString)
        {
            if (string.IsNullOrWhiteSpace(userString))
            {
                return new string[0];
            }

            int start = 0;
            bool quote = false;
            List<string> inputs = new List<string>();
            for (int i = 0; i < userString.Length; i++)
            {
                if (userString[i] == '"' || (!quote && userString[i] == ' '))
                {
                    if (i - start > 0)
                    {
                        inputs.Add(userString.Substring(start, i - start));
                    }

                    start = i + 1;
                }

                if (userString[i] == '"')
                {
                    quote = !quote;
                }
            }

            if (userString.Length - start > 0)
            {
                inputs.Add(userString.Substring(start, userString.Length - start));
            }

            return inputs.ToArray();
        }

    }
}

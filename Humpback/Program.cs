using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Humpback.Parts;
using System;
using System.Diagnostics;

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
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.Message);
                    if (e.InnerException.InnerException != null)
                    {
                        Console.WriteLine(e.InnerException.InnerException.Message);
                    }
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
            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }
            return 0;
        }

    }
}

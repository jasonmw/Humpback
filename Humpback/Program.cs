using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;
using Humpback.Parts;

namespace Humpback {
    class Program {
        private static Configuration _configuration;
        private static IHumpbackCommand _humpback_command;
        private static IFileWriter _file_writer;
        private static IMigrationProvider _migration_provider;
        private static ISqlFormatter _sql_formatter;
        private static IDatabaseProvider _database_provider;
        private static Settings _settings;

        static void Main(string[] args) {
            try {
                _settings = Settings.Load();
                _file_writer = new FileWriter();
                _configuration = new Configuration(args);
                _sql_formatter = new SQLServerFormatter(_configuration,_settings);
                _database_provider = new SQLDatabaseProvider(_configuration, _settings, _sql_formatter);
                _migration_provider = new FileMigrationProvider(_configuration, _settings, _database_provider);

                
                if (_configuration.WriteHelp) {
                    _humpback_command = new Help(_configuration);
                } else if (_configuration.Generate) {
                    Console.WriteLine("current project: " + _settings.current_project);
                    _humpback_command = new Generator(_configuration, _settings, _file_writer);
                } else if (_configuration.List) {
                    _humpback_command = new MigrationViewer(_configuration,_migration_provider);
                } else if (_configuration.Migrate) {
                    Console.WriteLine("current project: " + _settings.current_project);
                    _humpback_command = new Migrate(_configuration, _database_provider, _migration_provider);
                } else if (_configuration.Sql) {
                    Console.WriteLine("current project: " + _settings.current_project);
                    _humpback_command = new GenerateSQL(_configuration, _settings, _sql_formatter, _file_writer, _migration_provider);
                } else if(_configuration.Env) {
                    _humpback_command = new SettingsActions(_configuration, _settings);
                }

                _humpback_command.Execute();

            } catch (Exception e)  {
                Console.WriteLine(e.ToString());
                //File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "errorlog.txt"), e.ToString() + Environment.NewLine + Environment.NewLine);
                Console.ReadKey();
            }
        }

    }
}

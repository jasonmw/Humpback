using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static void Main(string[] args) {
            try {

                _configuration = new Configuration(args);
                _file_writer = new FileWriter();
                _migration_provider = new FileMigrationProvider(_configuration);

                if (_configuration.WriteHelp) {
                    _humpback_command = new Help(_configuration);
                } else if (_configuration.Generate) {
                    _humpback_command = new Generator(_configuration,_file_writer);
                } else if (_configuration.List) {
                    _humpback_command = new MigrationViewer(_migration_provider);
                } else if (_configuration.Run) {
                    Run();
                } else if (_configuration.Sql) {
                    Sql();
                }

                _humpback_command.Execute();

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            DebugWaitAtEnd();

        }

        [Conditional("DEBUG")]
        private static void DebugWaitAtEnd() {
           // Console.ReadLine();
        }

        private static void Run() {
            
        }
        private static void Sql() {
            
        }
    }
}

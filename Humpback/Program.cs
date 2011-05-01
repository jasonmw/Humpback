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
        private static IPart part;
        static void Main(string[] args) {
            try {

                _configuration = new Configuration(args);

                if (_configuration.WriteHelp) {
                    part = new Help(_configuration);
                } else if (_configuration.Generate) {
                    part = new Generator(_configuration);
                } else if (_configuration.List) {
                    part = new ListMigrations(_configuration);
                } else if (_configuration.Run) {
                    Run();
                } else if (_configuration.Sql) {
                    Sql();
                }

                part.Execute();

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            DebugWaitAtEnd();

        }

        [Conditional("DEBUG")]
        private static void DebugWaitAtEnd() {
           // Console.ReadLine();
        }

        private static void PrintHelp() {
            
        }
        private static void List() {
            
        }
        private static void Run() {
            
        }
        private static void Sql() {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Humpback {
    class Program {
        private static ConfigurationOptions.Configuration configuration;
        static void Main(string[] args) {

            configuration = new ConfigurationOptions.Configuration(args);

            if(configuration.WriteHelp) {
                PrintHelp();
            } else if(configuration.Generate) {
                Generate();
            } else if (configuration.List) {
                List();
            } else if (configuration.Run) {
                Run();
            } else if (configuration.Sql) {
                Sql();
            }

            DebugWaitAtEnd();

        }

        [Conditional("DEBUG")]
        private static void DebugWaitAtEnd() {
            Console.ReadLine();
        }

        private static void PrintHelp() {
            
        }
        private static void Generate() {
            
        }
        private static void List() {
            
        }
        private static void Run() {
            
        }
        private static void Sql() {
            
        }
    }
}

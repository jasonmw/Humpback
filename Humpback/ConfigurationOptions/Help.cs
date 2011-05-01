using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.ConfigurationOptions {
    public class Help :IPart {
        private Configuration _configuration;
        public Help(Configuration configuration) {
            _configuration = configuration;
        }

        


        public void Execute() {
            switch(_configuration.HelpPage) {
                case HelpSection.Generate:
                    break;
                case HelpSection.List:
                    break;
                case HelpSection.Run:
                    break;
                case HelpSection.Sql:
                    break;
                default:
                    WriteGeneralHelp();
                    break;
            }
        }

        private void WriteGeneralHelp() {
            Console.WriteLine(@"
Humpback Migration Help
=============================

Main Modes
  -generate -G | Generate JSON migration files
  -list     -L | List Migrations and current migration state
  -run      -R | Run Migrations against database
  -sql      -S | Generate SQL files from migration files

For help about a specific topic use the following commands

hump -?:generate
hump -?:list
hump -?:run
hump -?:sql

");
        }
    }
}

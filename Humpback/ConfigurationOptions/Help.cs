using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.ConfigurationOptions {
    public class Help : IHumpbackCommand {

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
                    write_general_help();
                    break;
            }
        }

        private static void write_general_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================

Main commands
  -generate -G | Generate JSON migration files
  -list     -L | List Migrations and current migration state
  -run      -R | Run Migrations against database
  -sql      -S | Generate SQL files from migration files


For information about a specific command use the following

  hump -?:generate
  hump -?:list
  hump -?:run
  hump -?:sql

"); // TODO: work out a way to not require the : in the help for a specific command
        }
    }
}

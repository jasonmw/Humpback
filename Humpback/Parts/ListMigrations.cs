using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class ListMigrations : IPart {
        
        private readonly Configuration _configuration;
        public ListMigrations(Configuration configuration) {
            if(!configuration.List) {
                throw new InvalidOperationException("Configuration not correctly set for List");
            }
            _configuration = configuration;
            
        }
        private SortedDictionary<string, string> _migrations;
        private SortedDictionary<string,string> Migrations {
            get {
                if(_migrations == null) {
                    _migrations = new SortedDictionary<string, string>();
                    foreach(string file in Directory.GetFiles(_configuration.MigrationFolder)) {
                        _migrations.Add(file, file);
                    }
                }
                return _migrations;
            }
        }

        public void Execute() {
            Console.WriteLine("===============================");
            Console.WriteLine("Humpback Migration List");
            var migrations = Migrations;
            Console.WriteLine(
                @"
       __________...----..____..-'``-..___
     ,'.                                  ```--.._
    :                                             ``._
    |                           --                    ``.
    |                <o>   -.-      -.     -   -.        `.
    :                     __           --            .     \
     `._____________     (  `.   -.-      --  -   .   `     \
        `-----------------\   \_.--------..__..--.._ `. `.   :
                           `--'     SSt             `-._ .   |
                                                        `.`  |
                                                          \` |
                                                           \ |
                                                           / \`.
                                                          /  _\-'
                                                         /_,'
");
            Console.WriteLine(migrations.Count + " Migrations Total");
            Console.WriteLine("===============================");
            // TODO: Get Current Migration Status for line updates
            int migrationCounter = 1;
            foreach(var m in migrations) {
                Console.WriteLine("* {0} {1}", ((migrationCounter++).ToString()).PadLeft(3), new FileInfo(m.Value).Name);

            }
        }
    }
}

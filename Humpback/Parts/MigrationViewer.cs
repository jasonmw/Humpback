using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class MigrationViewer : IHumpbackCommand {
        
        private IMigrationProvider _migration_provider;
        private Configuration _configuration;
        public MigrationViewer(Configuration configuration, IMigrationProvider migration_provider) {
            _migration_provider = migration_provider;
            _configuration = configuration;
        }

        private void ExecuteList() {
            Console.WriteLine("===============================");
            Console.WriteLine("Humpback Migration List");
            var migrations = _migration_provider.GetMigrations();
            Console.WriteLine(// Couldn't help myself, leaving in the 'SSt' is enough for &copy; stuff  http://www.ascii-art.net/about.php
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
"); // yes i know thats not actually a humpback whale, its a sperm whale, but for now thats too bad, theres not a terrible lot in ascii whale photos to choose from.
    // also, if user doesnt' have monospaced font in their shell, it wont render right.   bummer for them!
            Console.WriteLine(migrations.Count + 
@" Migrations              \o/ means it is currently deployed
=================================================================");
            int db_version = _migration_provider.DatabaseMigrationNumber();
            foreach(var m in migrations) {
                Console.WriteLine("{0} {1} {2}", (m.Key > db_version ? "   " : "\\o/"), ((m.Key).ToString()).PadLeft(3), new FileInfo(m.Value).Name);

            }
        }
                
        public void Execute() {

            int migration_id = 0;
            bool has_extra = _configuration.Extra.Any();
            bool extra_is_int = has_extra && int.TryParse(_configuration.Extra.First(), out migration_id);
            bool migration_exists = extra_is_int && _migration_provider.GetMigrations().ContainsKey(migration_id);
            if(migration_exists) {

                ExecuteDetail(migration_id);
            } else {
                ExecuteList();
            }
        }

        private void ExecuteDetail(int migration_id) {
            
            Console.WriteLine(@"
 |                <o>   -.-  
 :                     __    
  `._____________     (  `.  SSt
     `-----------------\   \_
                        `--' 
");
            var migration = _migration_provider.GetMigrationWithContents(migration_id);
            Console.WriteLine(new FileInfo(migration.Key).Name);
            if (migration_id > _migration_provider.DatabaseMigrationNumber()) {
                Console.WriteLine("Migration is NOT currently deployed");
            } else {
                Console.WriteLine("Migration IS currently deployed \\o/");
            }
            Console.WriteLine(    "====================================");
            Console.WriteLine(migration.Value);
            Console.WriteLine();
        }
    }
}

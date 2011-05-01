using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class MigrationViewer : IHumpbackCommand {
        
        private IMigrationProvider _migration_provider;
        public MigrationViewer(IMigrationProvider migration_provider) {
            _migration_provider = migration_provider;
        }

        public void Execute() {
            Console.WriteLine("===============================");
            Console.WriteLine("Humpback Migration List");
            var migrations = _migration_provider.GetMigrations();
            Console.WriteLine(// Couldn't help myself, i hope leaving in the 'SSt' is enough for &copy; stuff??
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
"); // yes i know thats not actually a humpback whale, its a sperm whale, but for now thats too bad, theres not a terrible lot in ascii photos to choose from.
    // also, if user doesnt' have monospaced font in their shell, it wont render right.   bummer for them!
            Console.WriteLine(migrations.Count + @" Migrations Total     \o/ (yay) means its in your db already
=================================================================");
            int db_version = _migration_provider.DatabaseMigrationNumber();
            foreach(var m in migrations) {
                Console.WriteLine("{0} {1} {2}", (m.Key > db_version ? "   " : "\\o/"), ((m.Key).ToString()).PadLeft(3), new FileInfo(m.Value).Name);

            }
        }
    }
}

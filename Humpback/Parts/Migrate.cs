using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humpback.Interfaces;

namespace Humpback.Parts {
    public class Migrate:IHumpbackCommand {
         
        private IDatabaseProvider _database_provider;
        private IMigrationProvider _migration_provider;
        private ConfigurationOptions.Configuration _configuration;

        public Migrate(ConfigurationOptions.Configuration configuration, IDatabaseProvider database_provider, IMigrationProvider migration_provider) {
            _configuration = configuration;
            _database_provider = database_provider;
            _migration_provider = migration_provider;
        }

//      > hump -m -all    | updates database to most recent migration
//      > hump -m 12      | updates database to a specific migration (up or down)
//      > hump -m -up     | migrates database up one migration
//      > hump -m -down   | migrates database down one migration
//      > hump -m -empty  | removes all migrations from database
//      > hump -m -reset  | removes and re-adds all migrations (-empty, then -all)


        public void Execute() {
            int top_migration = _migration_provider.GetMigrations().Max(m => m.Key);
            int current_migration = _migration_provider.DatabaseMigrationNumber();
            if(current_migration > top_migration) {
                current_migration = top_migration;
            }

            if(_configuration.All) {
                MigrateTo(top_migration);
            } else if (_configuration.Up) {
                MigrateTo(++current_migration);
            } else if (_configuration.Down) {
                MigrateTo(--current_migration);
            } else if (_configuration.Empty) {
                MigrateTo(0);
            } else if (_configuration.Reset) {
                MigrateTo(0);
                MigrateTo(top_migration);
            } else if (SingleMigrationID() > 0) {
                MigrateTo(SingleMigrationID());
            } else {
                MigrateTo(top_migration);
            }

        }

        private void MigrateTo(int migration) {

            var migrations = _migration_provider.GetMigrations();
            int current = _migration_provider.DatabaseMigrationNumber();

            if (migrations.Count < current) {
                current = migrations.Count;
            }
            if(migration > migrations.Max(m => m.Key) || migration < 0) {
                Console.WriteLine("Invalid Migration");
                return;
            }
            if (migration == current) {
                Console.WriteLine("Nothing to be done.");
                return;
            }

            while(current < migration) {
                // go Up
                KeyValuePair<string, string> migration_kvp = _migration_provider.GetMigrationWithContents(++current);
                string migration_json = migration_kvp.Value;
                dynamic migration_object = Helpers.DeserializeMigration(migration_json);
                Console.WriteLine("Executing " +current+". Executing Migration " + migration_kvp.Key);
                _database_provider.ExecuteUpCommand(migration_object);
                _database_provider.UpdateMigrationVersion(current);
            }
            while(current > migration && current > 0) {
                // go down
                KeyValuePair<string, string> migration_kvp = _migration_provider.GetMigrationWithContents(current);
                string migration_json = migration_kvp.Value;
                dynamic migration_object = Helpers.DeserializeMigration(migration_json);
                Console.WriteLine("Removing " + (current) + ". Executing Migration " + migration_kvp.Key);
                _database_provider.ExecuteDownCommand(migration_object);
                _database_provider.UpdateMigrationVersion(--current);
            }
        }


        private int SingleMigrationID() {
            int migration_id = 0;
            bool has_extra = _configuration.Extra.Any();
            bool extra_is_int = has_extra && int.TryParse(_configuration.Extra.First(), out migration_id);
            bool migration_exists = extra_is_int && _migration_provider.GetMigrations().ContainsKey(migration_id);
            return migration_exists ? migration_id : 0;
        }
    }
}

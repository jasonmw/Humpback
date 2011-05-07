using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class FileMigrationProvider : IMigrationProvider {
        private Configuration _configuration;
        private IDatabaseProvider _database_provider;
        private SortedDictionary<int, string> _migrations;
        private SortedDictionary<int, string> _migration_contents;
        private Settings _settings;

        public FileMigrationProvider(Configuration configuration, Settings settings, IDatabaseProvider database_provider) {
            _configuration = configuration;
            _settings = settings;
            _database_provider = database_provider;
        }

        public SortedDictionary<int, string> GetMigrations() {
            if (_migrations == null) {
                _migrations = new SortedDictionary<int, string>();
                var tmp_migrations = new SortedDictionary<string, string>();
                foreach (string file in Directory.GetFiles(_settings.MigrationsFolder())) {
                    if (new FileInfo(file).Extension.Equals(".js", StringComparison.InvariantCultureIgnoreCase)) {
                        tmp_migrations.Add(file, file);
                    }
                }
                int ctr = 1;
                foreach(var key in tmp_migrations.Keys) {
                    _migrations.Add(ctr++, key);
                }
            }
            return _migrations;
        }

        public SortedDictionary<int, string> GetMigrationsContents() {
            if (_migration_contents == null) {
                _migration_contents = new SortedDictionary<int, string>();
                foreach (var kvp in GetMigrations()) { // kvp is key value pair.
                    _migration_contents.Add(kvp.Key, File.ReadAllText(kvp.Value));
                }
            }
            return _migration_contents;
        }

        public KeyValuePair<string, string> GetMigrationWithContents(int migration_number) {
            string filename = GetMigrations()[migration_number];
            var contents = "";
            if(_migration_contents == null) {
                contents = File.ReadAllText(filename);
            } else {
                contents = GetMigrationsContents()[migration_number];
            }

            return new KeyValuePair<string, string>(filename, contents);
        }

        public int DatabaseMigrationNumber() {
            return _database_provider.GetMigrationVersion();
        }

        public void SetMigrationNumber(int number) {
            _database_provider.UpdateMigrationVersion(number);
        }
    }
}

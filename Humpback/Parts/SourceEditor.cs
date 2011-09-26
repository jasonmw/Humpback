using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Microsoft.Win32;

namespace Humpback.Parts {
    public class SourceEditor : IHumpbackCommand {

        private IMigrationProvider _migration_provider;
        private Configuration _configuration;
        private MigrationViewer list_viewer;
        public SourceEditor(Configuration configuration, IMigrationProvider migration_provider, MigrationViewer viewer) {
            _migration_provider = migration_provider;
            _configuration = configuration;
            list_viewer = viewer;
        }


        public void Execute() {

            int migration_id = 0;
            bool has_extra = _configuration.Extra.Any();
            bool extra_is_int = has_extra && int.TryParse(_configuration.Extra.First(), out migration_id);
            bool migration_exists = extra_is_int && _migration_provider.GetMigrations().ContainsKey(migration_id);
            if (migration_exists) {
                var migration = _migration_provider.GetMigrationWithContents(migration_id);
                var file = new FileInfo(migration.Key);
                Console.WriteLine("Opening {0} in the default editor.",file.FullName);
                var text_editor = (string)Registry.GetValue(@"HKEY_CLASSES_ROOT\txtfile\shell\open\command\", "", "notepad.exe");
                text_editor = text_editor.Replace("\"%1\"","").Trim();
                System.Diagnostics.Process.Start(text_editor, file.FullName);
            } else {
                Console.WriteLine("Please specify the number of a valid migration.");
                _configuration.Extra = new List<string>();
                list_viewer.Execute();
            }
        }
    }
}

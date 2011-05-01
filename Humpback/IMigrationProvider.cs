using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback {
    public interface IMigrationProvider {
        SortedDictionary<int, string> GetMigrations();
        SortedDictionary<int, string> GetMigrationsContents();
        KeyValuePair<string, string> GetMigrationWithContents(int migration_number);
        int DatabaseMigrationNumber();
    }
}

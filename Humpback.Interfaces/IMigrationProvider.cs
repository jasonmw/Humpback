using System.Collections.Generic;

namespace Humpback.Interfaces
{
    public interface IMigrationProvider
    {
        SortedDictionary<int, string> GetMigrations();
        SortedDictionary<int, string> GetMigrationsContents();
        KeyValuePair<string, string> GetMigrationWithContents(int migration_number);
        int DatabaseMigrationNumber();
        void SetMigrationNumber(int number);
    }
}

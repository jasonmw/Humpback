namespace Humpback.Interfaces
{
    public interface IDatabaseProvider
    {

        int GetMigrationVersion();
        void UpdateMigrationVersion(int number);
        int ExecuteUpCommand(dynamic up);
        int ExecuteDownCommand(dynamic down);
    }
}

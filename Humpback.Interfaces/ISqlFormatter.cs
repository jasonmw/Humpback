namespace Humpback.Interfaces
{
    public interface ISqlFormatter
    {
        string[] GenerateSQLUp(dynamic operation);
        string[] GenerateSQLDown(dynamic operation);
        string sql_file_name(string p);
        string sql_create_schema_info_table { get; }
        string sql_initialize_schema_info { get; }
        string sql_get_schema_info { get; }
        string sql_update_schema_info(int version);
    }
}

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

namespace Humpback.Smo
{
    public static class Executor
    {

        public static int Execute(string connection_string, string sql)
        {
            using (var conn = new SqlConnection(connection_string))
            {
                var serverConnection = new ServerConnection(connection_string);
                var server = new Server(serverConnection);
                conn.Open();
                return server.ConnectionContext.ExecuteNonQuery(sql);
            }
        }
    }
}

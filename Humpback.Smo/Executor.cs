using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Humpback.Smo
{
    public static class Executor
    {

        public static int Execute(string connection_string, string sql)
        {
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connection_string);
            Server server = new Server(new ServerConnection(connection));
            return server.ConnectionContext.ExecuteNonQuery(sql);
        }
    }
}

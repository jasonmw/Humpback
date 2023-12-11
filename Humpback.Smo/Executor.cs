using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Humpback.Smo
{
    public static class Executor
    {
        /// <summary>
        /// Execute TSQL strings using Microsoft.SqlServer.Management.Smo.
        /// </summary>
        /// <param name="connection_string">Config connection string.</param>
        /// <param name="sql">Compiled TSQL script.</param>
        /// <returns>number of records affected.</returns>
        public static int Execute(string connection_string, string sql)
        {
            var connection = new Microsoft.Data.SqlClient.SqlConnection(connection_string);
            Server server = new Server(new ServerConnection(connection));
            return server.ConnectionContext.ExecuteNonQuery(sql);
        }
    }
}

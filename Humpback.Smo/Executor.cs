using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Humpback.Smo {
    public static class Executor {

        public static int Execute(string connection_string, string sql) {
            using (var conn = new SqlConnection(connection_string)) {
                var serverConnection = new ServerConnection(connection_string);
                var server = new Server(serverConnection);
                conn.Open();
                return server.ConnectionContext.ExecuteNonQuery(sql);
            }
        }
    }
}

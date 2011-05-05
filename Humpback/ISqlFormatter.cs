using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback {
    public interface ISqlFormatter {
        string[] GenerateSQLUp(dynamic operation);
        string[] GenerateSQLDown(dynamic operation);


        string SqlFileName(string p);
        string sqlCreateSchemaInfoTable { get; }
        string sqlInitializeSchemaInfo { get; }
        string sqlGetSchemaInfo { get; }
        string sqlUpdateSchemaInfo(int version);
    }
}

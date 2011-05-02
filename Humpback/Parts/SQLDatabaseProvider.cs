using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.Parts {
    public class SQLDatabaseProvider : IDatabaseProvider {

        public bool ExecuteCommand(string command) {
            throw new NotImplementedException();
        }

        public int GetMigrationVersion() {
            throw new NotImplementedException();
        }

        public bool EnsureSchemaInfo() {
            throw new NotImplementedException();
        }
    }
}

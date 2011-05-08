using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback {
    public interface IDatabaseProvider {

        int GetMigrationVersion();
        void UpdateMigrationVersion(int number);
        int ExecuteUpCommand(dynamic up);
        int ExecuteDownCommand(dynamic down);
    }
}

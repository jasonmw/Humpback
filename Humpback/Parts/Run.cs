using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.Parts {
    public class Run:IHumpbackCommand {
        
        private ISqlFormatter _sql_formatter;
        private IDatabaseProvider _database_provider;
        private ConfigurationOptions.Configuration _configuration;

        public Run(ConfigurationOptions.Configuration configuration, ISqlFormatter sql_formatter, IDatabaseProvider database_provider) {
            _configuration = configuration;
            _sql_formatter = sql_formatter;
            _database_provider = database_provider;
        }


        public void Execute() {
            throw new NotImplementedException();
        }
    }
}

using Humpback.ConfigurationOptions;
using Humpback.Interfaces;
using Humpback.Parts;

namespace Humpback.Tests.Impl
{
    public class TestSQLDatabaseProvider : SQLDatabaseProvider
    {
        public TestSQLDatabaseProvider(Configuration configuration, Settings settings, ISqlFormatter sql_formatter) : base(configuration, settings, sql_formatter)
        {
        }

        protected override int ExecuteCommand(string command)
        {
            LastCommand = command;
            return 0;
        }

        public string LastCommand { get; private set; }


        public new virtual int ExecuteUpCommand(dynamic up)
        {

            var sql = _sql_formatter.GenerateSQLUp(up);
            // test for file
            bool has_filesmo = false;
            try
            {
                bool fsmo = up.up.filesmo != null;
                has_filesmo = true;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                // intentionally let thru, no smo object
            }
            if (has_filesmo)
            {
                LastCommand = sql[0];
            }
            else
            {

                foreach (var s in sql)
                {

                    LastCommand += s;
                }
            }
            return 1;
        }

        public new virtual int ExecuteDownCommand(dynamic down)
        {

            var sql = _sql_formatter.GenerateSQLDown(down);
            // test for file
            bool has_filesmo = false;
            try
            {
                bool fsmo = down.down.filesmo != null;
                has_filesmo = true;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                // intentionally let thru, no smo object
            }
            if (has_filesmo)
            {
                LastCommand = sql[0];
            }
            else
            {

                foreach (var s in sql)
                {

                    LastCommand += s;
                }
            }
            return 1;
        }

    }
}

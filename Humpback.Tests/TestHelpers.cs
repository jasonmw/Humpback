using Humpback.ConfigurationOptions;
using System;
using System.IO;

namespace Humpback.Tests
{
    public static class TestHelpers
    {
        public static Settings TestSettings
        {
            get
            {
                var project = new Project
                {
                    name = "default",
                    connection_string = "server=.;database=northwind;Integrated Security=True;",
                    directory = Path.Combine(Environment.CurrentDirectory, "db"),
                    flavor = "sqlserver"
                };
                return new Settings("default",
                                    new[] { project });

            }
        }
    }
}

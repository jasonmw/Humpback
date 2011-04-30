using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Humpback.ConfigurationOptions;
using System.Configuration;

namespace Humpback.ConfigurationOptions {
    public class Configuration {

        private void Parse(IEnumerable<string> options) {
            var oset = new OptionSet()
                .Add("?:|help:|h:|HELP:|H:|Help:", SetSpecificHelpPage)
                .Add("G:|gen:|g:|GEN:|Gen:|Generate:|generate:|GENERATE:", s => { SetMainToFalse(); Generate = true; })
                .Add("L:|list:|l:|LIST:|List", s => { SetMainToFalse(); List = true; })
                .Add("R:|run:|r:|RUN:|Run:", s => { SetMainToFalse(); Run = true; })
                .Add("S:|sql:|s:|SQL:|Sql", s => { SetMainToFalse(); Sql = true; });
            oset.Parse(options);
        }




        public Configuration() {
            ResetOptions();
        }

        public Configuration(IEnumerable<string> options):this() {
            if(options==null || options.Count() == 0) {
                return;
            }
            DebugWriteOutOptions(options);
            Parse(options);
        }

        [Conditional("DEBUG")]
        private void DebugWriteOutOptions(IEnumerable<string> options) {
            foreach(string s in options) {
                Debug.WriteLine("Option: " + s);
                Trace.WriteLine("Option: " + s);
                Console.WriteLine("Option: " + s);
            }
        }


        private void AssignDefaultConnectionString() {
            ConnectionString = ConfigurationManager.ConnectionStrings["default"] == null
                                   ? "server=.;database=default;Integrated Security=True;"
                                   : ConfigurationManager.ConnectionStrings["default"].ConnectionString;
        }

        private void SetSpecificHelpPage(string v) {
            SetMainToFalse();
            WriteHelp = true;
            if(String.IsNullOrWhiteSpace(v)) {
                HelpPage = HelpSection.All;
                return;
            }
            switch(v.ToUpperInvariant().Trim()) {
                case "G":
                case "GENERATE":
                case "GEN":
                    HelpPage = HelpSection.Generate;
                    break;
                case "L":
                case "LIST":
                    HelpPage = HelpSection.List;
                    break;
                case "R":
                case "RUN":
                    HelpPage = HelpSection.Run;
                    break;
                case "S":
                case "SQL":
                    HelpPage = HelpSection.Sql;
                    break;
            }
        }

        private void ResetOptions() {
            WriteHelp = false;
            HelpPage = HelpSection.All;
            AssignDefaultConnectionString();
            Generate = false;
            List = false;
            Run = true;
            Sql = false;
            SetMigrateToLatestVersion();
        }
        private void SetMainToFalse() {
            WriteHelp = false;
            Generate = false;
            List = false;
            Run = false;
            Sql = false;
        }

        private void SetMigrateToLatestVersion() {
            MigrateToVersion = 0;
        }

        public Boolean WriteHelp { get; set; }
        public HelpSection HelpPage { get; set; }
        public bool Generate { get; set; }
        public bool List { get; set; }
        public bool Run { get; set; }
        public bool Sql { get; set; }
        public string ConnectionString { get; set; }

        // Sub Properties for Generate
        // Sub Properties for List
        // Sub Properties for Run
        /*
            
         */
        public int MigrateToVersion { get; set; }
        // Sub Properties for Sql

    }
}

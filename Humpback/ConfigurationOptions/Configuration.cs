using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Humpback.ConfigurationOptions;
using System.Configuration;

namespace Humpback.ConfigurationOptions {
    public class Configuration {

        private void Parse(IEnumerable<string> options) {
            var oset = new OptionSet()
                .Add("?:|help:|h:|HELP:|H:|Help:", SetSpecificHelpPage)
                .Add("f=|folder=|F=|FOLDER=|Folder=", s => { MigrationFolder = s.Trim(); })
                .Add("cs=|connectionstring=|CS=|CONNECTIONSTRING=", s => { ConnectionString = s.Trim(); })
                .Add("g=|gen=|G=|GEN=|Gen=|Generate=|generate=|GENERATE=", s => { 
                    SetMainToFalse(); 
                    Generate = true; 
                    GenerateString = s.Trim();})
                .Add("l:|list:|L:|LIST:|List:", s => { SetMainToFalse(); List = true; })
                .Add("r:|run:|R:|RUN:|Run:", s => { SetMainToFalse(); Run = true; })
                .Add("s:|sql:|S:|SQL:|Sql:", s => { SetMainToFalse(); Sql = true; });
            
            Extra = oset.Parse(options);
            EnsureMigrationFolder();
        }

        public Configuration() {
            ResetOptions();
        }

        public Configuration(IEnumerable<string> options)
            : this() {
            if (options == null || options.Count() == 0) {
                return;
            }
            DebugWriteOutOptions(options);
            Parse(options);
        }

        [Conditional("DEBUG")]
        private void DebugWriteOutOptions(IEnumerable<string> options) {
            foreach (string s in options) {
                //Debug.WriteLine("Option: " + s);
                //Trace.WriteLine("Option: " + s);
                //Console.WriteLine("Option: " + s);
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
            if (String.IsNullOrWhiteSpace(v)) {
                HelpPage = HelpSection.All;
                return;
            }
            switch (v.ToUpperInvariant().Trim()) {
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
            WriteHelp = true;
            HelpPage = HelpSection.All;
            AssignDefaultConnectionString();
            Generate = false;
            List = false;
            Run = false;
            Sql = false;
            SetMigrateToLatestVersion();
            MigrationFolder = DefaultMigrationFolder();
            GenerateString = "";
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
        public IEnumerable<string> Extra { get; set; }

        // Sub Properties for Generate
        public string GenerateString { get; set; }
        private bool ExplicitExecute { get; set; }
        private bool ExplicitFile { get; set; }



        // Sub Properties for List
        // Sub Properties for Run
        /*
            
         */
        public int MigrateToVersion { get; set; }
        // Sub Properties for Sql



        public string MigrationFolder { get; set; }

        private void EnsureMigrationFolder() {

            if (String.IsNullOrWhiteSpace(MigrationFolder)) {
                MigrationFolder = DefaultMigrationFolder();
            }
            if (!Directory.Exists(MigrationFolder)) {
                Directory.CreateDirectory(MigrationFolder);
            }
            if (!Directory.Exists(Path.Combine(MigrationFolder,"..\\sql"))) {
                Directory.CreateDirectory(Path.Combine(MigrationFolder,"..\\sql"));
            }

        }

        private static string DefaultMigrationFolder() {
            return Path.Combine(Environment.CurrentDirectory, @"db\migrations");
        }
    }

    public enum GenerateActionType {
        Empty, AddTable, RemoveTable, AddColumn, RemoveColumn, AddIndex, RemoveIndex, Sql, File
    }
    public enum RunActionType {
        Up, Down
    }
}

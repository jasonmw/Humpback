using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Configuration;

namespace Humpback.ConfigurationOptions {
    public class Configuration {

        private void Parse(IEnumerable<string> options) {
            var oset = new OptionSet()
                .Add("?|help|h|HELP|H|Help", h => WriteHelp = true)
                .Add("g=|gen=|G=|GEN=|Gen=|Generate=|generate=|GENERATE=",
                     s => {
                         SetMainToFalse();
                         Generate = true;
                         GenerateString = s.Trim();
                     })
                .Add("l:|list:|L:|LIST:|List:",
                     s => {
                         SetMainToFalse();
                         List = true;
                     })
                .Add("m:|migrate:|M:|MIGRATE:|Migrate:",
                     s => {
                         SetMainToFalse();
                         Migrate = true;
                     })
                .Add("s:|sql:|S:|SQL:|Sql:",
                     s => {
                         SetMainToFalse();
                         Sql = true;
                     })
                .Add("f:|file:|F:|File:|FILE:",
                     s => {
                         SetMainToFalse();
                         File = true;
                     })
                .Add("e|E|ENV|env|Env", e => {SetMainToFalse();Env = true;})
                .Add("all", a => All = true)
                .Add("single", s => Single = true)
                .Add("dp", dp => Deployed = true)
                .Add("ndp", ndp => NotDeployed = true)
                .Add("screen", s => Screen = true)
                .Add("up", u => Up = true)
                .Add("down", d => Down = true)
                .Add("empty", e => Empty = true)
                .Add("reset", r => Reset = true)
                .Add("v", v => Verbose = true)
                .Add("dir=", s => settings_dir=s)
                .Add("cs=", s => settings_cs = s)
                .Add("flavor=", s => settings_flavor = s)
                .Add("rename=", s => settings_rename = s)
                .Add("set=", s => set_current_settings = s)
                .Add("add=", s => new_project_name = s)
                .Add("init", s => env_init = true)
                .Add("remove=", s => remove_project_name = s)
                ;
            
            Extra = oset.Parse(options);
        }

        public bool Env { get; private set; }
        public bool env_init { get; private set; }
        public string settings_dir { get; private set; }
        public string settings_cs { get; private set; }
        public string settings_flavor { get; private set; }
        public string settings_rename { get; private set; }
        public string set_current_settings { get; private set; }
        public string new_project_name { get; private set; }
        public string remove_project_name { get; private set; }



        public Configuration() {
            ResetOptions();
        }

        public Configuration(IEnumerable<string> options)
            : this() {
            if (options == null || options.Count() == 0) {
                return;
            }
            Parse(options);
        }



        public string NextSerialNumber() {
            return DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
        }


        

        private void ResetOptions() {
            WriteHelp = true;
            Generate = false;
            List = false;
            Migrate = false;
            Sql = false;
            Env = false;
            File = false;
            SetMigrateToLatestVersion();
            GenerateString = "";
        }
        private void SetMainToFalse() {
            WriteHelp = false;
            Generate = false;
            List = false;
            Migrate = false;
            Sql = false;
            Env = false;
            File = false;
        }

        private void SetMigrateToLatestVersion() {
            MigrateToVersion = 0;
        }

        public Boolean WriteHelp { get; set; }
        public bool Generate { get; set; }
        public bool List { get; set; }
        public bool File { get; set; }
        public bool Migrate { get; set; }
        public bool Sql { get; set; }
        public IEnumerable<string> Extra { get; set; }

        // Sub Properties for Generate
        public string GenerateString { get; set; }



        // Sub Properties for List
        // Sub Properties for Run

        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Empty { get; set; }
        public bool Reset { get; set; }

        public int MigrateToVersion { get; set; }
        // Sub Properties for Sql

        public bool All { get; set; }
        public bool Single { get; set; }
        public bool Deployed { get; set; }
        public bool NotDeployed { get; set; }
        public bool Screen { get; set; }


        // sub props for settings
        // shares All with sql



        public bool Verbose {
            get;
            set;
        }

    }

    public enum GenerateActionType {
        Empty, AddTable, RemoveTable, AddColumn, ChangeColumn, RemoveColumn, AddIndex, RemoveIndex, Sql, File,
        FileSmo
    }
    public enum RunActionType {
        Up, Down
    }
}

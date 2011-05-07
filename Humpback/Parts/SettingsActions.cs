using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humpback.ConfigurationOptions;

namespace Humpback.Parts {
    public class SettingsActions : IHumpbackCommand {
        private Configuration _configuraion;
        private Settings _current_settings;

        public SettingsActions(Configuration configuration, Settings current_settings) {
            _configuraion = configuration;
            _current_settings = current_settings;
        }
        public void Execute() {
            Console.WriteLine(@"
Humpback Migration Settings
===========================
");
            try {
                if (!String.IsNullOrWhiteSpace(_configuraion.new_project_name)) {
                    _current_settings.AddProject(_configuraion.new_project_name);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.settings_cs)) {
                    _current_settings.SetConnectionString(_configuraion.settings_cs);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.settings_dir)) {
                    _current_settings.SetDirectory(_configuraion.settings_dir);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.settings_flavor)) {
                    _current_settings.SetFlavor(_configuraion.settings_flavor);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.settings_rename)) {
                    _current_settings.Rename(_configuraion.settings_rename);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.remove_project_name)) {
                    _current_settings.Remove(_configuraion.remove_project_name);
                }
                if (!String.IsNullOrWhiteSpace(_configuraion.set_current_settings)) {
                    _current_settings.SetCurrent(_configuraion.set_current_settings);
                    Console.WriteLine("new current project: " + _configuraion.set_current_settings.ToLower());
                }
            } catch (Exception ex) {
                if(_configuraion.Verbose) {
                    Console.WriteLine(ex);
                } else {
                    Console.WriteLine(ex.Message);
                }
            }
            foreach(var setting in _current_settings.Projects) {
                if (setting.name == _current_settings.CurrentProject) {
                    Console.Write(setting.name);
                    Console.WriteLine(" ***");
                } else {
                    Console.WriteLine(setting.name);
                }
                Console.WriteLine("  - connection string : " + setting.connection_string);
                Console.WriteLine("  - working directory : " + setting.directory);
                Console.WriteLine("  - flavor            : " + setting.flavor);
            }
        }
    }
}

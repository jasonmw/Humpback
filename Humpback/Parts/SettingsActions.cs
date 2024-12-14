using Humpback.ConfigurationOptions;
using System;
using System.Linq;

namespace Humpback.Parts
{
    public class SettingsActions : IHumpbackCommand
    {
        private Configuration _configuraion;
        private Settings _current_settings;

        public SettingsActions(Configuration configuration, Settings current_settings)
        {
            _configuraion = configuration;
            _current_settings = current_settings;
        }
        public void Execute()
        {
            Console.WriteLine(@"
Humpback Migration Settings
===========================
");
            if (!String.IsNullOrWhiteSpace(_configuraion.new_project_name))
            {
                _current_settings.AddProject(_configuraion.new_project_name);
            }
            else if (_configuraion.env_init)
            {
                if (!_current_settings.Projects.Any(p => p.name.Equals("default", StringComparison.OrdinalIgnoreCase)))
                {
                    _current_settings.AddProject("default");
                }
                else
                {
                    string new_name = "";
                    new_name = AskForProjectName();
                    _current_settings.AddProject(new_name);
                }
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.settings_cs))
            {
                _current_settings.SetConnectionString(_configuraion.settings_cs);
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.settings_dir))
            {
                _current_settings.SetDirectory(_configuraion.settings_dir);
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.settings_flavor))
            {
                _current_settings.SetFlavor(_configuraion.settings_flavor);
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.settings_rename))
            {
                _current_settings.Rename(_configuraion.settings_rename);
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.remove_project_name))
            {
                _current_settings.Remove(_configuraion.remove_project_name);
            }
            if (!String.IsNullOrWhiteSpace(_configuraion.set_current_settings))
            {
                _current_settings.SetCurrent(_configuraion.set_current_settings);
                Console.WriteLine("new current project: " + _configuraion.set_current_settings.ToLower());
            }
            Console.WriteLine("Settings file " + Settings.SettingsFilePath);
            foreach (var setting in _current_settings.Projects)
            {
                if (setting.name == _current_settings.CurrentProject)
                {
                    Console.Write(setting.name);
                    Console.WriteLine(" ***");
                }
                else
                {
                    Console.WriteLine(setting.name);
                }
                Console.WriteLine("  - connection string : " + setting.connection_string);
                Console.WriteLine("  - working directory : " + setting.directory);
                Console.WriteLine("  - flavor            : " + setting.flavor);
            }
        }

        private string AskForProjectName()
        {
            string new_name;
            do
            {
                Console.Write("Project Name > ");
                new_name = Console.ReadLine();
                if (_current_settings.Projects.Any(
                    p => p.name.Equals(new_name, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("That name already exists as a project.");
                    Console.WriteLine("To switch to that project use the command");
                    Console.WriteLine("hump -env -set " + new_name);
                    Console.WriteLine();
                    Console.WriteLine("Your existing projects are:");
                    foreach (var p in _current_settings.Projects)
                    {
                        Console.WriteLine("  - " + p.name);
                    }
                }
            } while (string.IsNullOrWhiteSpace(new_name) ||
                     _current_settings.Projects.Any(
                         p => p.name.Equals(new_name, StringComparison.OrdinalIgnoreCase)));
            return new_name;
        }
    }
}

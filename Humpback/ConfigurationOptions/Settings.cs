using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Humpback.ConfigurationOptions
{
    public class Settings
    {

        public string CurrentProject { get; set; }
        public Project[] Projects { get; set; }

        public Settings()
        {
            Projects = new Project[0];
        }

        public Settings(string project_name, Project[] projects)
        {
            Projects = projects;
            CurrentProject = project_name;
        }

        public static Settings Load()
        {
            if (!File.Exists(SettingsFilePath))
            {
                var settings = create("default");
                settings.save();
                return settings;
            }
            var json = File.ReadAllText(SettingsFilePath);
            var serializer = new JavaScriptSerializer();
            return (Settings)serializer.Deserialize(json, typeof(Settings));
        }

        public Project SetCurrent(string project_name)
        {
            validate_current_project(project_name);
            CurrentProject = project_name.ToLower();
            save();
            return GetCurrent();
        }

        public Project GetCurrent()
        {
            validate_current_project(CurrentProject);
            return Projects.Single(p => p.name == CurrentProject);
        }

        public string SqlFileFolder()
        {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "sql");
        }

        public string MigrationsFolder()
        {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "migrations");
        }

        public string OutputFolder()
        {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "generated");
        }

        public string ConnectionString()
        {
            return GetCurrent().connection_string;
        }

        public void Rename(string new_name)
        {
            if (string.IsNullOrWhiteSpace(new_name))
            {
                throw new InvalidDataException("please specify a name with some printable characters.");
            }
            foreach (var proj in Projects)
            {
                if (proj.name == CurrentProject)
                {
                    proj.name = new_name.ToLower();
                    CurrentProject = new_name.ToLower();
                    break;
                }
            }
            save();
        }

        public void SetDirectory(string new_directory)
        {
            if (string.IsNullOrWhiteSpace(new_directory))
            {
                throw new InvalidDataException("please specify a directory path with some printable characters.");
            }
            if (!Directory.Exists(new_directory))
            {
                Directory.CreateDirectory(new_directory);
            }
            GetCurrent().directory = new_directory;
            save();
        }

        public void SetConnectionString(string new_connection_string)
        {
            if (string.IsNullOrWhiteSpace(new_connection_string))
            {
                throw new InvalidDataException("please specify a connection string with some printable characters.");
            }
            GetCurrent().connection_string = new_connection_string;
            save();
        }

        internal void SetFlavor(string new_flavor)
        {
            if (string.IsNullOrWhiteSpace(new_flavor))
            {
                throw new InvalidDataException("please specify a flavor with some printable characters.");
            }
            var flavors = new HashSet<string> { "sqlserver" };
            if (!flavors.Contains(new_flavor.ToLower()))
            {
                throw new InvalidDataException("currently sqlserver is the only flavor.");
            }
            GetCurrent().flavor = new_flavor.ToLower();
            save();
        }

        internal void AddProject(string project_name)
        {
            if (string.IsNullOrWhiteSpace(project_name))
            {
                throw new InvalidOperationException("No current project specified.");
            }
            if (Projects.Any(p => p.name == project_name))
            {
                throw new InvalidDataException(string.Format("'{0}' is already a project.", project_name));
            }
            var project = new Project
            {
                name = project_name.ToLower(),
                connection_string = "server=.;database=northwind;Integrated Security=True;",
                directory = Path.Combine(Environment.CurrentDirectory, "db"),
                flavor = "sqlserver"
            };
            CurrentProject = project_name.ToLower();
            var pjts = new List<Project>(Projects) { project };
            Projects = pjts.ToArray();
            save();
        }

        internal void Remove(string project_name)
        {
            Projects = Projects.Where(p => p.name != project_name.ToLower()).ToArray();
            if (CurrentProject == project_name.ToLower())
            {
                CurrentProject = "";
                if (Projects.Length > 0)
                {
                    CurrentProject = Projects.First().name;
                }
            }
            save();
        }

        private static Settings create(string project_name)
        {
            var project = new Project
            {
                name = project_name.ToLower(),
                connection_string = "server=.;database=northwind;Integrated Security=True;",
                directory = Path.Combine(Environment.CurrentDirectory, "db"),
                flavor = "sqlserver"
            };
            return new Settings
            {
                CurrentProject = project_name.ToLower(),
                Projects = new[] { project }
            };

        }

        private void validate_current_project(string project)
        {
            if (string.IsNullOrWhiteSpace(project))
            {
                throw new InvalidOperationException("No current project set.");
            }
            if (!Projects.Any(p => p.name == project))
            {
                throw new KeyNotFoundException(string.Format("'{0}' is not a valid project.", project));
            }
        }

        private void save()
        {
            var file_path = SettingsFilePath;
            var fi = new FileInfo(file_path);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.Directory.FullName);
            }

            var serializer = new JavaScriptSerializer();
            var output = serializer.Serialize(this);
            File.WriteAllText(file_path, output, Encoding.UTF8);
            EnsureDirectories();
        }

        internal void EnsureDirectories()
        {
            if (!Directory.Exists(MigrationsFolder()))
            {
                Directory.CreateDirectory(MigrationsFolder());
            }
            if (!Directory.Exists(OutputFolder()))
            {
                Directory.CreateDirectory(OutputFolder());
            }
            if (!Directory.Exists(SqlFileFolder()))
            {
                Directory.CreateDirectory(SqlFileFolder());
            }
        }


        internal static string SettingsFilePath
        {
            get
            {
                // if local exists, use it, otherwise return the Application Settings Version
                return local_settings_file_if_exists() ?? application_settings_file_path();
            }
        }

        private static string application_settings_file_path()
        {
            var app_data_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var app_settings_path = Path.Combine(app_data_path, "humpback");
            var app_settings_file_path = Path.Combine(app_settings_path, "settings.js");
            return app_settings_file_path;
        }
        private static string local_settings_file_if_exists()
        {
            var current_path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var local_file_path = Path.Combine(current_path, "settings.js");
            if (File.Exists(local_file_path))
            {
                return local_file_path;
            }
            return null;
        }
    }
}

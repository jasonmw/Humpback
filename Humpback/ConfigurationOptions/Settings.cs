using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Humpback.ConfigurationOptions {
    public class Settings {

        public string current_project { get; set; }
        public Project[] projects { get; set; }


        public Settings() {
            projects = new Project[0];
        }
        public Settings(string project_name, Project[] projects) {
            this.projects = projects;
            current_project = project_name;
        }

        public static Settings Load() {
            if (!File.Exists(settings_file_path)) {
                var settings = Create("default");
                settings.Save();
                return settings;
            }
            var json = File.ReadAllText(settings_file_path);
            var serializer = new JavaScriptSerializer();
            return (Settings)serializer.Deserialize(json, typeof(Settings));
        }


        public Project SetCurrent(string project_name) {
            validate_current_project(project_name);
            current_project = project_name.ToLower();
            Save();
            return GetCurrent();
        }


        public Project GetCurrent() {
            validate_current_project(current_project);
            return projects.Single(p => p.name == current_project);
        }


        public string SqlFileFolder() {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "sql");
        }

        public string MigrationsFolder() {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "migrations");
        }

        public string OutputFolder() {
            var i = new DirectoryInfo(GetCurrent().directory);
            return Path.Combine(i.FullName, "generated");
        }
        public string ConnectionString() {
            return GetCurrent().connection_string;
        }

        public void Rename(string new_name) {
            if (string.IsNullOrWhiteSpace(new_name)) {
                throw new InvalidDataException("please specify a name with some printable characters.");
            }
            foreach(var proj in projects) {
                if(proj.name == current_project) {
                    proj.name = new_name.ToLower();
                    current_project = new_name.ToLower();
                    break;
                }
            }
            Save();
        }
        public void SetDirectory(string new_directory) {
            if (string.IsNullOrWhiteSpace(new_directory)) {
                throw new InvalidDataException("please specify a directory path with some printable characters.");
            }
            if (!Directory.Exists(new_directory)) {
                Directory.CreateDirectory(new_directory);
            }
            GetCurrent().directory = new_directory;
            Save();
        }
        public void SetConnectionString(string new_connection_string) {
            if (string.IsNullOrWhiteSpace(new_connection_string)) {
                throw new InvalidDataException("please specify a connection string with some printable characters.");
            }
            GetCurrent().connection_string = new_connection_string;
            Save();
        }
        public void SetFlavor(string new_flavor) {
            if (string.IsNullOrWhiteSpace(new_flavor)) {
                throw new InvalidDataException("please specify a flavor with some printable characters.");
            }
            var flavors = new HashSet<string> { "sqlserver" };
            if (!flavors.Contains(new_flavor.ToLower())) {
                throw new InvalidDataException("currently sqlserver is the only flavor.");
            }
            GetCurrent().flavor = new_flavor.ToLower();
            Save();
        }



        public void AddProject(string project_name) {
            if (string.IsNullOrWhiteSpace(project_name)) {
                throw new InvalidOperationException("No current project specified.");
            }
            if (projects.Any(p => p.name == project_name)) {
                throw new InvalidDataException(string.Format("'{0}' is already a project.", project_name));
            }
            var project = new Project {
                name = project_name.ToLower(),
                connection_string = "server=.;database=northwind;Integrated Security=True;",
                directory = Path.Combine(Environment.CurrentDirectory, "db"),
                flavor = "sqlserver"
            };
            current_project = project_name.ToLower();
            var pjts = new List<Project>(projects) { project };
            projects = pjts.ToArray();
            Save();
        }

        private static Settings Create(string project_name) {
            var project = new Project {
                name = project_name.ToLower(),
                connection_string = "server=.;database=northwind;Integrated Security=True;",
                directory = Path.Combine(Environment.CurrentDirectory, "db"),
                flavor = "sqlserver"
            };
            return new Settings {
                current_project = project_name.ToLower(),
                projects = new[] { project }
            };

        }

        private void validate_current_project(string project) {
            if (string.IsNullOrWhiteSpace(project)) {
                throw new InvalidOperationException("No current project set.");
            }
            if (!projects.Any(p => p.name == project)) {
                throw new KeyNotFoundException(string.Format("'{0}' is not a valid project.", project));
            }
        }
        public void Remove(string project_name) {
            projects = projects.Where(p => p.name != project_name.ToLower()).ToArray();
            if(current_project == project_name.ToLower()) {
                current_project = "";
                if(projects.Length > 0) {
                    current_project = projects.First().name;
                }
            }
            Save();
        }
        private void Save() {
            string file_path = settings_file_path;
            var serializer = new JavaScriptSerializer();
            string output = serializer.Serialize(this);
            File.WriteAllText(file_path, output, Encoding.UTF8);

            EnsureDirectories();
        }

        private void EnsureDirectories() {
            if (!Directory.Exists(MigrationsFolder())) {
                Directory.CreateDirectory(MigrationsFolder());
            }
            if (!Directory.Exists(OutputFolder())) {
                Directory.CreateDirectory(OutputFolder());
            }
            if (!Directory.Exists(SqlFileFolder())) {
                Directory.CreateDirectory(SqlFileFolder());
            }
        }


        private static string settings_file_path {
            get {
                var path = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                return Path.Combine(path, "settings.js");
            }
        }
    }
}

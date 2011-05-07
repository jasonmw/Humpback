using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.ConfigurationOptions {
    public class Project {
        public string name { get; set; }
        public string directory { get; set; }
        public string connection_string { get; set; }
        public string flavor { get; set; }
    }
}

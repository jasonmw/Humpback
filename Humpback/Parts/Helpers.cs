using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Humpback.Parts {
    public static class Helpers {

        private static string[] keywords_tab_level_1 = new[] {
            "up","down"
        };
        private static string[] keywords_tab_level_2 = new[] {
            "execute","create_table","drop_table","add_column","remove_column","add_index","remove_index"
        };
        private static string[] keywords_tab_level_3 = new[] {
            "name","columns","table","column","table_name","timestamps"
        };

        // Here be formatting dragons
        public static string Json(dynamic input) {
            string json = new JavaScriptSerializer().Serialize(input);
            json = keywords_tab_level_1.Aggregate(json, (current, s) => current.Replace("\"" + s + "\":", Environment.NewLine + "\t\"" + s + "\":"));
            json = keywords_tab_level_2.Aggregate(json, (current, s) => current.Replace("\"" + s + "\":", Environment.NewLine + "\t\t\"" + s + "\":"));
            json = keywords_tab_level_3.Aggregate(json, (current, s) => current.Replace("\"" + s + "\":", Environment.NewLine + "\t\t\t\"" + s + "\":"));
            json = json.Replace("}},", Environment.NewLine + "\t\t}" + Environment.NewLine + "\t},");
            json = json.Substring(0, json.Length - 2) + (json.Substring(0, json.Length - 2).Contains("\"")?"\"":"") + Environment.NewLine + "\t}" + Environment.NewLine + "}";
            return json;
        }
    }
}

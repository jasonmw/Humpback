using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Humpback.ConfigurationOptions {
    public class Help : IHumpbackCommand {

        private Configuration _configuration;

        public Help(Configuration configuration) {
            _configuration = configuration;
        }



        public void Execute() {
            string flag = (_configuration.Extra!=null && _configuration.Extra.Any()) ? _configuration.Extra.First() : "";
            flag = flag.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(flag)) {
                write_general_help();
            } else if ("GENERATE".StartsWith(flag)) {
                write_generate_help();
            } else if ("LIST".StartsWith(flag)) {
                write_list_help();
            } else if ("MIGRATE".StartsWith(flag)) {
                write_migrate_help();
            } else if ("SQL".StartsWith(flag)) {
                write_sql_help();
            } else {
                write_general_help();
            }
        }
        private static void write_generate_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================
Generate Migrations
==============================

Generating migrations was modeled after the conventions in rails.
the migration types that can be generated are:

  + create_table
  + drop_table
  + add_column
  + remove_column
  + add_index
  + remove_index
  + sql
  + file

if you do not specify a primary key, one will be appointed for you.
timestamps will be added automatically


create_table example ( tablename column_name:column_type column_name:column_type )
> hump -g Person FirstName:string LastName:string Birthdate:datetime FavoriteNumber:int

drop_table example ( DropTableName )
> hump -g DropPerson

add_column example ( AddColumnNameToTableName column_name:column_type column_name:column_type  )
> hump -g AddFavoriteThingsToPerson FavoriteColor:string FavoriteAnimal:string

remove_column example ( RemoveColumnNameFromTableName )
only removes one column currently
> hump -g RemoveBirthdayFromPerson

add_index example ( AddIndexToTableName  column_name column_name)
> hump -g AddIndexToPerson FirstName LastName

remove_index ( RemoveIndexFromTableName  column_name column_name)
column names are required to be able to generate the index name
> hump -g RemoveIndexFromPerson FirstName LastName

file ( file SqlFileName )
the sql file will be generated for you
> hump -g file MyBigCreateStoredProcedureFile

sql ( sql SqlString )
> hump -g sql ""CREATE VIEW PersonView AS SELECT FirstName,LastName from Person"" 

");
        }
        private static void write_list_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================
List Migrations and Details
==============================

2 commands available for listing migrations


> hump -list    | List all migrations and their deploy status
> hump -list 6  | List single migration by number


");
        }
        private static void write_migrate_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================
Executing Migrations
==============================

Executes Migrations against the database.
Available actions:

> hump -m -all    | updates database to most recent migration
> hump -m 12      | updates database to a specific migration (up or down)
> hump -m -up     | migrates database up one migration
> hump -m -down   | migrates database down one migration
> hump -m -empty  | removes all migrations from database
> hump -m -reset  | removes and re-adds all migrations (-empty, then -all)

");
        }
        private static void write_sql_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================
Generating SQL Files
==============================

Generates Sql files based on migrations.
Available actions:

> hump -s -all      | writes all migrations into sql file(s)
> hump -s 12        | writes 1 specified migration into a sql file
> hump -s -dp       | writes all deployed migraions into sql file(s)
> hump -s -ndp      | writes all undeployed migrations into sql file(s)
          -single   | specifies to write all sql to one file
                    | default is seperate files per migration
          -screen   | writes the sql to the screen for viewing
                    | does NOT write file when screen is specified

");
        }
        private static void write_general_help() {
            Console.WriteLine(@"
Humpback Migration Information
==============================

Main commands
  -generate -g | Generate JSON migration files
  -list     -l | List Migrations and current migration state
  -migrate  -m | Run Migrations against database
  -sql      -s | Generate SQL files from migration files


For information about a specific command use the following

  hump -? generate
  hump -? migrate
  hump -? list
  hump -? sql

");
        }
    }
}

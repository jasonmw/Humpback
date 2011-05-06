Humpback is a database migration tool
=====================================
Inspired by Rails tooling and another project called Manatee https://github.com/robconery/Manatee.

The database migrations are similar to rails, the format they are stored in is json.

Humpback does 4 Main things
The analagous features are like so

- Humpback > Generate ~ rails generate migration
- Humpback > List     ~ manatee page and color coding
- Humpback > Sql      ~ view/export sql files from migrations for safe keeping or direct usage
- Humpback > Migrate  ~ rake db:migrate




Using Humpback
------------------------------

Main commands  

    generate -g | Generate JSON migration files  
    list     -l | List Migrations and current migration state  
    migrate  -m | Run Migrations against database  
    sql      -s | Generate SQL files from migration files  


For information about a specific command use the following  

    hump -? generate  
    hump -? migrate  
    hump -? list  
    hump -? sql  




GENERATE
------------------------------

Generating migrations was modeled after the conventions in rails.  
like addXXXtoXXX and removeXXXfromXXX  

the migration types that can be generated are:

  + create_table
  + drop_table
  + add_column
  + remove_column
  + add_index
  + remove_index
  + sql
  + file (execute an external sql file, i.e. a long stored procedure)

if you do not specify a primary key, one will be appointed for you.  
timestamps will be added automatically  

just like rails, sequential filenames will be generated for you (UTC timestamp)  

**create_table** example ( tablename column_name:column_type column_name:column_type )  
    `> hump -g Person FirstName:string LastName:string Birthdate:datetime FavoriteNumber:int`

**drop_table** example ( DropTableName )  
    `> hump -g DropPerson`

**add_column** example ( AddColumnNameToTableName column_name:column_type column_name:column_type  )  
    `> hump -g AddFavoriteThingsToPerson FavoriteColor:string FavoriteAnimal:string`

**change_column** example ( ChangeTableName column_name:column_type column_name:column_type )  
    `> hump -g ChangePerson FavoriteNumber:long`

**remove_column** example ( RemoveColumnNameFromTableName )  
only removes one column currently
    `> hump -g RemoveBirthdayFromPerson`

**add_index** example ( AddIndexToTableName  column_name column_name)  
    `> hump -g AddIndexToPerson FirstName LastName`

**remove_index** example ( RemoveIndexFromTableName  column_name column_name)  
column names are required to be able to generate the index name  
    `> hump -g RemoveIndexFromPerson FirstName LastName`

**sql** ( sql SqlString )  
    `> hump -g sql ""CREATE VIEW PersonView AS SELECT FirstName,LastName from Person""`

**file** ( file SqlFileName )  
the sql file will be generated for you  
    `> hump -g file MyBigCreateStoredProcedureFile`



List Migrations and Details
---------------------------

2 commands available for listing migrations

    > hump -list    | List all migrations and their deploy status
    > hump -list 6  | List single migration by number

the -list shows your migrations and their status' like this  

    6 Migrations              \o/ means it is currently deployed
    ============================================================
    \o/   1 20110501025407_AddTable_tname.js
    \o/   2 20110502062537_AddColumn_tname_name.js
    \o/   3 20110502064908_SQLFile_This_Is_A_Test.js
    \o/   4 20110502165734_AddTable_Products.js
          5 20110502231012_AddTable_Customers.js
          6 20110502231111_AddColumn_Customers_buddyname.js
      
like in the second example, if you specify a number  
it will print the migration json to the console.  



Executing Migrations
--------------------
can you say rake db:migrate  
Executes Migrations against the database.  
Available actions:  

    > hump -m -all    | updates database to most recent migration
    > hump -m 12      | updates database to a specific migration (up or down)
    > hump -m -up     | migrates database up one migration
    > hump -m -down   | migrates database down one migration
    > hump -m -empty  | removes all migrations from database
    > hump -m -reset  | removes and re-adds all migrations (-empty, then -all)



Generating SQL Files
--------------------

Generates Sql files based on migrations.  
This can be valuable for a number of strategies.  
Available actions:  

    > hump -s -all      | writes all migrations into sql file(s)
    > hump -s 12        | writes 1 specified migration into a sql file
    > hump -s -dp       | writes all deployed migraions into sql file(s)
    > hump -s -ndp      | writes all undeployed migrations into sql file(s)
              -single   | specifies to write all sql to one file
                        | default is seperate files per migration
              -screen   | writes the sql to the screen for viewing
                        | does NOT write file when screen is specified

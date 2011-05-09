Humpback is a database migration tool
=====================================
Inspired by Rails tooling and another project called Manatee https://github.com/robconery/Manatee.  
(i totally took the Manatee idea and built the tools i would want, in order to use the concept outside of web matrix)

The database migrations are similar to rails, they are [stored as json](https://github.com/jasonmw/Humpback/wiki/Migration-Format-Examples).

Your interface is the command line.

##Available via NuGet as a package manager console tool
From Visual Studio package manager console

```
Install-Package Humpback
```
or from the nuget.exe command line application.

 ```
 > nuget.exe install Humpback
 ```
 

Humpback does 4 Main things
---------------------------
The analagous features are like so

    Humpback > Generate ~ rails generate migration
    Humpback > List     ~ Manatee page and color coding
    Humpback > Sql      ~ view/export sql files from migrations for safe keeping or direct usage
    Humpback > Migrate  ~ rake db:migrate




Using Humpback
------------------------------

Main commands  

    env      -e | Configure the migration environment (working directory, connection string)
    generate -g | Generate JSON migration files  
    list     -l | List Migrations and current migration state  
    migrate  -m | Run Migrations against database  
    sql      -s | Generate SQL files from migration files  


For information about a specific command use the following  

    hump -? env
    hump -? generate  
    hump -? migrate  
    hump -? list  
    hump -? sql  


The json syntax is explained very well by Rob at https://github.com/robconery/Manatee  
and also [HERE](https://github.com/jasonmw/Humpback/wiki/Migration-Format-Examples)
This should consume the files specified for manatee.  


Generate Migrations
-------------------

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

if you add a column that is the name of another table, and specify the type as 'reference' (works for create table too)

    > hump -g AddUserToPost User:reference
    
would generate this as the up operation

    'add_column':{'table':'Post', 'columns':[{'name':'User','type':'reference'}]}
    

and the sql that will be generated...

    > hump -s 4 -screen ( this was my migration number 4.  see hump -? list )

	ALTER TABLE [Post] ADD [UserId] INT NOT NULL
	
	ALTER TABLE [Post] ADD CONSTRAINT [FK_post_user_userid] FOREIGN KEY (UserId)
	REFERENCES User (Id) ON DELETE NO ACTION ON UPDATE NO ACTION;
	
see https://github.com/jasonmw/Humpback/wiki/Migration-Format-Examples for examples and details of the migration format
    
just like rails, sequential filenames will be generated for you (UTC timestamp)  

**create_table** ( tablename column_name:column_type column_name:column_type )  
    `> hump -g Person FirstName:string LastName:string Birthdate:datetime FavoriteNumber:int`

**drop_table** ( DropTableName )  
    `> hump -g DropPerson`

**add_column** ( AddColumnNameToTableName column_name:column_type column_name:column_type  )  
    `> hump -g AddFavoriteThingsToPerson FavoriteColor:string FavoriteAnimal:string`

**change_column** ( ChangeTableName column_name:column_type column_name:column_type )  
    `> hump -g ChangePerson FavoriteNumber:long`

**remove_column** ( RemoveColumnNameFromTableName )  
only removes one column currently
    `> hump -g RemoveBirthdayFromPerson`

**add_index** ( AddIndexToTableName  column_name column_name)  
    `> hump -g AddIndexToPerson FirstName LastName`

**remove_index** ( RemoveIndexFromTableName  column_name column_name)  
column names are required to be able to generate the index name  
    `> hump -g RemoveIndexFromPerson FirstName LastName`

**sql** ( sql SqlString )  
    `> hump -g sql "CREATE VIEW PersonView AS SELECT FirstName,LastName from Person"`

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



Managing environment settings
-----------------------------
Environment settings are stored in a json format in a file called settings.js

On the first run of the application, the file will be created if it does not exist.

You may prefer to configure settings from the command line.

Available actions:

    > hump -e           | lists environment information
    > hump -e -set x    | sets the current project to x if x is a valid project
    > hump -e -add x    | creates a new project called x and sets it to the current project
    > hump -e -remove x | removes the project named x
    > hump -e -rename x | renames the current project to x
    > hump -e -dir x    | sets the working directory of the current project to x
    > hump -e -cs x     | sets the connection string of the current project to x
    > hump -e -flavor x | sets the sql flavor of the current project to x (currently, only sqlserver available)




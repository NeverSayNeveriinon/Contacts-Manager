using System.Text.Json;
using Core.Domain.Entities;
using Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContext
{
    public class PersonsDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        // This Constructor with the 'DbContextOptions' parameter must be defined, if not we will get a compiler error
        public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options)
        {
            
        }


        //
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            
            // Seed Data for Country //

            // Read the json file into a string
            string countriesSeed_Path = @"C:\Visual Studio\Asp.Net Core MVC\With_Repository\With_Repository\wwwroot\JSON\seed_countries.json";
            // string countriesSeed_Path = @"wwwroot\JSON\seed_countries.json";
            // string countriesSeed_Path = @"..\..\..\..\With_Repository\wwwroot\JSON\seed_countries.json";
            string countriesSeed_Json = File.ReadAllText(countriesSeed_Path);
            
            // Deserialize the json file to 'a List of Country'
            List<Country> countriesSeed_List = JsonSerializer.Deserialize<List<Country>>(countriesSeed_Json)!;

            // inserting one row in table per 'each Country object in List'
            foreach (Country country in countriesSeed_List)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }


            // Seed Data for Person //

            // Read the json file into a string
            string personsSeed_Path = @"C:\Visual Studio\Asp.Net Core MVC\With_Repository\With_Repository\wwwroot\JSON\seed_persons.json";
            // string personsSeed_Path = @"wwwroot\JSON\seed_persons.json";
            // string personsSeed_Path = @"..\..\..\..\With_Repository\wwwroot\JSON\seed_persons.json";
            string personsSeed_Json = File.ReadAllText(personsSeed_Path);

            // Deserialize the json file to 'a List of Person'
            List<Person> personsSeed_List = JsonSerializer.Deserialize<List<Person>>(personsSeed_Json)!;

            // inserting one row in table per 'each Person object in List'
            foreach (Person person in personsSeed_List)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            
            // Seed Data for Roles (user,admin) //
            
            List<ApplicationRole> rolesList =
            [
                new ApplicationRole() { Id = Guid.Parse("211D03AE-07FE-4CB4-813E-163F46568B44"),Name = "User", NormalizedName = "USER", 
                                        ConcurrencyStamp = "4A191780-214E-47D6-83B8-4345A61E804C"},
                new ApplicationRole() { Id = Guid.Parse("A9C2BC35-61FE-4E60-8158-2BFD6E1478EB"),Name = "Admin", NormalizedName = "ADMIN", 
                                        ConcurrencyStamp = "7DB6149D-F1EE-4A0B-BA7B-D99E41414D73"}
            ];
            modelBuilder.Entity<ApplicationRole>().HasData(rolesList);
            
            
            
            // Fluent API //

            // -
            //modelBuilder.HasDefaultSchema("dbo");

            // -
            //modelBuilder.Entity<Person>().ToTable("Persons", schema: "dbo");

            // -
            //modelBuilder.Entity<Person>().ToView("PersonsView", schema: "dbo");

            // -
            //modelBuilder.Entity<Person>().HasKey(entity => entity.ID);

            // -
            //modelBuilder.Entity<Person>().Property(entity => entity.TIN).IsRequired(required:true);


            // - 1. Mention the DB tables names (2. and A constraint on "Person" table)
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons", tablebuilder => tablebuilder.HasCheckConstraint("CHK_TINumber", "len([TINumber]) = 11"));

            // -
            modelBuilder.Entity<Person>().Property(entity => entity.TIN)
                .HasColumnName("TINumber")
                .HasColumnType("varchar(11)")
                .HasDefaultValue("ABCDEFGH123");

            // - for make the property unique (must use index on that first)
            //modelBuilder.Entity<Person>().HasIndex(entity => entity.TIN).IsUnique();

            
            // -
            // modelBuilder.Entity<Person>()
            //     .HasOne(e => e.Country)
            //     .WithMany(e => e.Persons)
            //     .HasForeignKey(e => e.CountryID)
            //     .IsRequired();
        }




        // Stored Procedures //


        //
        public async Task<List<Person>> SP_GetAllPersons()
        {
            // Way 1: (With "FromSql") --> (Recommended)
            // the '$' is necessary for "FromSql" method because its parameter datatype is 'FormattableString'
            List<Person> persons = await Persons.FromSql($" EXECUTE dbo.GetAllPersons ").ToListAsync();
            // !!Runtime EXCEPTION!! --> List<Person> persons = Persons.FromSql($"{sp_GetAllPersons}").ToList();;


            //// Way 2: (With "FromSqlRaw")
            //string sp_GetAllPersons = @"
            //    EXECUTE dbo.GetAllPersons   
            //    ";

            //List<Person> persons1 = Persons.FromSqlRaw(sp_GetAllPersons).ToList();

            return persons; 
        }


        //
        public async Task<List<Country>> SP_GetAllCountries ()
        {
            List<Country> countries = await Countries.FromSql($" EXECUTE dbo.GetAllCountries ").ToListAsync();
            return countries;
        }


        //
        public async Task<Person?> SP_GetPersonByID(Guid id)
        {
            SqlParameter idParameter = new SqlParameter("@ID", id);
            Person? person = await Persons.FromSql($" EXECUTE dbo.GetPersonByID {idParameter}").FirstOrDefaultAsync();
            return person;
        }


        //
        public async Task<int> SP_AddPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@ID", person.ID),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.RecieveNewsLetters)
            };

            // (With "ExecuteSqlRaw") --> (Recommended)
            // Because we have more than one parameter, I couldn't find a way to interpolate to a string to use "ExecuteSql",
            // so i will use "ExecuteSqlRaw"
            int numOfRowsAffected = await Database.ExecuteSqlRawAsync(" EXECUTE dbo.AddPerson @ID, @Name, @Email, @DateOfBirth, @Gender, " +
                                                                                  "@CountryID, @Address, @ReceiveNewsLetters", parameters);

            return numOfRowsAffected;
        }
    }
}


// ## Tips ##
//
// 1) There are two general ways to connect a '.net core app' to a 'database':
//  1. "ADO.Net": It is the lowest data access layer technology in .net. (Mostly uses RAW Sql)
//  2. "ORM": It is an abstraction layer that connects object oriented programming (OOP) to relational databases
//     2.1: "Entity Framework": it's an ORM that uses ADO.NET internally and has EF migrations and support LINQ. 
//            Full Definition: Entity Framework (EF) Core is a full-featured ORM framework providing a high-level abstraction over the database.
//            EF Core allows you to work with databases using LINQ queries, which makes it easy to write and read data from the database,
//            also EF Core also supports many databases
//     2.1: "Dapper": it's a 'Micro-ORM' that uses ADO.NET internally and support raw sql. 
//            Full Definition: Dapper is a simple and lightweight ORM framework that allows you to execute SQL queries and map the results to .NET objects.
//            Dapper uses raw SQL queries, which means that you have full control over the SQL that is executed. This makes Dapper fast and efficient,
//            especially when dealing with large datasets. Additionally, Dapper is easy to use and has a small learning curve
//
// 2) EF Core vs Dapper vs ADO.Net :
//    1. EF uses 'LINQ', Dapper and ADO.net uses 'Raw SQL' and they can't use LINQ (Although ef can have sql and dapper can have sql generation
//        , but in general they don't have) -> (to execute sql in EF use these functions "FromSql","FromSqlRaw" of 'DbSet'
//          and "SqlQuery","SqlQueryRaw" and "ExecuteSql", "ExecuteSqlRaw" from 'DbContext.Database')
//    2. EF and Dapper will 'Map queries to objects' since they are 'ORM', but ADO.Net can't do that 
//       it means, in 'ORM's The columns as created as properties in model class, So the Intellisense offers columns of the table as properties,
//       while writing the code. Plus, the developer need not convert data types of values; it's automatically done by ORM itself.
//    3. The CRUD operations are done with shorter amount of code in orms, ranking in order with longest to shortest:
//       ADO.Net > Dapper > EF Core , so EF is the best
//    4. In terms of 'Performance, because ADzO.Net and Dapper use SQL, they have better performance, ranking in order with best to worst:
//       ADO.Net > Dapper > EF Core , SO ADO.NET and Dapper are recommended for larger & high-traffic applications.
//
// 3) In EF Core we have two main approaches:
//     1. "CodeFirst": You will write asp.net code first and the models (as tables), properties (as columns) and constraints,... will be represented 
//                     as database with executing 'Migration' commands and to update or change anything in database you should use 'migration'.
//                     in short terms -> "define entity model classes and the database will be created by applying migrations"
//                     and "Code will define database"
//         - Suitable for newer databases.
//         - Manual changes to DB will be most probably lost because your code defines the database.
//         - Stored procedures are to be written as a part of C# code.
//         - Suitable for smaller applications or prototype-level applications only; but not for larger or high data-intense applications.
//     2. "DbFirst ": You will design the database first with sql and the models (as tables), properties (as columns) will be represented as them,
//                    and to update or change anything in database you should use 'sql' in DBMS itself. 
//                    in short terms -> "define entity model classes based on an existing database"
//         - Suitable if you have an existing database or DB designed by DBAs, developed separately.
//         - Manual changes to DB can be done independently.
//         - All DBMS objects such as Stored procedures, indexes, triggers etc., can be created with SQL independently.
//         - Suitable for larger applications and high data-intense applications.
//    *Important*: EF Core has better support for 'CodeFirst' approach, maybe if the performance is really matter,
//                 the 'DbFirst' approach with Dapper would be a better choice
//
// 4) The reason to define constructor with " public ... (DbContextOptions<...> options) : base(options) " signature is that when we add our 'DbContext'
//    as a service in 'Program.cs' file, we define an 'option' and use 'UseSqlServer' of that, to be able to that we have to define this constructor,
//    if we don't do that we will have an compiler error
//
// 5) The Steps to 'Connect' our code to 'Database':
//     1. Create a database and save its connection string ("Datasource=.....")
//     2. Install Package 'Microsoft.EntityFrameworkCore.SqlServer' for accessing to 'DbContext' in 'Entities' Project, (maybe needed for startup project too)
//     3. Define a class that inherits from 'DbContext' and has some 'DbSet' properties of some models (The class represent as our database, each DbSet of models
//        as tables and each property of each model as columns of tables)
//     4. Define a constructor with " public ... (DbContextOptions<...> options) : base(options) " signature (if not there will be an error)
//     5. (Optionally) Define 'OnModelCreating' method and change the names of tables and maybe add some seed data.
//     6. in 'Program.cs' file, use 'AddDbContext' to add our 'DbContext' as a service, then use 'UseSqlServer' with parameter of the previous
//        connection string that we mentioned in '1.'.In this way everything is ready before applying migrations. it is best practice to put the connection string
//        into a json file like 'appsettings.json' in [ConnectionStrings:DefaultConnection].
//     7. Install Some Packages because the 'Migration' commands need those packages:
//        7.1 'Microsoft.EntityFrameworkCore.Design' in startup project (the actual project with program.cs)
//        7.2 'Microsoft.EntityFrameworkCore.Tools' in entities project (the project containing the 'DbContext' class)
//     8. To actually create tables we need to apply two 'Migration' commands. These two are always needed each time for updating database
//        8.1 First we write the "Add-Migration" command following a 'name', for exp -> "Add-Migration Initial", --> this will create a .cs file in migrations folder
//            that has generated some C# code in EF framework to be converted to SQL.
//        8.2 Second we write the "Update-Database" command, --> this will generate the SQL statements based on last migration file and will be added in database
//     9. The Connection is done, we can inject the 'DbContext' to our 'Services' with the help of DI, and use 'LINQ to Entities' against that
//
// 6)
//     6.1 The "FromSql" method:
//         allows us to write raw SQL, however it can't be used so dynamic, (because it is safe for SQL Injection attack), some examples:
//           1. This can be done : (Yes)
//                var user = "John";
//                var blogs = context.Blogs
//                                   .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
//                                   .ToList();
//           2. This can be done too: (Yes)
//                var user = new SqlParameter("user", "John");
//                var blogs = context.Blogs
//                                   .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser {user}")
//                                   .ToList();
//           3. But this can't be done (throw an exception): (No)
//                var propertyName = "User";
//                var propertyValue = "John";
//                var blogs = context.Blogs
//                                   .FromSql($"SELECT * FROM [Blogs] WHERE {propertyName} = {propertyValue}")
//                                   .ToList();
//         The number '3.' will throw an exception, since databases do not allow parameterizing column names (or any other part of the schema)
//         FromSql and its parameterization should be used wherever possible (due to safety against SQL Injection). However, there are certain scenarios 
//        where SQL needs  to be dynamically pieced together, and database parameters cannot be used. In Such scenarios:
//     6.2 The "FromSqlRaw" method:
//         will help us,If you've decided to dynamically construct your SQL, you'll have to use FromSqlRaw,
//         which allows interpolating variable data directly into the SQL string, instead of using a database parameter,
//         Be very careful when using FromSqlRaw, and always make sure values are either from a safe origin, or are properly sanitized. 
//         SQL injection attacks can have disastrous consequences for your application
//   **Important**: The Differences between "FromSql" and "FromSqlRaw" is exactly applied between
//                  - "ExecuteSql" and "ExecuteSqlRaw" and
//                  - "SqlQuery" and "SqlQueryRaw"
//
// 7) 1. !!Runtime EXCEPTION!! --> With The "FromSql" method    "List<Person> persons = Persons.FromSql($"{sp_GetAllPersons}").ToList();"
//    2.  but this works fine  --> With The "FromSqlRaw" method "List<Person> persons = Persons.FromSqlRaw($"{sp_GetAllPersons}").ToList();"
//
// 8) When to use "FromSql" or "ExecuteSql" methods:
//     In general no matter u are gonna execute a procedure,function or a query or command,
//     1. If the SQL statement has return values (like 'query with select' or 'sp with select'), you have to use "FromSql"
//     2. If the SQL statement doesn't have return values (like 'Insert,Update,Delete commands' or 'sp with 'IUD' commands'), you have to use "ExecuteSql"
//
// 9) The "FromSqlInterpolated" method is just older version of "FromSql" and they are the same, same with "ExecuteSql" and "ExecuteSqlInterpolated"
//
// 10) The Steps to use 'Stored Procedures' in EF Core:
//     - Creation Part
//        1. First we have to add a migration (better to be empty migration) 
//        2. Write the Raw Sql to create a procedure in our database in 'Up' method of migration file and drop procedure in 'Down' method
//        3. Use Update-Database command to apply changes to database and our stored procedure(s) will be added in database
//     - Execution Part
//        1. We can execute the raw sql in our services, but for reusability better to write a method for that in DbContext class,
//           So you have to write a method in 'DbContext' class 
//        2. In the method, write the Raw Sql to execute the required stored procedure, and if it has a parameter or parameters,
//           better to use 'SqlParameter' class for them
//        3. Then call that method from "DbContext" in your "Service" class (in case of 'Insert,Update,Delete', there is no need for 'SaveChanges',
//           since we've written the Sql ourselves and 'SaveChanges' is not needed to do that for us)
//
// 11)  EF Core uses a metadata model to describe how the application's entity types are mapped to the underlying database.
//      This model is built using a set of conventions - heuristics that look for common patterns. The model can then be customized using mapping attributes
//      (also known as 'Data Annotations') and/or calls to the ModelBuilder methods (also known as 'Fluent API') in 'OnModelCreating',
//      both of which will override the configuration performed by conventions.
//      In 'Fluent API' we can configure our database (tables,columns,...), but for some of them we have other ways too, for exp
//          - for specifying 'Primary Key' (here 'ID' of person)
//               1. Add "[Key]" in top of 'ID' property in 'Person' Entity
//               2. Use "HasKey" method of modelBuilder like this -> "modelBuilder.Entity<Person>().HasKey(entity => entity.ID);"
//          - for specifying a column is 'Not Nullable'
//               1. Add "[Required]" in top of property in Entity class
//               2. Add "[DisallowNull]" in top of property in Entity class
//               3. Use "IsRequired(true)" method of modelBuilder like this -> "modelBuilder.Entity<Person>().Property(entity => entity.TIN).IsRequired(required:true);"
//               4. Make the datatype of property 'Not Nullable', means without '?' suffix like this --> "string", "int", "DateTime"
//          - for specifying a column is 'Nullable'
//               1. Add "[AllowNull]" in top of property in Entity class
//               2. Use "IsRequired(false)" method of modelBuilder like this -> "modelBuilder.Entity<Person>().Property(entity => entity.TIN).IsRequired(required:false);"
//               3. Make the datatype of property 'Nullable', means with '?' suffix like this --> "string?", "int?", "DateTime?"

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Core.Domain.Entities;
using Core.Domain.IdentityEntities;


namespace Infrastructure.DbContext;

public class PersonsDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
{
    public virtual DbSet<Person> Persons { get; set; }
    public virtual DbSet<Country> Countries { get; set; }

    public PersonsDbContext(DbContextOptions<PersonsDbContext> options) : base(options)
    {
    }


    //
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Seed_Data
        
        // Seed Data for Country //

        // Read the json file into a string
        string countriesSeed_Path = @"C:\Visual Studio\Asp.Net Core MVC\With_Repository\With_Repository\wwwroot\JSON\seed_countries.json";
        string countriesSeed_Json = File.ReadAllText(countriesSeed_Path);
            
        // Deserialize the json file to 'a List of Country'
        List<Country> countriesSeed_List = JsonSerializer.Deserialize<List<Country>>(countriesSeed_Json)!;

        // inserting one row in table per 'each Country object in List'
        modelBuilder.Entity<Country>().HasData(countriesSeed_List);


        // Seed Data for Person //

        // Read the json file into a string
        string personsSeed_Path = @"C:\Visual Studio\Asp.Net Core MVC\With_Repository\With_Repository\wwwroot\JSON\seed_persons.json";
        string personsSeed_Json = File.ReadAllText(personsSeed_Path);

        // Deserialize the json file to 'a List of Person'
        List<Person> personsSeed_List = JsonSerializer.Deserialize<List<Person>>(personsSeed_Json)!;

        // inserting one row in table per 'each Person object in List'
        modelBuilder.Entity<Person>().HasData(personsSeed_List);

            
        // Seed Data for Roles (user,admin) //
            
        List<ApplicationRole> rolesList =
        [
            new ApplicationRole() { Id = Guid.Parse("211D03AE-07FE-4CB4-813E-163F46568B44"),Name = "User", NormalizedName = "USER", 
                ConcurrencyStamp = "4A191780-214E-47D6-83B8-4345A61E804C"},
            new ApplicationRole() { Id = Guid.Parse("A9C2BC35-61FE-4E60-8158-2BFD6E1478EB"),Name = "Admin", NormalizedName = "ADMIN", 
                ConcurrencyStamp = "7DB6149D-F1EE-4A0B-BA7B-D99E41414D73"}
        ];
        modelBuilder.Entity<ApplicationRole>().HasData(rolesList);

        
        #endregion

        #region FluenAPI_Configuration
        
        // -
        modelBuilder.Entity<Country>().ToTable("Countries");
        modelBuilder.Entity<Person>().ToTable("Persons", tablebuilder => tablebuilder.HasCheckConstraint("CHK_TINumber", "len([TINumber]) = 11"));

        modelBuilder.Entity<Person>().Property(entity => entity.TIN)
            .HasColumnName("TINumber")
            .HasColumnType("varchar(11)")
            .HasDefaultValue("ABCDEFGH123");

            
        // -
        modelBuilder.Entity<Person>()
                    .HasOne(e => e.Country)
                    .WithMany(e => e.Persons)
                    .HasForeignKey(e => e.CountryID)
                    .IsRequired();
        
        #endregion    
    }


    #region Stored_Procedures

    //
    public async Task<List<Person>> SP_GetAllPersons()
    {
        List<Person> persons = await Persons.FromSql($" EXECUTE dbo.GetAllPersons ").ToListAsync();
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
            new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
        };
        
        int numOfRowsAffected = await Database.ExecuteSqlRawAsync(" EXECUTE dbo.AddPerson @ID, @Name, @Email, @DateOfBirth, @Gender, " +
                                                                              "@CountryID, @Address, @ReceiveNewsLetters", parameters);

        return numOfRowsAffected;
    }
    
    
    #endregion
}

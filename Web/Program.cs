using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;

using Core.Domain.IdentityEntities;
using Core.Domain.RepositoryContracts;
using Core.ServiceContracts;
using Core.Services;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Web.Middlewares;


namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });
        
        // Serilog 'Logging'
        builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => 
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration) // read configuration settings from built-in IConfiguration
                .ReadFrom.Services(services); // read out current app's services and make them available to serilog
        } );
         
        
         // http logging
        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                    HttpLoggingFields.ResponsePropertiesAndHeaders;
        });
        
        
        
        // Services IOC 
        builder.Services.AddScoped<ICountriesService, CountriesService>();
        builder.Services.AddScoped<IPersonsService, PersonsService>();
        
        builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
        builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

        // DataBase IOC
        var DBconnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<PersonsDbContext>
        (options =>
        {
            options.UseSqlServer(DBconnectionString);
        });
        
        
        // Identity IOC
        builder.Services.AddIdentity<ApplicationUser,ApplicationRole>(options => 
        {
            options.Password.RequiredLength = 5;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = false;
            options.Password.RequiredUniqueChars = 3; //Eg: AB12AB
        }).AddEntityFrameworkStores<PersonsDbContext>()
          .AddUserStore<UserStore<ApplicationUser,ApplicationRole,PersonsDbContext,Guid>>()
          .AddRoleStore<RoleStore<ApplicationRole,PersonsDbContext,Guid>>()
          .AddDefaultTokenProviders();
                                       
                       
        
        // Authorization
         // configure a policy to authorization
        builder.Services.AddAuthorization(options =>
        {
            // enforces authorization policy (user must be authenticated) for all the action methods
            var policyBuilder = new AuthorizationPolicyBuilder();
            var policy = policyBuilder.RequireAuthenticatedUser().Build(); 
            options.FallbackPolicy = policy;
            
            // add a custom policy to be used in 'AccountController'
            options.AddPolicy("NotAuthorized", custompolicy =>
            {
                custompolicy.RequireAssertion(context =>
                {
                      return !context.User.Identity?.IsAuthenticated ?? false;
                });
            });
        });
         // mention the login path to redirect if there is no authorization
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login"; // the default is "/Account/Login" too 
        });
        
        
        
        var app = builder.Build();
        
        
        // Middlewares
        
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseCustomExceptionHandlingMiddleware();
        }

        app.UseHsts();
        app.UseHttpsRedirection();
        
        app.UseSerilogRequestLogging();   
        app.UseHttpLogging();
        
        app.UseStaticFiles();

        app.UseRouting(); // Identifying action method based on route
        app.UseAuthentication(); // Reading Identity cookie
        app.UseAuthorization(); // Validates access permissions of the user
        app.MapControllers(); // Execute the filter pipeline (action + filters)
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name : "areas",
                pattern : "{area:exists}/{controller=Persons}/{action=Index}/{id?}"
            );
        });
            
        app.Run();
    }
}

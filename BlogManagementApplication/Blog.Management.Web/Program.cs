using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Management.Infrastructure.DbContexts;
using Blog.Management.Infrastructure.Extensions;
using Blog.Management.Web.AutofacModule;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Reflection;

namespace Blog.Management.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Bootstrap Logger
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); ;
            var migrationAssembly = Assembly.GetExecutingAssembly().FullName;
            if (string.IsNullOrEmpty(migrationAssembly))
            {
                throw new InvalidOperationException("Migration assembly not found.");
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "ApplicationLogs", AutoCreateSqlTable = true })
                .ReadFrom.Configuration(configuration)
                .CreateBootstrapLogger();
            #endregion

            try
            {
                Log.Information("Application Starting...");

                var builder = WebApplication.CreateBuilder(args);

                #region Serilog Configuration
                builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
                {
                    loggerConfiguration.MinimumLevel.Debug()
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sinkOptions: new MSSqlServerSinkOptions { TableName = "ApplicationLogs", AutoCreateSqlTable = true })
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(builder.Configuration);

                });
                #endregion

                // Add services to the container.

                //Dbcontext register...
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, (x) => x.MigrationsAssembly(migrationAssembly)));

                builder.Services.AddDbContext<BlogManagementDbContext>(options =>
                options.UseSqlServer(connectionString, (x) => x.MigrationsAssembly(migrationAssembly)));

                //This is Autofac service...
                builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
                builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
                {
                    containerBuilder.RegisterModule(new WebModule(connectionString, migrationAssembly));
                });


                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                //This is my extension method here have all identity related configuration...
                builder.Services.AddIdentity();
        
                builder.Services.AddControllersWithViews();


                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseMigrationsEndPoint();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                await app.RunAsync();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Application Crashed...");
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}

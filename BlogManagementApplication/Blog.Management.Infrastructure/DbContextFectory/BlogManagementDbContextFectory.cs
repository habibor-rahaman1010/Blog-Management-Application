using Blog.Management.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Blog.Management.Infrastructure.DbContextFectory
{
    public class BlogManagementDbContextFectory : IDesignTimeDbContextFactory<BlogManagementDbContext>
    {
        public BlogManagementDbContext CreateDbContext(string[] args)
        {
            try
            {
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Blog.Management.Web");

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var migrationAssembly = typeof(BlogManagementDbContext).Assembly.GetName().Name;

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
                }

                if (string.IsNullOrEmpty(migrationAssembly))
                {
                    throw new InvalidOperationException("Migration assembly name could not be determined.");
                }

                return new BlogManagementDbContext(connectionString, migrationAssembly);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating BlogManagementDbContext: {ex.Message}");
                throw;
            }
        }
    }
}

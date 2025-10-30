using Microsoft.EntityFrameworkCore;

namespace Blog.Management.Infrastructure.DbContexts
{
    public class BlogManagementDbContext : DbContext
    {
        private readonly string _connectionString;
        private readonly string _migrationAssembly;

        public BlogManagementDbContext(string connectionString, string migrationAssembly)
        {
            _connectionString = connectionString;
            _migrationAssembly = migrationAssembly;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString,
                    x => x.MigrationsAssembly(_migrationAssembly));
            }

            base.OnConfiguring(optionsBuilder);
        }

    }
}

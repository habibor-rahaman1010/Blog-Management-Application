using Blog.Management.Domain.Entities;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Infrastructure.DbContexts;


namespace Blog.Management.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category, Guid>, ICategoryRepository
    {
        private readonly BlogManagementDbContext _dbContext;

        public CategoryRepository(BlogManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

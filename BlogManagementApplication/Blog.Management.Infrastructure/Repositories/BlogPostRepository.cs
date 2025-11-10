using Blog.Management.Domain.Entities;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Infrastructure.DbContexts;

namespace Blog.Management.Infrastructure.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, Guid>, IBlogPostRepository
    {
        private readonly BlogManagementDbContext _dbContext;

        public BlogPostRepository(BlogManagementDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

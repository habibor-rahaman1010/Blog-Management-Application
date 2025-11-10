using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Microsoft.EntityFrameworkCore;

namespace Blog.Management.Infrastructure.UnitOfWork
{
    public class ApplicationUnitOfWork<TDbContext> : UnitOfWork<TDbContext>, IApplicationUnitOfWork where TDbContext : DbContext
    {
        public ICategoryRepository CategoryRepository { get; private set; }
        public IBlogPostRepository BlogPostRepository { get; private set; }

        public ApplicationUnitOfWork(TDbContext dbContext,
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository) : base(dbContext)
        {
            BlogPostRepository = blogPostRepository;
            CategoryRepository = categoryRepository;
        }
    }
}

using Blog.Management.Domain.Entities;

namespace Blog.Management.Domain.RepositoryInterfaces
{
    public interface IBlogPostRepository : IGenericRepository<BlogPost, Guid>
    {

    }
}

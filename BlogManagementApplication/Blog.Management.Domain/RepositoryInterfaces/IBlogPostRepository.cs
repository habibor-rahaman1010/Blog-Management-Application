using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.QueryParams;
using Blog.Management.Domain.Utilities;

namespace Blog.Management.Domain.RepositoryInterfaces
{
    public interface IBlogPostRepository : IGenericRepository<BlogPost, Guid>
    {
        public Task<PagedWithResult<BlogPostSPDto>> GetBlogPostsByStoredProcedure(int pageIndex, int pageSize, BlogPostQueryparams request);
    }
}

using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.Utilities;

namespace Blog.Management.Application.ServiceInterfaces
{
    public interface IBlogPostManagementService
    {
        public Task<PagedWithResult<BlogPostDto>> GetBlogPostsAsync(string title, int pageIndex, int pageSize);
        public Task<BlogPostDto> GetBlogPostByIdAsync(Guid id);
        public Task<BlogPostDto> CreateBlogPostAsync(BlogPostCreateDto CreateRequestDto);
        public Task<BlogPostDto> UpdateBlogPostAsync(Guid id, BlogPostUpdateDto updateRequestDto);
        public Task<bool> DeleteBlogPostAsync(Guid id);
    }
}
using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Blog.Management.Application.Services
{
    public class BlogPostManagementService : IBlogPostManagementService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IApplicationTime _applicationTime;
        private readonly ILogger<BlogPostManagementService> _logger;

        public BlogPostManagementService(
            IApplicationUnitOfWork unitOfWork,
            IMapper mapper,
            IApplicationTime applicationTime,
            ILogger<BlogPostManagementService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationTime = applicationTime;
            _logger = logger;
        }

        public async Task<BlogPostDto> CreateBlogPostAsync(BlogPostCreateDto createRequestDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var blogPost = await _mapper.From(createRequestDto).AdaptToTypeAsync<BlogPost>();
                blogPost.Author = "Habibor Rahaman";
                blogPost.CreatedAt = _applicationTime.GetCurrentDateTime();

                await _unitOfWork.BlogPostRepository.AddAsync(blogPost);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return await _mapper.From(blogPost).AdaptToTypeAsync<BlogPostDto>();
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(dbEx, "Database error while creating blog post.");
                throw new InvalidOperationException("An error occurred while saving the blog post to the database.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Unexpected error in CreateBlogPostAsync.");
                throw;
            }
        }

        public async Task<BlogPostDto> UpdateBlogPostAsync(Guid id, BlogPostUpdateDto updateRequestDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var blogPost = await _unitOfWork.BlogPostRepository.GetByIdAsync(id);
                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post not found for update: {blogPostId}", id);
                    return new BlogPostDto();
                }

                _mapper.Map(updateRequestDto, blogPost);
                blogPost.UpdatedAt = _applicationTime.GetCurrentDateTime();

                await _unitOfWork.BlogPostRepository.UpdateAsync(blogPost);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                return await _mapper.From(blogPost).AdaptToTypeAsync<BlogPostDto>();
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(dbEx, "Database error while updating blog post with ID {blogPostId}", id);
                throw new InvalidOperationException("An error occurred while updating the blog post.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Unexpected error updating blog post {blogPostId}", id);
                throw new ApplicationException($"Unable to update blog post with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteBlogPostAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.BlogPostRepository.DeleteAsync(x => x.Id == id);
                var affected = await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                if (affected > 0)
                {
                    return true;
                }

                return false;
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(dbEx, "Database constraint error while deleting blog post ID {blogPostId}", id);
                throw new InvalidOperationException("Cannot delete this blog post because it is referenced by other entities.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Unexpected error deleting blog post {blogPostId}", id);
                throw new ApplicationException($"Unable to delete blog post with ID {id}.", ex);
            }
        }

        public async Task<BlogPostDto> GetBlogPostByIdAsync(Guid id)
        {
            try
            {
                var blogPost = await _unitOfWork.BlogPostRepository.GetByIdAsync(id);
                if (blogPost != null)
                {
                    return await _mapper.From(blogPost).AdaptToTypeAsync<BlogPostDto>();
                }

                _logger.LogWarning("Blog post not found: {blogPostId}", id);
                return new BlogPostDto();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Blog post not found: {blogPostId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post by ID: {blogPostId}", id);
                throw new ApplicationException($"Unable to retrieve blog post with ID {id}.", ex);
            }
        }

        public async Task<PagedWithResult<BlogPostDto>> GetBlogPostsAsync(string? searchQuery, int pageIndex, int pageSize)
        {
            try
            {
                Expression<Func<BlogPost, bool>>? filter = null;

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    filter = (x) =>
                        x.Title.Contains(searchQuery) ||
                        x.Content.Contains(searchQuery) ||
                        x.Author.Contains(searchQuery) ||
                        (x.Category != null && x.Category.Name.Contains(searchQuery));
                }

                var blogPosts = await _unitOfWork.BlogPostRepository.GetPagedListAsync(filter, null, null, pageIndex, pageSize);
                if (blogPosts != null && blogPosts.Items.Any())
                {
                    return await _mapper.From(blogPosts).AdaptToTypeAsync<PagedWithResult<BlogPostDto>>();
                }

                return new PagedWithResult<BlogPostDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching blog posts list.");
                throw new ApplicationException("Unable to retrieve blog posts.", ex);
            }
        }
    }
}

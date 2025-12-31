using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Domain.QueryParams;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Management.Web.Controllers
{
    public class PublicBlogPostController : Controller
    {
        private readonly IBlogPostManagementService _blogPostManagementService;
        private readonly ICategoryManagementService _categoryManagementService;

        public PublicBlogPostController(IBlogPostManagementService blogPostManagementService,
            ICategoryManagementService categoryManagementService)
        {
            _blogPostManagementService = blogPostManagementService;
            _categoryManagementService = categoryManagementService;
        }

        public async Task<IActionResult> BlogPostList(string? searchQuery, int pageIndex = 1, int pageSize = 12)
        {
            try
            {
                ViewBag.SearchQuery = searchQuery ?? "";

                var blogPosts = await _blogPostManagementService.GetBlogPostsAsync(searchQuery!, pageIndex, pageSize);
                if (blogPosts != null)
                {
                    return View(blogPosts);
                }
                return View(new List<BlogPostDto>());
            }

            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occired: ", ex);
            }
        }

        public async Task<IActionResult> BlogPostListSP(int pageIndex = 1, int pageSize = 12, BlogPostQueryparams request = null)
        {
            try
            {
                var blogPosts = await _blogPostManagementService.GetBlogPostsBySPAsync(pageIndex, pageSize, request);
                if (blogPosts != null)
                {
                    return View(blogPosts);
                }
                return View(new List<BlogPostSPDto>());
            }

            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occired: ", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> BlogDetails(Guid id)
        {
            try
            {
                var blogPost = await _blogPostManagementService.GetBlogPostByIdAsync(id);
                var category = await _categoryManagementService.GetCategoryByIdAsync(blogPost.CategoryId);
                blogPost.CategoryName = category.Name;
                return View(blogPost);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occired: ", ex);
            }
        }
    }
}

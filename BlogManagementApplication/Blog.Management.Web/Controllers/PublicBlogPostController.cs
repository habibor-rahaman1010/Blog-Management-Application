using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Blog.Management.Web.Controllers
{
    public class PublicBlogPostController : Controller
    {
        private readonly IBlogPostManagementService _blogPostManagementService;

        public PublicBlogPostController(IBlogPostManagementService blogPostManagementService)
        {
            _blogPostManagementService = blogPostManagementService;
        }

        public async Task<IActionResult> BlogPostList(int pageIndex = 1, int pageSize = 12)
        {
            try
            {
                var blogPosts = await _blogPostManagementService.GetBlogPostsAsync(pageIndex, pageSize);
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
    }
}

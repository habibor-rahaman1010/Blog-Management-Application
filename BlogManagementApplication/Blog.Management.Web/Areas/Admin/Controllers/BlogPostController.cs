using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Domain.Utilities;
using Blog.Management.Web.CustomActionFilters;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Management.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogPostController : Controller
    {
        private readonly IBlogPostManagementService _blogPostManagementService;
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly IFileService _fileService;
        private readonly ILogger<BlogPostController> _logger;
        private readonly IMapper _mapper;

        public BlogPostController(IBlogPostManagementService blogPostManagementService,
            IFileService fileService,
            ICategoryManagementService categoryManagementService,
            IMapper mapper,
            ILogger<BlogPostController> logger)
        {
            _mapper = mapper;
            _fileService = fileService;
            _blogPostManagementService = blogPostManagementService;
            _categoryManagementService = categoryManagementService;
            _logger = logger;
        }

        public async Task<IActionResult> BlogPostList(int pageIndex = 1, int pageSize = 10)
        {
            var blogPost = await _blogPostManagementService.GetBlogPostsAsync(pageIndex, pageSize);
            return View(blogPost);
        }

        [HttpGet]
        public async Task<IActionResult> BlogPostCreate()
        {
            try
            {
                var model = new BlogPostCreateDto();
                model.SetCategoryValues(await _categoryManagementService.GetAllCategory());
                return View(model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken, ValidateModel]
        public async Task<IActionResult> BlogPostCreate(BlogPostCreateDto request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                if (request.CoverImageFile != null)
                {
                    var result = _fileService.SaveImage(request.CoverImageFile);
                    if (result.Item1 == 1)
                    {                     
                        request.CoverImageUrl = result.Item2;
                    }
                }

                await _blogPostManagementService.CreateBlogPostAsync(request); 
                return RedirectToAction(nameof(BlogPostList));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }
    }
}

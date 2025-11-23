using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
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
        private readonly ILogger<BlogPostController> _logger;
        private readonly IMapper _mapper;

        public BlogPostController(IBlogPostManagementService blogPostManagementService,
            ICategoryManagementService categoryManagementService,
            IMapper mapper,
            ILogger<BlogPostController> logger)
        {
            _mapper = mapper;
            _blogPostManagementService = blogPostManagementService;
            _categoryManagementService = categoryManagementService;
            _logger = logger;
        }

        public IActionResult BlogList()
        {
            return View();
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
                throw new NotFiniteNumberException();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }
    }
}

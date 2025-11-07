using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Web.CustomActionFilters;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Management.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryManagementService _categoryManagementService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryManagementService categoryManagementService,
            IMapper mapper,
            ILogger<CategoryController> logger)
        {
            _categoryManagementService = categoryManagementService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CategoryList(int pageIndex = 1)
        {
            var categoris = await _categoryManagementService.GetCategoriesAsync(pageIndex, 10);
            return View(categoris);
        }

        [HttpGet]
        [ValidateModel]
        public IActionResult CreateCategory()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new ApplicationException();
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto category)
        {
            try
            {
                await _categoryManagementService.CreateCategoryAsync(category);
                return RedirectToAction(nameof(CategoryList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw new ApplicationException();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CategoryUpdate(Guid id)
        {
            try
            {
                var category = await _categoryManagementService.GetCategoryByIdAsync(id);
                var categoryUpdateDto = await _mapper.From(category).AdaptToTypeAsync<CategoryUpdateDto>();
                if (category == null)
                {
                    return NotFound();
                }
                return View(categoryUpdateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for update");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ValidateModel]
        public async Task<IActionResult> CategoryUpdate(Guid id, CategoryUpdateDto category)
        {
            try
            {
                await _categoryManagementService.UpdateCategoryAsync(id, category);
                return RedirectToAction(nameof(CategoryList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading category for update");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

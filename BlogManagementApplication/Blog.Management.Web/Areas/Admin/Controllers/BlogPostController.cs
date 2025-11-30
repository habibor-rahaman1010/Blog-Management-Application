using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.Extensions;
using Blog.Management.Web.Areas.Admin.Models;
using Blog.Management.Web.CustomActionFilters;
using Mapster;
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

        public async Task<IActionResult> BlogPostList(int pageIndex = 1, int pageSize = 2)
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
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog post created successfuly",
                    Type = ResponseTypes.Success
                });
                return RedirectToAction(nameof(BlogPostList));
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog post creation failed",
                    Type = ResponseTypes.Danger
                });
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> BlogPostUpdate(Guid id)
        {
            try
            {
                var existingPost = await _blogPostManagementService.GetBlogPostByIdAsync(id);
                var blogPostUpdateDto = await _mapper.From(existingPost).AdaptToTypeAsync<BlogPostUpdateDto>();
                blogPostUpdateDto.SetCategoryValues(await _categoryManagementService.GetAllCategory());
                if (existingPost == null)
                {
                    return NotFound();
                }
                return View(blogPostUpdateDto);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpPost, ValidateModel]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> BlogPostUpdate(Guid id, BlogPostUpdateDto request)
        {
            try
            {
                var existing = await _blogPostManagementService.GetBlogPostByIdAsync(id);

                if (request.CoverImageFile != null)
                {
                    var result = _fileService.SaveImage(request.CoverImageFile);
                    if (result.Item1 == 1)
                    {
                        var oldImage = existing.CoverImageUrl;
                        request.CoverImageUrl = result.Item2;
                        var isDeleted = _fileService.DeleteImage(oldImage);
                    }
                }
                else
                {
                    request.CoverImageUrl = existing.CoverImageUrl;
                }

                await _blogPostManagementService.UpdateBlogPostAsync(id, request);
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog post updated successfuly",
                    Type = ResponseTypes.Success
                });
                return RedirectToAction(nameof(BlogPostList));

            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Blog post update failed",
                    Type = ResponseTypes.Danger
                });
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }
    }
}
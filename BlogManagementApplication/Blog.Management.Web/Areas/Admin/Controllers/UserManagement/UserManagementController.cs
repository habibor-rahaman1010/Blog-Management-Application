using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.ApplicationIdentity;
using Blog.Management.Web.Areas.Admin.Models;
using Blog.Management.Web.Areas.Admin.Models.UserManagementModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Management.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Management.Web.Areas.Admin.Controllers.UserManagement
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IApplicationTime _applicationTime;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            IApplicationTime applicationTime,
            ILogger<UserManagementController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationTime = applicationTime;
            _logger = logger;
        }

        public async Task<IActionResult> UserRoleList(int page = 1, int pageSize = 10)
        {
            var totalRoles = await _roleManager.Roles.CountAsync();

            var roles = await _roleManager.Roles
            .OrderBy(r => r.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRoles / (double)pageSize);

            return View(roles);
        }

        [Authorize(Policy = "SuperAdminOnly")]
        public IActionResult CreateRole()
        {
            var model = new RoleCreateModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleCreateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleManager.CreateAsync(new ApplicationRole
                    {
                        Id = Guid.NewGuid(),
                        Name = model.RoleName,
                        NormalizedName = model.RoleName.ToUpper(),
                        ConcurrencyStamp = _applicationTime.GetCurrentDateTime().Ticks.ToString(),
                    });

                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "The Role has been created successfully!",
                        Type = ResponseTypes.Success
                    });

                    return RedirectToAction(nameof(UserRoleList));
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "The Role creation has failed!",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "Ultimately the Role creation failed!");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApplicationUser(int page = 1, int pageSize = 10)
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();

                var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);


                return View(users);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occurred: ", ex);
            }
        }
    }
}

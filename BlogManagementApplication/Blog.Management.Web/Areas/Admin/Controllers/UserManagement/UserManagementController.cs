using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.ApplicationIdentity;
using Blog.Management.Web.Areas.Admin.Models;
using Blog.Management.Web.Areas.Admin.Models.UserManagementModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Management.Infrastructure.Extensions;

namespace Blog.Management.Web.Areas.Admin.Controllers.UserManagement
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationTime _applicationTime;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IApplicationTime applicationTime,
            ILogger<UserManagementController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _applicationTime = applicationTime;
            _logger = logger;
        }

        public async Task<IActionResult> UserRoleList()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        //This is method for new role create...
        //[Authorize(Policy = "CustomAdminAccess")]
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
                        Message = "The Role has been created successfuly!",
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
                    _logger.LogError(ex, "Ultimatly the Role creation failed!");
                }
            }
            return View(model);
        }
    }
}

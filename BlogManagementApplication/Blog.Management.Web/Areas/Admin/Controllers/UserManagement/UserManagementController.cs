using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.ApplicationIdentity;
using Blog.Management.Infrastructure.Extensions;
using Blog.Management.Web.Areas.Admin.Models;
using Blog.Management.Web.Areas.Admin.Models.UserManagementModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        [HttpGet]
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
        public async Task<IActionResult> UpdateUserRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role not found!",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction(nameof(UserRoleList));
            }

            var model = new RoleUpdateModel
            {
                Id = Guid.Parse(role.Id.ToString()),
                RoleName = role.Name!
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(RoleUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(model.Id.ToString());
                    if (role == null)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "Role not found!",
                            Type = ResponseTypes.Danger
                        });
                        return RedirectToAction(nameof(UserRoleList));
                    }

                    role.Name = model.RoleName;
                    role.NormalizedName = model.RoleName.ToUpper();
                    role.ConcurrencyStamp = _applicationTime.GetCurrentDateTime().Ticks.ToString();

                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = "The Role has been updated successfully!",
                            Type = ResponseTypes.Success
                        });

                        return RedirectToAction(nameof(UserRoleList));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "Role update failed!",
                        Type = ResponseTypes.Danger
                    });
                    _logger.LogError(ex, "An error occurred while updating the role.");
                }
            }

            return View(model);
        }

        [HttpPost, AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteUserRole(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role not found!",
                    Type = ResponseTypes.Danger
                });
                return RedirectToAction(nameof(UserRoleList));
            }

            try
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    TempData.Put("ResponseMessage", new ResponseModel
                    {
                        Message = "The Role has been deleted successfully!",
                        Type = ResponseTypes.Success
                    });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        TempData.Put("ResponseMessage", new ResponseModel
                        {
                            Message = error.Description,
                            Type = ResponseTypes.Danger
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData.Put("ResponseMessage", new ResponseModel
                {
                    Message = "Role deletion failed!",
                    Type = ResponseTypes.Danger
                });
                _logger.LogError(ex, "An error occurred while deleting the role.");
            }

            return RedirectToAction(nameof(UserRoleList));
        }


        [HttpGet]
        public IActionResult ChangeUserRole()
        {
            try
            {
                var model = new UserRoleChangeModel();
                LoadValues(model);
                return View(model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(UserRoleChangeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                    var roles = await _userManager.GetRolesAsync(user!);
                    await _userManager.RemoveFromRolesAsync(user!, roles);
                    var newUserRole = await _roleManager.FindByIdAsync(model.RoleId.ToString());
                    await _userManager.AddToRoleAsync(user!, newUserRole!.Name!);
                }
                LoadValues(model);
                return RedirectToAction(nameof(GetAllApplicationUser));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        private void LoadValues(UserRoleChangeModel model)
        {
            try
            {
                var usersList = _userManager.Users
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Email })
                .ToList();
                usersList.Insert(0, new SelectListItem { Value = "", Text = "--Select A User--" });

                var rolesList = _roleManager.Roles
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();
                rolesList.Insert(0, new SelectListItem { Value = "", Text = "--Select A Role--" });

                model.Users = new List<SelectListItem>(usersList);
                model.Roles = new List<SelectListItem>(rolesList);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        //This is code for get list of claim
        [HttpGet]
        public async Task<IActionResult> UserClaimsList(int page = 1, int pageSize = 10)
        {
            var usersWithClaims = new List<UserClaimListModel>();

            var totalUsers = _userManager.Users.Count();

            var users = _userManager.Users
                                    .OrderBy(u => u.UserName)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);

                usersWithClaims.Add(new UserClaimListModel
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Claims = claims
                });
            }

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            return View(usersWithClaims);
        }


        // This code for claim create
        [HttpGet]
        public IActionResult AddUserClaim()
        {
            try
            {
                var model = new AddUserClaimModel();
                model.Users = new SelectList(from c in _userManager.Users select c, "Id", "UserName");
                return View(model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserClaim(AddUserClaimModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                    if (user != null)
                    {
                        await _userManager.AddClaimAsync(user, new Claim(model.ClaimName, model.ClaimValue));
                    }
                    model.Users = new SelectList(from c in _userManager.Users select c, "Id", "UserName");
                    return RedirectToAction(nameof(UserClaimsList));
                }

                model.Users = new SelectList(from c in _userManager.Users select c, "Id", "UserName");
                return View(model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserClaimEdit(Guid userId, string claimType, string claimValue)
        {
            if (userId.ToString() == null || claimType == null || claimValue == null)
            {
                return BadRequest("Invalid claim edit request.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Populate the model with the original claim type and value
            var model = new UserClaimEditModel
            {
                UserId = userId,
                OriginalClaimType = claimType,
                OriginalClaimValue = claimValue,
                NewClaimType = claimType,
                NewClaimValue = claimValue
            };

            return View(model);
        }

        //This method for claim edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UserClaimEdit(UserClaimEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Remove the original claim
            var oldClaim = new Claim(model.OriginalClaimType, model.OriginalClaimValue);
            var removeResult = await _userManager.RemoveClaimAsync(user, oldClaim);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Add the new claim with updated type and value
            var newClaim = new Claim(model.NewClaimType, model.NewClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, newClaim);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(UserClaimsList));
        }


        //This code for delete claim of a user
        public async Task<IActionResult> UserClaimDelete(Guid userId, string claimType, string claimValue)
        {
            if (userId.ToString() == null || claimType == null || claimValue == null)
            {
                return BadRequest("Invalid claim deletion request.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var claim = new Claim(claimType, claimValue);
            var result = await _userManager.RemoveClaimAsync(user, claim);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(UserClaimsList));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("ListUserClaims", await GetUsersWithClaims());
        }

        private async Task<List<UserClaimListModel>> GetUsersWithClaims()
        {
            var usersWithClaims = new List<UserClaimListModel>();
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                usersWithClaims.Add(new UserClaimListModel
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    UserName = user.UserName!,
                    Claims = claims
                });
            }

            return usersWithClaims;
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

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(currentUser);

            var userViewModel = new UserProfileViewModel
            {
                User = currentUser,
                Roles = roles
            };

            return View(userViewModel);
        }
    }
}

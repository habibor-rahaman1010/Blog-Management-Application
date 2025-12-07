using Blog.Management.Infrastructure.ApplicationIdentity;
using Blog.Management.Web.Areas.Admin.Controllers;
using Blog.Management.Web.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Blog.Management.Web.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly ILogger<AuthenticationController> _logger;


        public AuthenticationController(ApplicationSignInManager signInManager,
            ApplicationUserManager userManager,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        //--------User Registration Code-----------

        public async Task<IActionResult> UserRegistration(string returnUrl = null)
        {
            try
            {
                var model = new RegistrationModel();
                model.ReturnUrl = returnUrl;
                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Exception Occurred: ", ex);
            }
        }


        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<IActionResult> UserRegistration(RegistrationModel model)
        {
            model.ReturnUrl ??= Url.Content("~/");
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Email,
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Member");

                    var claimResult = await _userManager.AddClaimAsync(user, new Claim("Read", "true"));
                    if (!claimResult.Succeeded)
                    {
                        foreach (var error in claimResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("BlogPostList", "PublicBlogPost");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}

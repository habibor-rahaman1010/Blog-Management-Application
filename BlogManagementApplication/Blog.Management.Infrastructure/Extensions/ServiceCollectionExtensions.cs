using Blog.Management.Infrastructure.ApplicationIdentity;
using Blog.Management.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Management.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddIdentity(this IServiceCollection services)
        {
            //This service for Application Identity user
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddSignInManager<ApplicationSignInManager>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });


            //add policy role configuration
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });

                options.AddPolicy("SupportAccess", policy =>
                {
                    policy.RequireRole("Admin", "Support");
                });

                options.AddPolicy("MemberAccess", policy =>
                {
                    policy.RequireRole("Admin", "Support", "Member");
                });
            });


            //add Claim base authentication configuration
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ReadPermission", policy =>
                {
                    policy.RequireClaim("Read", "true");
                });

                options.AddPolicy("CreatePermission", policy =>
                {
                    policy.RequireClaim("Create", "true");
                });

                options.AddPolicy("UpdatePermission", policy =>
                {
                    policy.RequireClaim("Update", "true");
                });

                options.AddPolicy("DeletePermission", policy =>
                {
                    policy.RequireClaim("Delete", "true");
                });
            });

        }
    }
}
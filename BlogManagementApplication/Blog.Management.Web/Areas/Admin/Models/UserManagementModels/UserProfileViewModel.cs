using Blog.Management.Infrastructure.ApplicationIdentity;

namespace Blog.Management.Web.Areas.Admin.Models.UserManagementModels
{
    public class UserProfileViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }

        public UserProfileViewModel()
        {
            User = new ApplicationUser();
            Roles = new List<string>();
        }
    }
}

using System.Security.Claims;

namespace Blog.Management.Web.Areas.Admin.Models.UserManagementModels
{
    public class UserClaimListModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<Claim> Claims { get; set; }

        public UserClaimListModel()
        {
            Claims = new List<Claim>();
        }
    }
}

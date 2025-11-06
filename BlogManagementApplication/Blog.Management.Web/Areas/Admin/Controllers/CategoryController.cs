using Microsoft.AspNetCore.Mvc;

namespace Blog.Management.Web.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult CategoryList()
        {
            return View();
        }
    }
}

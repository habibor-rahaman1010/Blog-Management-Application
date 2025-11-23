using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Management.Infrastructure.Utilities
{
    public class RazorUtility
    {
        public static IList<SelectListItem> ConvertCategories(IEnumerable<CategoryDto> categories)
        {
            var items = (from category in categories
                         select new SelectListItem(category.Name, category.Id.ToString()))
                         .ToList();

            items.Insert(0, new SelectListItem("---Select A Category---", string.Empty));

            return items;
        }
    }
}

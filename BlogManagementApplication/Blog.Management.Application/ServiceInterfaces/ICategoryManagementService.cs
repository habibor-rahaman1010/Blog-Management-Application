using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Domain.Utilities;

namespace Blog.Management.Application.ServiceInterfaces
{
    public interface ICategoryManagementService
    {
        public Task<PagedWithResult<CategoryDto>> GetCategoriesAsync(int pageIndex, int pageSize);
        public Task<IEnumerable<CategoryDto>> GetAllCategory();
        public Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        public Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto CreateRequestDto);
        public Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryUpdateDto updateRequestDto);
        public Task<bool> DeleteCategoryAsync(Guid id);
    }
}
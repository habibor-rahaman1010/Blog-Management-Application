using Blog.Management.Application.ApplicationDtos.CategorisDtos;

namespace Blog.Management.Application.ServiceInterfaces
{
    public interface ICategoryManagementService
    {
        public Task<List<CategoryDto>> GetCategoriesAsync();
        public Task<CategoryDto> GetCategoryByIdAsync(Guid id);
        public Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto CreateRequestDto);
        public Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryUpdateDto updateRequestDto);
        public Task<bool> DeleteCategoryAsync(Guid id);
    }
}
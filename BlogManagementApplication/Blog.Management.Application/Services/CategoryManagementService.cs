using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Application.ServiceInterfaces;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Management.Application.Services
{
    public class CategoryManagementService : ICategoryManagementService
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private ILogger<CategoryManagementService> _logger;
        private readonly IApplicationTime _applicationTime;
        private readonly IMapper _mapper;

        public CategoryManagementService(IApplicationUnitOfWork unitOfWork, IMapper mapper,
            IApplicationTime applicationTime,
            ILogger<CategoryManagementService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _applicationTime = applicationTime;
            _logger = logger;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryCreateDto CreateRequestDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var category = await _mapper.From(CreateRequestDto).AdaptToTypeAsync<Category>();
                category.CreatedBy = "Habibor Rahaman";
                category.CreatedAt = _applicationTime.GetCurrentTime();

                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitAsync();
                return await _mapper.From(category).AdaptToTypeAsync<CategoryDto>();
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(dbEx, "Database error while creating category.");
                throw new InvalidOperationException("An error occurred while saving category to the database.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Unexpected error in CreateCategoryAsync.");
                throw;
            }
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.CategoryRepository.DeleteAsync(x => x.Id == id);
                var aknowledge = await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                if (aknowledge > 0)
                {
                    return true;
                }
                return false;
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(dbEx, "Database constraint error while deleting category ID {categoryId}", id);
                throw new InvalidOperationException("Cannot delete this category because it is referenced by other entities.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(ex, "Unexpected error deleting category {categoryId}", id);
                throw new ApplicationException($"Unable to delete category with ID {id}.", ex);
            }
        }

        public async Task<PagedWithResult<CategoryDto>> GetCategoriesAsync(int pageIndex, int pageSize)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetPagedListAsync(null, null, null, pageIndex, pageSize);
                if (categories != null && categories.Items.Any() != false)
                {
                    return await _mapper.From(categories).AdaptToTypeAsync<PagedWithResult<CategoryDto>>();
                }

                return new PagedWithResult<CategoryDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all caegoris.");
                throw new ApplicationException("Unable to retrieve category list.", ex);
            }
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category != null)
                {
                    return await _mapper.From(category).AdaptToTypeAsync<CategoryDto>();
                }

                return new CategoryDto();
            }
            catch (KeyNotFoundException knfEx)
            {
                _logger.LogWarning(knfEx, "Category not found: {categoryId}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category by ID: {categoryId}", id);
                throw new ApplicationException($"Unable to retrieve category with ID {id}.", ex);
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryUpdateDto updateRequestDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new CategoryDto();
                }

                //updateRequestDto.Adapt(category); -> Other way to maped!

                _mapper.Map(updateRequestDto, category);
                category.UpdatedAt = _applicationTime.GetCurrentTime();

                await _unitOfWork.CategoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return await _mapper.From(category).AdaptToTypeAsync<CategoryDto>();
            }
            catch (DbUpdateException dbEx)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(dbEx, "Database error while updating category with ID {categoryId}", id);
                throw new InvalidOperationException("An error occurred while updating the category.", dbEx);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                _logger.LogError(ex, "Unexpected error updating category {categoryId}", id);
                throw new ApplicationException($"Unable to update category with ID {id}.", ex);
            }
        }
    }
}
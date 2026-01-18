using System.Linq.Expressions;
using Blog.Management.Application.ApplicationDtos.CategorisDtos;
using Blog.Management.Application.Services;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Blog.Management.Tests.Application.Services
{
    public class CategoryManagementServiceTests
    {
        private readonly Mock<IApplicationUnitOfWork> _uowMock = new();
        private readonly Mock<dynamic> _categoryRepoMock = new();
        private readonly Mock<IApplicationTime> _timeMock = new();
        private readonly Mock<ILogger<CategoryManagementService>> _loggerMock = new();
        private readonly IMapper _mapper;

        public CategoryManagementServiceTests()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<CategoryCreateDto, Category>();
            config.NewConfig<CategoryUpdateDto, Category>();
            config.NewConfig<Category, CategoryDto>();

            config.NewConfig<PagedWithResult<Category>, PagedWithResult<CategoryDto>>();

            _mapper = new Mapper(config);
        }

        private Mock<ICategoryRepository> BuildCategoryRepoMock()
        {
            return new Mock<ICategoryRepository>();
        }

        private CategoryManagementService BuildService(Mock<ICategoryRepository> repoMock)
        {
            _uowMock.Reset();

            _uowMock.SetupGet(x => x.CategoryRepository).Returns(repoMock.Object);

            _uowMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            _timeMock.Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2026, 01, 18, 10, 0, 0, DateTimeKind.Utc));

            return new CategoryManagementService(
                _uowMock.Object,
                _mapper,
                _timeMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateCategoryAsync_Success_ShouldCommit_AndReturnDto()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            Category? savedEntity = null;

            repoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => savedEntity = c)
                .Returns(Task.CompletedTask);

            var dto = new CategoryCreateDto
            {

            };

            // Act
            var result = await sut.CreateCategoryAsync(dto);

            // Assert - transaction flow
            _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);
            _uowMock.Verify(x => x.RollbackAsync(), Times.Never);

            // Assert - entity set
            Assert.NotNull(savedEntity);
            Assert.Equal("Habibor Rahaman", savedEntity!.CreatedBy);
            Assert.Equal(new DateTime(2026, 01, 18, 10, 0, 0, DateTimeKind.Utc), savedEntity.CreatedAt);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateCategoryAsync_DbUpdateException_ShouldRollback_AndThrowInvalidOperation()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            // SaveChanges throws DbUpdateException
            _uowMock.Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("db fail"));

            var dto = new CategoryCreateDto();

            // Act + Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateCategoryAsync(dto));

            _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(x => x.RollbackAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
            Assert.Contains("saving category", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenAffectedGreaterThanZero_ShouldReturnTrue()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            repoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(Task.CompletedTask);

            _uowMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(2);

            var id = Guid.NewGuid();

            // Act
            var ok = await sut.DeleteCategoryAsync(id);

            // Assert
            Assert.True(ok);
            _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);
            _uowMock.Verify(x => x.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCategoryAsync_DbUpdateException_ShouldRollback_AndThrowInvalidOperation()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            repoMock.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(Task.CompletedTask);

            _uowMock.Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("constraint"));

            var id = Guid.NewGuid();

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteCategoryAsync(id));

            _uowMock.Verify(x => x.RollbackAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WhenFound_ShouldReturnDto()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(new Category
                {
                    Id = id,
                    // Name = "Tech"
                });

            // Act
            var dto = await sut.GetCategoryByIdAsync(id);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_WhenNotFound_ShouldReturnEmptyDto()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

            // Act
            var dto = await sut.GetCategoryByIdAsync(id);

            // Assert
            Assert.NotNull(dto);
            Assert.Equal(Guid.Empty, dto.Id);
        }

        [Fact]
        public async Task UpdateCategoryAsync_WhenNotFound_ShouldReturnEmptyDto()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            var id = Guid.NewGuid();
            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Category?)null);

            // Act
            var dto = await sut.UpdateCategoryAsync(id, new CategoryUpdateDto());

            // Assert
            Assert.NotNull(dto);

            _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Never);
            _uowMock.Verify(x => x.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateCategoryAsync_Success_ShouldCommit_AndSetUpdatedAt()
        {
            // Arrange
            var repoMock = BuildCategoryRepoMock();
            var sut = BuildService(repoMock);

            var id = Guid.NewGuid();
            var entity = new Category { Id = id };

            repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

            var updateDto = new CategoryUpdateDto
            {
            };

            // Act
            var dto = await sut.UpdateCategoryAsync(id, updateDto);

            // Assert
            _uowMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uowMock.Verify(x => x.SaveChangesAsync(), Times.Once);
            _uowMock.Verify(x => x.CommitAsync(), Times.Once);
            _uowMock.Verify(x => x.RollbackAsync(), Times.Never);

            Assert.Equal(new DateTime(2026, 01, 18, 10, 0, 0, DateTimeKind.Utc), entity.UpdatedAt);
            Assert.NotNull(dto);
        }
    } 
}
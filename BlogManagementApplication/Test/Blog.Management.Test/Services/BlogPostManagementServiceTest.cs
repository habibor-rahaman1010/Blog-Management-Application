using System.Linq.Expressions;
using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Application.Services;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.QueryParams;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.UnitOfWorkInterface;
using Blog.Management.Domain.Utilities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;

namespace Blog.Management.Tests.Application.Services
{
    public class BlogPostManagementServiceTests
    {
        private readonly Mock<IApplicationUnitOfWork> _uow = new();
        private readonly Mock<ILogger<BlogPostManagementService>> _logger = new();
        private readonly Mock<IApplicationTime> _time = new();
        private readonly IMapper _mapper;

        public BlogPostManagementServiceTests()
        {
            var config = new TypeAdapterConfig();

            config.NewConfig<BlogPostCreateDto, BlogPost>();
            config.NewConfig<BlogPostUpdateDto, BlogPost>();
            config.NewConfig<BlogPost, BlogPostDto>();
            config.NewConfig<PagedWithResult<BlogPost>, PagedWithResult<BlogPostDto>>();
            config.NewConfig<PagedWithResult<BlogPostSPDto>, PagedWithResult<BlogPostSPDto>>();

            _mapper = new Mapper(config);

            _time.Setup(t => t.GetCurrentDateTime())
                 .Returns(new DateTime(2026, 01, 18, 0, 0, 0, DateTimeKind.Utc));
        }

        private BlogPostManagementService CreateSut(Mock<IBlogPostRepository> repo)
        {
            _uow.SetupGet(x => x.BlogPostRepository).Returns(repo.Object);
            _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            return new BlogPostManagementService(
                _uow.Object,
                _mapper,
                _time.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task CreateBlogPostAsync_Success_ShouldBeginSaveCommit_AndReturnDto()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            BlogPost? captured = null;

            repo.Setup(r => r.AddAsync(It.IsAny<BlogPost>()))
            .Callback<BlogPost>(bp => captured = bp)
            .Returns(Task.CompletedTask);

            var input = new BlogPostCreateDto
            {

            };

            // Act
            var dto = await sut.CreateBlogPostAsync(input);

            // Assert
            _uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
            _uow.Verify(x => x.RollbackAsync(), Times.Never);

            Assert.NotNull(captured);
            Assert.Equal("Habibor Rahaman", captured!.Author);
            Assert.Equal(new DateTime(2026, 01, 18, 0, 0, 0, DateTimeKind.Utc), captured.CreatedAt);
            Assert.NotNull(dto);
        }

        [Fact]
        public async Task CreateBlogPostAsync_DbUpdateException_ShouldRollback_AndThrowInvalidOperationException()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            _uow.Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("db fail"));

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CreateBlogPostAsync(new BlogPostCreateDto()));

            _uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uow.Verify(x => x.RollbackAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBlogPostAsync_WhenNotFound_ShouldReturnEmptyDto_AndNotCommit()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            var id = Guid.NewGuid();
            repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((BlogPost?)null);

            // Act
            var dto = await sut.UpdateBlogPostAsync(id, new BlogPostUpdateDto());

            // Assert
            Assert.NotNull(dto);

            _uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
            _uow.Verify(x => x.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateBlogPostAsync_Success_ShouldUpdate_Save_Commit_AndSetUpdatedAt()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            var id = Guid.NewGuid();
            var entity = new BlogPost { Id = id, Title = "Old" };

            repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

            repo.Setup(r => r.UpdateAsync(It.IsAny<BlogPost>())).Returns(Task.CompletedTask);

            // Act
            var dto = await sut.UpdateBlogPostAsync(id, new BlogPostUpdateDto
            {

            });

            // Assert
            _uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
            _uow.Verify(x => x.RollbackAsync(), Times.Never);

            Assert.Equal(new DateTime(2026, 01, 18, 0, 0, 0, DateTimeKind.Utc), entity.UpdatedAt);
            Assert.NotNull(dto);
        }

        [Fact]
        public async Task DeleteBlogPostAsync_WhenAffectedGreaterThanZero_ShouldReturnTrue()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            repo.Setup(r => r.DeleteAsync(It.IsAny<Expression<Func<BlogPost, bool>>>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(2);

            // Act
            var ok = await sut.DeleteBlogPostAsync(Guid.NewGuid());

            // Assert
            Assert.True(ok);
            _uow.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Once);
            _uow.Verify(x => x.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteBlogPostAsync_DbUpdateException_ShouldRollback_AndThrowInvalidOperationException()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            _uow.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new DbUpdateException("constraint"));

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.DeleteBlogPostAsync(Guid.NewGuid()));

            _uow.Verify(x => x.RollbackAsync(), Times.Once);
            _uow.Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task GetBlogPostByIdAsync_WhenFound_ShouldReturnDto()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            var id = Guid.NewGuid();
            repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new BlogPost { Id = id, Title = "Hello" });

            // Act
            var dto = await sut.GetBlogPostByIdAsync(id);

            // Assert
            Assert.NotNull(dto);
        }

        [Fact]
        public async Task GetBlogPostsAsync_WithSearchQuery_ShouldReturnPagedDto()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            var paged = new PagedWithResult<BlogPost>
            {
                Items = new List<BlogPost>
                {
                    new BlogPost { Id = Guid.NewGuid(), Title = "C# Tips", Content = "C# is a object oriented program!" }
                },
                Total = 1
            };

            repo.Setup(r => r.GetPagedListAsync(
                    It.IsAny<Expression<Func<BlogPost, bool>>?>(),
                    It.IsAny<Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>>?>(),
                    It.IsAny<Func<IQueryable<BlogPost>, IIncludableQueryable<BlogPost, object>>?>(),
                    1,
                    10,
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(paged);

            // Act
            var result = await sut.GetBlogPostsAsync("C#", 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.True(result.Items.Any());
        }

        [Fact]
        public async Task GetBlogPostsAsync_WithoutSearchQuery_ShouldPassNullFilter()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            repo.Setup(r => r.GetPagedListAsync(
                    null,
                    It.IsAny<Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>>?>(),
                    It.IsAny<Func<IQueryable<BlogPost>, IIncludableQueryable<BlogPost, object>>?>(),
                    1,
                    10,
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                ))
                .ReturnsAsync(new PagedWithResult<BlogPost>
                {
                    Items = new List<BlogPost>(),
                    Total = 0
                });

            // Act
            var result = await sut.GetBlogPostsAsync(null, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Items);
        }

        [Fact]
        public async Task GetBlogPostsBySPAsync_WhenHasItems_ShouldReturnPagedSpDto()
        {
            // Arrange
            var repo = new Mock<IBlogPostRepository>();
            var sut = CreateSut(repo);

            var spPaged = new PagedWithResult<BlogPostSPDto>
            {
                Items = new List<BlogPostSPDto> { new BlogPostSPDto() },
                Total = 1
            };

            repo.Setup(r => r.GetBlogPostsByStoredProcedure(1, 10, It.IsAny<BlogPostQueryparams>()))
                .ReturnsAsync(spPaged);

            // Act
            var result = await sut.GetBlogPostsBySPAsync(1, 10, new BlogPostQueryparams());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Items.Any());
        }
    }
}
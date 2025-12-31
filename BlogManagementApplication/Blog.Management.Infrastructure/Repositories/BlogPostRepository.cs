using Blog.Management.Application.ApplicationDtos.BlogPostDtos;
using Blog.Management.Domain.Entities;
using Blog.Management.Domain.QueryParams;
using Blog.Management.Domain.RepositoryInterfaces;
using Blog.Management.Domain.Utilities;
using Blog.Management.Infrastructure.DbContexts;

namespace Blog.Management.Infrastructure.Repositories
{
    public class BlogPostRepository : GenericRepository<BlogPost, Guid>, IBlogPostRepository
    {
        private readonly BlogManagementDbContext _dbContext;
        private readonly ISqlUtility _sqlUtility;

        public BlogPostRepository(BlogManagementDbContext dbContext, ISqlUtility sqlUtility) : base(dbContext)
        {
            _dbContext = dbContext;
            _sqlUtility = sqlUtility;
        }

        public async Task<PagedWithResult<BlogPostSPDto>> GetBlogPostsByStoredProcedure(int pageIndex, int pageSize, BlogPostQueryparams request)
        {
            try
            {
                var procedureName = "usp_blogpost_filter";
                var parameters = new Dictionary<string, object>()
                {
                    {"AuthorName", request.AuthorName!},
                    {"CategoryName", request.CategoryName!},
                    {"Fromdate", request.FromDate!},
                    {"Todate ", request.Todate!},
                    {"SearchKeyWord ", request.SearchKeyWord!},
                    {"PageNumber ", pageIndex},
                    {"PageSize ", pageSize},
                };

                var outputParamter = new Dictionary<string, Type>
                {
                    { "Total", typeof(int) },
                    { "TotalDisplay", typeof(int) },
                };

                var result = await _sqlUtility.QueryWithStoredProcedureAsync<BlogPostSPDto>(procedureName, parameters, outputParamter);

                var data = new PagedWithResult<BlogPostSPDto>()
                {
                    Items = result.result,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = (int)result.outValues["Total"],
                    TotalDisplay = (int)result.outValues["TotalDisplay"]
                };

                return data;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Exception Occured: ", ex);
            }
        }
    }
}
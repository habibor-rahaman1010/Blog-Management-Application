namespace Blog.Management.Domain.Utilities
{
    public interface IApplicationTime
    {
        public DateTime GetCurrentTime();
        public DateTime GetUtcNowTime();
    }
}

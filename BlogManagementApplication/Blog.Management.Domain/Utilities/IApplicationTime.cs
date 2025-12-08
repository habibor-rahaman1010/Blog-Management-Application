namespace Blog.Management.Domain.Utilities
{
    public interface IApplicationTime
    {
        public DateTime GetCurrentDateTime();
        public DateTime GetUtcNowDateTime();
    }
}

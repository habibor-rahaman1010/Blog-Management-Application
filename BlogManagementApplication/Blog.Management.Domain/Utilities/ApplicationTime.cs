namespace Blog.Management.Domain.Utilities
{
    public class ApplicationTime : IApplicationTime
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public DateTime GetUtcNowTime()
        {
            return DateTime.UtcNow;
        }
    }
}

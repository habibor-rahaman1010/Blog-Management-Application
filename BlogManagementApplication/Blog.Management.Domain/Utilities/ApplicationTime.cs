namespace Blog.Management.Domain.Utilities
{
    public class ApplicationTime : IApplicationTime
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public DateTime GetUtcNowDateTime()
        {
            return DateTime.UtcNow;
        }
    }
}

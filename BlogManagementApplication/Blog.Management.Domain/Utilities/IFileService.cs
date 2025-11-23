using Microsoft.AspNetCore.Http;

namespace Blog.Management.Domain.Utilities
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile imageFile);
        public bool DeleteImage(string imageFileName);
    }
}

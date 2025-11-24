using Blog.Management.Domain.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Blog.Management.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Tuple<int, string> SaveImage(IFormFile imageFile)
        {
            try
            {
                var wwwPath = _env.WebRootPath;
                var folderPath = Path.Combine(wwwPath, "Uploads", "BlogImages");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(extension))
                {
                    return new Tuple<int, string>(0, $"Only {string.Join(",", allowedExtensions)} extensions are allowed");
                }

                string fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return new Tuple<int, string>(1, fileName);
            }
            catch
            {
                return new Tuple<int, string>(0, "Error has occurred");
            }
        }

        public bool DeleteImage(string imageFileName)
        {
            try
            {
                var path = Path.Combine(_env.WebRootPath, "Uploads", "BlogImages", imageFileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
using Blog.Management.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace Blog.Management.Test.Services
{
    public class FileServiceTests
    {

        private readonly Mock<IWebHostEnvironment> _mockEnv;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _mockEnv = new Mock<IWebHostEnvironment>();
            _mockEnv.Setup(e => e.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            _fileService = new FileService(_mockEnv.Object);
        }

        [Fact]
        public void SaveImage_Should_Save_Jpg_Image_Successfully()
        {
            // Arrange
            var fileName = "test.jpg";
            var fileMock = new Mock<IFormFile>();
            var content = "Fake image content";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            // Act
            var result = _fileService.SaveImage(fileMock.Object);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.EndsWith(".jpg", result.Item2);

            // Clean up the file created during test
            var savedFilePath = Path.Combine(_mockEnv.Object.WebRootPath, "Uploads", "BlogImages", result.Item2);
            if (File.Exists(savedFilePath))
            {
                File.Delete(savedFilePath);
            }
        }

        [Fact]
        public void SaveImage_Should_Return_Error_For_Invalid_Extension()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.pdf");

            // Act
            var result = _fileService.SaveImage(fileMock.Object);

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Contains("Only", result.Item2);
        }

        [Fact]
        public void DeleteImage_Should_Return_True_When_File_Exists()
        {
            // Arrange
            var folder = Path.Combine(_mockEnv.Object.WebRootPath, "Uploads", "BlogImages");
            Directory.CreateDirectory(folder);
            var filePath = Path.Combine(folder, "testdelete.jpg");
            File.WriteAllText(filePath, "dummy content");

            // Act
            var result = _fileService.DeleteImage("testdelete.jpg");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DeleteImage_Should_Return_False_When_File_Does_Not_Exist()
        {
            // Act
            var result = _fileService.DeleteImage("nonexistent.jpg");

            // Assert
            Assert.False(result);
        }
    }
}
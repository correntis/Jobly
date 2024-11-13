using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using UsersService.Domain.Abstractions.Services;
using UsersService.Domain.Constants;

namespace UsersService.Application.Services
{
    public class ImagesService : IImagesService
    {
        private readonly string _contentRootPath;

        public ImagesService(IWebHostEnvironment environment)
        {
            _contentRootPath = environment.ContentRootPath;
        }

        public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file is null)
            {
                return null;
            }

            var folderPath = Path.Combine(_contentRootPath, BusinessRules.Image.Folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var fullPath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);

            await file.CopyToAsync(stream, cancellationToken);

            return fileName;
        }

        public void Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var fullPath = Path.Combine(_contentRootPath, BusinessRules.Image.Folder, fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Invalid file path {fullPath}");
            }

            File.Delete(fullPath);
        }
    }
}

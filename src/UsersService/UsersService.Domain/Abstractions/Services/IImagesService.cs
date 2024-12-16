using Microsoft.AspNetCore.Http;

namespace UsersService.Domain.Abstractions.Services
{
    public interface IImagesService
    {
        void Delete(string fileName);
        Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);
    }
}
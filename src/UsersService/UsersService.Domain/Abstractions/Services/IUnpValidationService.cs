namespace UsersService.Domain.Abstractions.Services
{
    public interface IUnpValidationService
    {
        Task<bool> ValidateUnpAsync(string unp, CancellationToken cancellationToken = default);
    }
}


namespace VacanciesService.Domain.Abstractions.Services
{
    public interface IUsersService
    {
        Task<bool> IsCompanyExistsAsync(Guid id, CancellationToken token = default);
        Task<bool> IsUserExistsAsync(Guid id, CancellationToken token = default);
    }
}
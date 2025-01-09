using VacanciesService.Domain.Models;

namespace VacanciesService.Domain.Abstractions.Services
{
    public interface IUsersService
    {
        Task<bool> IsCompanyExistsAsync(Guid id, CancellationToken token = default);
        Task<bool> IsUserExistsAsync(Guid id, CancellationToken token = default);
        Task<Resume> GetResumeAsync(string resumeId, CancellationToken token = default);
        Task<string> GetUserNameAsync(Guid userId, CancellationToken token = default);
        Task<List<Resume>> GetBestResumesAsync(List<string> skills, List<string> tags, List<Language> languages, CancellationToken token = default);
    }
}
namespace VacanciesService.Domain.Abstractions.Repositories
{
    public interface IWriteRepository
    {
        Task SaveChangesAsync(CancellationToken token = default);
    }
}

using System.Linq.Expressions;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;

namespace VacanciesService.Domain.Abstractions.Repositories
{
    public interface IVacanciesDetailsRepository
    {
        Task AddAsync(VacancyDetailsEntity entity, CancellationToken cancellationToken = default);
        Task DeleteByAsync<TValue>(Expression<Func<VacancyDetailsEntity, TValue>> field, TValue value, CancellationToken cancellationToken = default);
        Task<VacancyDetailsEntity> GetByAsync<TValue>(Expression<Func<VacancyDetailsEntity, TValue>> field, TValue value, CancellationToken cancellationToken = default);
        Task<List<VacancyDetailsEntity>> GetFilteredPageAsync(VacancyDetailsFilter detailsFilter, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task UpdateAsync(VacancyDetailsEntity entity, CancellationToken cancellationToken = default);
        Task UpdateByAsync<TValue>(string id, Expression<Func<VacancyDetailsEntity, object>> field, TValue value, CancellationToken cancellationToken = default);
    }
}
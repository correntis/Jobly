using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface ICompaniesRepository
    {
        Task AddAsync(CompanyEntity companyEntity, CancellationToken cancellationToken = default);
        void Update(CompanyEntity companyEntity);
        void Remove(CompanyEntity companyEntity);
        Task<CompanyEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CompanyEntity> GetWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> IsExists(Guid id, CancellationToken cancellationToken = default);
    }
}
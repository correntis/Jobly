using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface ICompaniesRepository
    {
        Task AddAsync(CompanyEntity companyEntity, CancellationToken cancellationToken = default);
        Task<CompanyEntity> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<CompanyEntity> GetWithIncludesAsync(int id, CancellationToken cancellationToken = default);
        void Remove(CompanyEntity companyEntity);
    }
}
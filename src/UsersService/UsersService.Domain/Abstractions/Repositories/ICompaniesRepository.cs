using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface ICompaniesRepository
    {
        Task AddAsync(CompanyEntity companyEntity, CancellationToken cancellationToken);
        Task<CompanyEntity> GetAsync(int id, CancellationToken cancellationToken);
        Task<CompanyEntity> GetWithIncludesAsync(int id, CancellationToken cancellationToken);
        void Remove(CompanyEntity companyEntity);
    }
}
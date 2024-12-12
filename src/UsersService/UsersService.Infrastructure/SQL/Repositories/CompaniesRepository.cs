using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Repositories
{
    public class CompaniesRepository : ICompaniesRepository
    {
        private readonly UsersDbContext _context;

        public CompaniesRepository(UsersDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CompanyEntity companyEntity, CancellationToken cancellationToken = default)
        {
            await _context.Companies.AddAsync(companyEntity, cancellationToken);
        }

        public void Update(CompanyEntity companyEntity)
        {
            _context.Companies.Update(companyEntity);
        }

        public void Remove(CompanyEntity companyEntity)
        {
            _context.Companies.Remove(companyEntity);
        }

        public async Task<CompanyEntity> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<CompanyEntity> GetWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}

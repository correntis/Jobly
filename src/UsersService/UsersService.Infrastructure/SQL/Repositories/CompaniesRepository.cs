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

        public async Task AddAsync(CompanyEntity companyEntity, CancellationToken cancellationToken)
        {
            await _context.Companies.AddAsync(companyEntity, cancellationToken);
        }

        public void Remove(CompanyEntity companyEntity, CancellationToken cancellationToken)
        {
            _context.Companies.Remove(companyEntity);
        }

        public async Task<CompanyEntity> Get(int id, CancellationToken cancellationToken)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<CompanyEntity> GetWithIncludes(int id, CancellationToken cancellationToken)
        {
            return await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}

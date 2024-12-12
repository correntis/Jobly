using UsersService.Domain.Abstractions.Repositories;
using UsersService.Infrastructure.NoSQL;
using UsersService.Infrastructure.NoSQL.Repositories;
using UsersService.Infrastructure.SQL;
using UsersService.Infrastructure.SQL.Repositories;

namespace UsersService.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsersDbContext _usersContext;

        public IUsersRepository UsersRepository { get; private set; }
        public IRolesRepository RolesRepository { get; private set; }
        public ICompaniesRepository CompaniesRepository { get; private set; }
        public IResumesRepository ResumesRepository { get; private set; }

        public UnitOfWork(
            UsersDbContext usersContext,
            MongoDbContext resumesContext,
            IUsersRepository usersRepository,
            IRolesRepository rolesRepository)
        {
            _usersContext = usersContext;

            CompaniesRepository = new CompaniesRepository(usersContext);
            ResumesRepository = new ResumesRepository(resumesContext);
            UsersRepository = usersRepository;
            RolesRepository = rolesRepository;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _usersContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _usersContext.Dispose();
        }
    }
}

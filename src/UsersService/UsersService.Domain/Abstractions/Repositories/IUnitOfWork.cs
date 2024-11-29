using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICompaniesRepository CompaniesRepository { get; }
        IResumesRepository ResumesRepository { get; }
        UserManager<UserEntity> UsersRepository { get; }
        RoleManager<RoleEntity> RolesRepository { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
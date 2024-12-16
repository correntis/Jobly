using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ICompaniesRepository CompaniesRepository { get; }
        IResumesRepository ResumesRepository { get; }
        IUsersRepository UsersRepository { get; }
        IRolesRepository RolesRepository { get; }

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
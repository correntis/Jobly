using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IRolesRepository
    {
        Task<IdentityResult> AddRole(RoleEntity roleEntity);
        Task<IdentityResult> DeleteRole(RoleEntity roleEntity);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
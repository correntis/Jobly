using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly RoleManager<RoleEntity> _roleManager;

        public RolesRepository(RoleManager<RoleEntity> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddRole(RoleEntity roleEntity)
        {
            return await _roleManager.CreateAsync(roleEntity);
        }

        public async Task<IdentityResult> DeleteRole(RoleEntity roleEntity)
        {
            return await _roleManager.DeleteAsync(roleEntity);
        }

        public Task<bool> RoleExistsAsync(string roleName)
        {
            return _roleManager.RoleExistsAsync(roleName);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<UserEntity> _userManager;

        public UsersRepository(UserManager<UserEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> AddAsync(UserEntity userEntity, string password)
        {
            return await _userManager.CreateAsync(userEntity, password);
        }

        public async Task<IdentityResult> UpdateAsync(UserEntity userEntity)
        {
            return await _userManager.UpdateAsync(userEntity);
        }

        public async Task<IdentityResult> DeleteAsync(UserEntity userEntity)
        {
            return await _userManager.DeleteAsync(userEntity);
        }

        public async Task<IdentityResult> AddToRolesAsync(UserEntity userEntity, IEnumerable<string> rolesNames)
        {
            return await _userManager.AddToRolesAsync(userEntity, rolesNames);
        }

        public async Task<UserEntity> GetByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<UserEntity> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetRolesAsync(UserEntity userEntity)
        {
            return await _userManager.GetRolesAsync(userEntity);
        }

        public async Task<bool> CheckPasswordAsync(UserEntity userEntity, string password)
        {
            return await _userManager.CheckPasswordAsync(userEntity, password);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken token = default)
        {
            return await _userManager.Users.AnyAsync(u => u.Id == id, cancellationToken: token);
        }
    }
}

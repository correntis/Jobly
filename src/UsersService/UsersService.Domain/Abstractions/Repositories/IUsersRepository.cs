using Microsoft.AspNetCore.Identity;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUsersRepository
    {
        Task<IdentityResult> AddAsync(UserEntity userEntity, string password);
        Task<IdentityResult> AddToRolesAsync(UserEntity userEntity, IEnumerable<string> rolesNames);
        Task<IdentityResult> UpdateAsync(UserEntity userEntity);
        Task<IdentityResult> DeleteAsync(UserEntity userEntity);
        Task<UserEntity> GetByEmailAsync(string email);
        Task<UserEntity> GetByIdAsync(Guid id);
        Task<IList<string>> GetRolesAsync(UserEntity userEntity);
        Task<bool> CheckPasswordAsync(UserEntity userEntity, string password);
        Task<bool> ExistsAsync(Guid id, CancellationToken token = default);
    }
}
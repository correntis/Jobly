using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUsersRepository
    {
        Task AddAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
        Task<UserEntity> GetAsync(int id, CancellationToken cancellationToken = default);
        Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        void Remove(UserEntity userEntity);
    }
}
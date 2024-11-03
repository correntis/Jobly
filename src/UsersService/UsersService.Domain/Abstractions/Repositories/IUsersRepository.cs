using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUsersRepository
    {
        Task AddAsync(UserEntity userEntity, CancellationToken cancellationToken);
        Task<UserEntity> GetAsync(int id, CancellationToken cancellationToken);
        Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken);
        void Remove(UserEntity userEntity);
    }
}
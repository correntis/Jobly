using UsersService.Domain.Entities.SQL;

namespace UsersService.Domain.Abstractions.Repositories
{
    public interface IUsersRepository
    {
        Task AddAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
        Task<UserEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        void Update(UserEntity userEntity);
        void Remove(UserEntity userEntity);
    }
}
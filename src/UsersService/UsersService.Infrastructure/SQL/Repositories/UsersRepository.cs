using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Abstractions.Repositories;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDbContext _context;

        public UsersRepository(UsersDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(userEntity, cancellationToken);
        }

        public void Update(UserEntity userEntity)
        {
            _context.Users.Update(userEntity);
        }

        public void Remove(UserEntity userEntity)
        {
            _context.Users.Remove(userEntity);
        }

        public async Task<UserEntity> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}

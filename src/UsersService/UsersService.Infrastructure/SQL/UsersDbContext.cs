using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
    {
        public DbSet<CompanyEntity> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}

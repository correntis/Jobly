using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<CompanyEntity> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}

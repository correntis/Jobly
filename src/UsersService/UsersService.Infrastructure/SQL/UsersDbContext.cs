using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UsersService.Domain.Entities.SQL;
using Guid = System.Guid;

namespace UsersService.Infrastructure.SQL
{
    public class UsersDbContext(DbContextOptions<UsersDbContext> options) : IdentityDbContext<UserEntity, RoleEntity, Guid>(options)
    {
        public DbSet<CompanyEntity> Companies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<RoleEntity>().HasData(GetRoles());
            modelBuilder.Entity<UserEntity>().HasData(GetUsers());
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(GetUserRoles());

            base.OnModelCreating(modelBuilder);
        }

        public static List<UserEntity> GetUsers()
        {
            return
            [
                new UserEntity
                {
                    Id = new Guid("01eb48d5-6f39-436f-b55a-a948f240241d"),
                    FirstName = "Maksim",
                    LastName = "Rusetski",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO2U87owvdt6cLGcTjrd0pM7Z5hm/5DFR1A8uPZjQh9wb7XCYWcKDxMfRSEmJ0ec6Q==",
                    ConcurrencyStamp = "4cac2a77-ba92-4925-8dc5-6ee76aefe20f",
                    SecurityStamp = "42JNKQDOFZBXPARN2I7WHT4MU236MCTK",
                },
                new UserEntity
                {
                    Id = new Guid("11eb48d5-6f39-436f-b55a-a948f240241d"),
                    FirstName = "Maksim",
                    LastName = "Rusetski",
                    Email = "company@example.com",
                    NormalizedEmail = "COMPANY@EXAMPLE.COM",
                    PasswordHash = "AQAAAAIAAYagAAAAEO2U87owvdt6cLGcTjrd0pM7Z5hm/5DFR1A8uPZjQh9wb7XCYWcKDxMfRSEmJ0ec6Q==",
                    ConcurrencyStamp = "5cac2a77-ba92-4925-8dc5-6ee76aefe20f",
                    SecurityStamp = "52JNKQDOFZBXPARN2I7WHT4MU236MCTK",
                },
            ];
        }

        public static List<RoleEntity> GetRoles()
        {
            return
            [
                new RoleEntity
                {
                    Id = new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"),
                    Name = "User",
                    NormalizedName = "USER",
                },
                new RoleEntity
                {
                    Id = new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"),
                    Name = "Company",
                    NormalizedName = "COMPANY",
                },
            ];
        }

        public static List<IdentityUserRole<Guid>> GetUserRoles()
        {
            return
            [
                new IdentityUserRole<Guid>
                {
                    UserId = new Guid("01eb48d5-6f39-436f-b55a-a948f240241d"),
                    RoleId = new Guid("356f9c63-9980-442f-a9d8-4a2ffdf41ea5"),
                },

                new IdentityUserRole<Guid>
                {
                    UserId = new Guid("11eb48d5-6f39-436f-b55a-a948f240241d"),
                    RoleId = new Guid("6658e4bf-c52c-4a4b-a66d-acad84e85ab3"),
                },
            ];
        }
    }
}

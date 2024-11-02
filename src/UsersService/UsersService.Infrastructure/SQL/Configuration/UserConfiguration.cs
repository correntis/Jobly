using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email);

            builder.Property(u => u.FirstName)
                .HasMaxLength(BusinessRules.User.MaxFirstNameLength)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(BusinessRules.User.MaxLastNameLength);

            builder.Property(u => u.Type)
                .HasMaxLength(BusinessRules.User.MaxTypeLength)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(BusinessRules.User.MaxEmailLength)
                .IsRequired();

            builder.Property(u => u.Phone)
                .HasMaxLength(BusinessRules.User.MaxPhoneLength);

            builder.Property(u => u.PasswordHash)
                .IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UsersService.Domain.Constants;
using UsersService.Domain.Entities.SQL;

namespace UsersService.Infrastructure.SQL.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<CompanyEntity>
    {
        public void Configure(EntityTypeBuilder<CompanyEntity> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Name);

            builder.HasIndex(c => c.Type);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");

            builder.Property(c => c.Name)
                .HasMaxLength(BusinessRules.Company.MaxNameLength)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(BusinessRules.Company.MaxDesctiptionLength);

            builder.Property(c => c.City)
                .HasMaxLength(BusinessRules.Company.MaxCityLength);

            builder.Property(c => c.Address)
                .HasMaxLength(BusinessRules.Company.MaxAddressLength);

            builder.Property(c => c.Email)
                .HasMaxLength(BusinessRules.Company.MaxEmailLength);

            builder.Property(c => c.Phone)
                .HasMaxLength(BusinessRules.Company.MaxPhoneLength);

            builder.Property(c => c.WebSite)
                .HasMaxLength(BusinessRules.Company.MaxWebSiteLength);

            builder.Property(c => c.Type)
                .HasMaxLength(BusinessRules.Company.MaxTypeLength)
                .IsRequired();

            builder.Property(c => c.Unp)
                .HasMaxLength(BusinessRules.Company.MaxUnpLength)
                .IsRequired();

            builder.HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<CompanyEntity>();
        }
    }
}

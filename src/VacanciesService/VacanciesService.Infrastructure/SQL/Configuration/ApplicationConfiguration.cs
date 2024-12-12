using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Configuration
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<ApplicationEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationEntity> builder)
        {
            builder.ToTable("Applications");

            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.UserId);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.Status)
                .HasMaxLength(BusinessRules.Application.StatusMaxLength)
                .IsRequired();

            builder.Property(a => a.AppliedAt)
                .HasColumnType("timestamp");

            builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamp")
                .HasConversion(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Local))
                .IsRequired();
        }
    }
}

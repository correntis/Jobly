using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Configuration
{
    public class VacancyConfiguration : IEntityTypeConfiguration<VacancyEntity>
    {
        public void Configure(EntityTypeBuilder<VacancyEntity> builder)
        {
            builder.ToTable("Vacancies");

            builder.HasKey(v => v.Id);

            builder.HasIndex(v => v.CompanyId);

            builder.HasMany(v => v.Applications)
                .WithOne(a => a.Vacancy);

            builder.Property(v => v.Id)
                .ValueGeneratedOnAdd();

            builder.Property(v => v.EmploymentType)
                .HasMaxLength(BusinessRules.Vacancy.EmployementTypeMaxLenght);

            builder.Property(v => v.Title)
                .HasMaxLength(BusinessRules.Vacancy.TitleMaxLength)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .HasColumnType("timestamp")
                .HasConversion(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Local))
                .IsRequired();

            builder.Property(v => v.DeadlineAt)
                .HasColumnType("timestamp")
                .HasConversion(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Local))
                .IsRequired();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Configuration
{
    public class InteractionConfiguration : IEntityTypeConfiguration<VacancyInteractionEntity>
    {
        public void Configure(EntityTypeBuilder<VacancyInteractionEntity> builder)
        {
            builder.ToTable("Interactions");

            builder.HasKey(i => i.Id);

            builder.HasIndex(i => i.UserId);
            builder.HasIndex(i => i.VacancyId);

            builder.Property(i => i.Id)
                .HasColumnType("uuid")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(i => i.CreatedAt)
                .HasColumnType("timestamptz")
                .HasConversion(
                    i => DateTime.SpecifyKind(i, DateTimeKind.Utc),
                    i => DateTime.SpecifyKind(i, DateTimeKind.Utc))
                .IsRequired();
        }
    }
}

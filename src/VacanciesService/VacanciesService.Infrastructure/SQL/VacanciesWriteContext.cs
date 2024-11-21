using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL
{
    public class VacanciesWriteContext(DbContextOptions<VacanciesWriteContext> options) : DbContext(options), IVacanciesWriteContext
    {
        public DbSet<VacancyEntity> Vacancies { get; set; }
        public DbSet<ApplicationEntity> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VacanciesWriteContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

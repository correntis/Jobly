using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL
{
    public class VacanciesReadContext(DbContextOptions<VacanciesReadContext> options) : DbContext(options), IVacanciesReadContext
    {
        public DbSet<VacancyEntity> Vacancies { get; set; }
        public DbSet<ApplicationEntity> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VacanciesReadContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

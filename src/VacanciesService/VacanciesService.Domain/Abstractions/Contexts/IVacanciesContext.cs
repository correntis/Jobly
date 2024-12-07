using Microsoft.EntityFrameworkCore;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Domain.Abstractions.Contexts
{
    public interface IVacanciesContext
    {
        DbSet<ApplicationEntity> Applications { get; set; }
        DbSet<VacancyEntity> Vacancies { get; set; }
        DbSet<VacancyInteractionEntity> Interactions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

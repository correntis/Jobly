using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Infrastructure.SQL.Repositories.Write
{
    public class WriteInteractionsRepository : IWriteInteractionsRepository
    {
        private readonly VacanciesWriteContext _vacanciesContext;

        public WriteInteractionsRepository(VacanciesWriteContext vacanciesContext)
        {
            _vacanciesContext = vacanciesContext;
        }

        public async Task AddAsync(VacancyInteractionEntity interactionEntity, CancellationToken token = default)
        {
            await _vacanciesContext.Interactions.AddAsync(interactionEntity, token);
        }

        public void RemoveRange(List<VacancyInteractionEntity> interactionsEntities)
        {
            _vacanciesContext.Interactions.RemoveRange(interactionsEntities);
        }

        public void Attach(VacancyInteractionEntity vacancyInteraction)
        {
            _vacanciesContext.Interactions.Attach(vacancyInteraction);
        }

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _vacanciesContext.SaveChangesAsync(token);
        }

        public void Update(VacancyInteractionEntity interactionEntity)
        {
            _vacanciesContext.Update(interactionEntity);
        }
    }
}

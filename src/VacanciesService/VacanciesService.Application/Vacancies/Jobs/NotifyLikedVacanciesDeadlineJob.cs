using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Application.Vacancies.Jobs
{
    public class NotifyLikedVacanciesDeadlineJob
    {
        private readonly ILogger<NotifyLikedVacanciesDeadlineJob> _logger;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IBrokerProcuder _brokerProcuder;

        public NotifyLikedVacanciesDeadlineJob(
            ILogger<NotifyLikedVacanciesDeadlineJob> logger,
            IReadInteractionsRepository readInteractionsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IBrokerProcuder brokerProcuder)
        {
            _logger = logger;
            _readInteractionsRepository = readInteractionsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _brokerProcuder = brokerProcuder;
        }

        public async Task ExecuteAsync()
        {
            var deadlineVacancies = await _readVacanciesRepository
                .GetByDeadlineAsync(BusinessRules.Vacancy.NotifyAboutDeadlineTime);

            var likeInteractions = await _readInteractionsRepository
                .GetLikedByVacanciesAsync(deadlineVacancies.Select(vacancy => vacancy.Id).ToList());

            var vacanciesMap = deadlineVacancies.ToDictionary(vacancy => vacancy.Id);

            var produceTasks = CreateProduceTasks(likeInteractions, vacanciesMap);

            await Task.WhenAll(produceTasks);

            _logger.LogInformation("[Broker] DeadlineVacancyEvent produced {Times} time/s", produceTasks.Count);
        }

        private List<Task> CreateProduceTasks(
            List<VacancyInteractionEntity> likeInteractions,
            Dictionary<Guid, VacancyEntity> vacanciesMap)
        {
            return likeInteractions.Select(async interaction =>
            {
                if (vacanciesMap.TryGetValue(interaction.VacancyId, out VacancyEntity vacancy))
                {
                    await ProduceVacancyDeadlineEvent(
                        interaction.UserId,
                        interaction.VacancyId,
                        vacancy.DeadlineAt,
                        vacancy.Title);
                }
            }).ToList();
        }

        private async Task ProduceVacancyDeadlineEvent(Guid userId, Guid vacancyId, DateTime deadline, string title)
        {
            var vacancyDeadlineEvent = new LikedVacancyDeadlineEvent()
            {
                UserId = userId,
                VacancyId = vacancyId,
                VacancyDeadlineAt = deadline,
                VacancyName = title,
            };

            await _brokerProcuder.SendAsync(vacancyDeadlineEvent);
        }
    }
}

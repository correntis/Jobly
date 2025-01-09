using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.Vacancies.Recommendations.ML;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Enums;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Jobs
{
    public class NotifyAboutVacancyBestMatchesJob
    {
        private readonly ILogger<NotifyAboutVacancyBestMatchesJob> _logger;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IVacanciesDetailsRepository _vacanciesDetailsRepository;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IUsersService _usersService;
        private readonly IBrokerProcuder _brokerProcuder;
        private readonly IVacancyTrainingDataConverter _trainingDataConverter;
        private readonly VacancyRecommendationsModel _vacancyRecommendationsModel;

        public NotifyAboutVacancyBestMatchesJob(
            ILogger<NotifyAboutVacancyBestMatchesJob> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IVacanciesDetailsRepository vacanciesDetailsRepository,
            IReadInteractionsRepository readInteractionsRepository,
            IUsersService usersService,
            IBrokerProcuder brokerProcuder,
            IVacancyTrainingDataConverter trainingDataConverter,
            VacancyRecommendationsModel vacancyRecommendationsModel)
        {
            _logger = logger;
            _readVacanciesRepository = readVacanciesRepository;
            _vacanciesDetailsRepository = vacanciesDetailsRepository;
            _readInteractionsRepository = readInteractionsRepository;
            _usersService = usersService;
            _brokerProcuder = brokerProcuder;
            _trainingDataConverter = trainingDataConverter;
            _vacancyRecommendationsModel = vacancyRecommendationsModel;
        }

        public async Task ExecuteAsync(Guid vacancyId)
        {
            _logger.LogInformation("[NotifyMatchesJob] Start handle for vacancy {VacancyId}", vacancyId);

            var vacancy = await _readVacanciesRepository.GetAsync(vacancyId);
            var vacancyDetails = await _vacanciesDetailsRepository.GetByAsync(details => details.VacancyId, vacancyId);
            var resumes = await GetResumesAsync(vacancyDetails);

            var trainingData = GetTrainingDataForVacancy(resumes, vacancyDetails);
            trainingData = ProcessPredictions(trainingData);

            var events = GetRecomendationEvents(trainingData, vacancy);

            await ProduceRecommendationEventsAsync(events);
        }

        private async Task<List<Resume>> GetResumesAsync(VacancyDetailsEntity vacancyDetails)
        {
            return await _usersService.GetBestResumesAsync(
                vacancyDetails.Skills,
                vacancyDetails.Tags,
                vacancyDetails.Languages.Select(languageEntity => new Language()
                {
                    Name = languageEntity.Name,
                    Level = languageEntity.Level,
                }).ToList());
        }

        private List<TrainingVacancyRecommendationData> GetTrainingDataForVacancy(
           List<Resume> resumes,
           VacancyDetailsEntity vacancyDetails)
        {
            var trainingData = new List<TrainingVacancyRecommendationData>();

            resumes.AsParallel().ForAll(resume =>
            {
                var data = new TrainingVacancyRecommendationData
                {
                    VacancyId = vacancyDetails.VacancyId,
                    UserId = resume.UserId,

                    ResumeSkills = _trainingDataConverter.ConvertList(resume.Skills),
                    ResumeTags = _trainingDataConverter.ConvertList(resume.Tags),
                    ResumeLanguages = _trainingDataConverter.ConvertLanguages(resume.Languages),

                    VacancySkills = _trainingDataConverter.ConvertList(vacancyDetails.Skills),
                    VacancyTags = _trainingDataConverter.ConvertList(vacancyDetails.Tags),
                    VacancyLanguages = _trainingDataConverter.ConvertLanguages(vacancyDetails.Languages),
                    VacancyExperience = _trainingDataConverter.ConvertExperience(vacancyDetails.Experience),
                    VacancySalary = _trainingDataConverter.ConvertSalary(vacancyDetails.Salary),

                    InteractionType = (int)InteractionType.None,
                };

                trainingData.Add(data);
            });

            return trainingData;
        }

        private List<TrainingVacancyRecommendationData> ProcessPredictions(List<TrainingVacancyRecommendationData> trainingData)
        {
            trainingData.AsParallel().ForAll(item =>
            {
                item.SuitabilityScore = _vacancyRecommendationsModel.PredictInteraction(item);
            });

            return trainingData
                .AsParallel()
                .Where(item => item.SuitabilityScore >= BusinessRules.Vacancy.MinNotificationPredictionScore)
                .OrderByDescending(item => item.SuitabilityScore)
                .ToList();
        }

        private List<RecomendVacancyEvent> GetRecomendationEvents(
            List<TrainingVacancyRecommendationData> trainingData,
            VacancyEntity vacancyEntity)
        {
            var events = new List<RecomendVacancyEvent>();

            foreach (var data in trainingData)
            {
                events.Add(new RecomendVacancyEvent()
                {
                    UserId = data.UserId,
                    MatchScore = data.SuitabilityScore,
                    VacancyId = vacancyEntity.Id,
                    VacancyName = vacancyEntity.Title,
                });
            }

            return events;
        }

        private async Task ProduceRecommendationEventsAsync(List<RecomendVacancyEvent> recomendationEvents)
        {
            foreach (var recomendationEvent in recomendationEvents)
            {
                await _brokerProcuder.SendAsync(recomendationEvent);
            }
        }
    }
}

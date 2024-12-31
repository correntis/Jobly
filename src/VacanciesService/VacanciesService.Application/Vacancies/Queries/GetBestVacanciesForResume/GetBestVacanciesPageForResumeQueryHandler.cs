using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.Vacancies.Recommendations.ML;
using VacanciesService.Domain.Abstractions.Repositories.Cache;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Enums;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetBestVacanciesForResume
{
    public class GetBestVacanciesPageForResumeQueryHandler
        : IRequestHandler<GetBestVacanciesPageForResumeQuery, IEnumerable<Vacancy>>
    {
        private readonly ILogger<GetBestVacanciesPageForResumeQuery> _logger;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IRecommendationsCacheRepository _cache;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;
        private readonly IVacancyTrainingDataConverter _trainingDataConverter;
        private readonly VacancyRecommendationsModel _recommendationModel;

        public GetBestVacanciesPageForResumeQueryHandler(
            ILogger<GetBestVacanciesPageForResumeQuery> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IReadInteractionsRepository readInteractionsRepository,
            IVacanciesDetailsRepository detailsRepository,
            IRecommendationsCacheRepository cache,
            IUsersService usersService,
            IMapper mapper,
            IVacancyTrainingDataConverter trainingDataConverter,
            VacancyRecommendationsModel recommendationModel)
        {
            _logger = logger;
            _readVacanciesRepository = readVacanciesRepository;
            _readInteractionsRepository = readInteractionsRepository;
            _detailsRepository = detailsRepository;
            _cache = cache;
            _usersService = usersService;
            _mapper = mapper;
            _trainingDataConverter = trainingDataConverter;
            _recommendationModel = recommendationModel;
        }

        public async Task<IEnumerable<Vacancy>> Handle(GetBestVacanciesPageForResumeQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for resume with ID {ResumeId}",
                request.GetType().Name,
                request.ResumeId);

            var cachedVacancies = await TryGetFromCacheAsync(request.ResumeId, token);

            if(cachedVacancies is not null)
            {
                return cachedVacancies
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);
            }

            var resume = await _usersService.GetResumeAsync(request.ResumeId, token);
            var vacanciesDetailsEntities = await GetFilteredVacancyDetailsAsync(resume, token);
            var interactionsEntities = await GetInteractionsForVacanciesAsync(
                vacanciesDetailsEntities.Select(vd => vd.VacancyId), resume, token);

            var trainingData = GetTrainingDataForResume(resume, vacanciesDetailsEntities, interactionsEntities);

            trainingData = ProcessPredictions(trainingData);

            var recommendedVacancies =
                await LoadFullVacanciesAsync(vacanciesDetailsEntities, trainingData.Select(t => t.VacancyId), token);

            await CacheVacanciesAsync(resume.Id, recommendedVacancies, token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for resume with ID {ResumeId}",
                request.GetType().Name,
                request.ResumeId);

            return recommendedVacancies
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize);
        }

        private async Task<IEnumerable<Vacancy>> TryGetFromCacheAsync(string resumeId, CancellationToken token)
        {
            return await _cache.GetVacanciesAsync(resumeId, token);
        }

        private async Task CacheVacanciesAsync(string resumeId, List<Vacancy> vacancies, CancellationToken token)
        {
            var expiresTime = DateTime.UtcNow.AddHours(BusinessRules.Vacancy.CacheExpiresHours);

            await _cache.SetVacanciesAsync(resumeId, vacancies, expiresTime, token);
        }

        private List<TrainingVacancyRecommendationData> ProcessPredictions(List<TrainingVacancyRecommendationData> trainingData)
        {
            trainingData.AsParallel().ForAll(item =>
            {
                item.SuitabilityScore = _recommendationModel.PredictInteraction(item);
            });

            return trainingData
                .AsParallel()
                .Where(item => item.SuitabilityScore >= BusinessRules.Vacancy.MinRecommendationPredictionScore)
                .OrderByDescending(item => item.SuitabilityScore)
                .ToList();
        }

        private List<TrainingVacancyRecommendationData> GetTrainingDataForResume(
            Resume resume,
            List<VacancyDetailsEntity> vacanciesDetails,
            List<VacancyInteractionEntity> interactions)
        {
            var trainingData = new List<TrainingVacancyRecommendationData>();
            var interactionMap = interactions.ToDictionary(i => i.VacancyId);

            vacanciesDetails.AsParallel().ForAll(details =>
            {
                interactionMap.TryGetValue(details.VacancyId, out var interaction);

                var data = new TrainingVacancyRecommendationData
                {
                    VacancyId = details.VacancyId,

                    ResumeSkills = _trainingDataConverter.ConvertList(resume.Skills),
                    ResumeTags = _trainingDataConverter.ConvertList(resume.Tags),
                    ResumeLanguages = _trainingDataConverter.ConvertLanguages(resume.Languages),

                    VacancySkills = _trainingDataConverter.ConvertList(details.Skills),
                    VacancyTags = _trainingDataConverter.ConvertList(details.Tags),
                    VacancyLanguages = _trainingDataConverter.ConvertLanguages(details.Languages),
                    VacancyExperience = _trainingDataConverter.ConvertExperience(details.Experience),
                    VacancySalary = _trainingDataConverter.ConvertSalary(details.Salary),

                    InteractionType = interaction?.Type ?? (int)InteractionType.None,
                };

                trainingData.Add(data);
            });

            return trainingData;
        }

        private async Task<List<Vacancy>> LoadFullVacanciesAsync(
            IEnumerable<VacancyDetailsEntity> vacanciesDetailsEntities,
            IEnumerable<Guid> vacanciesIds,
            CancellationToken token)
        {
            var detailsMap = vacanciesDetailsEntities.ToDictionary(d => d.VacancyId);

            var vacanciesEntities = await _readVacanciesRepository.GetAllIn(vacanciesIds.ToList(), token);

            var vacancies = _mapper.Map<List<Vacancy>>(vacanciesEntities);

            foreach (var vacancy in vacancies)
            {
                if (detailsMap.TryGetValue(vacancy.Id, out VacancyDetailsEntity detailsEntity))
                {
                    vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(detailsEntity);
                }
            }

            return vacancies;
        }

        private async Task<List<VacancyDetailsEntity>> GetFilteredVacancyDetailsAsync(Resume resume, CancellationToken token)
        {
            var detailsFilter = _mapper.Map<VacancyDetailsFilter>(resume);

            var detailsEntities = await _detailsRepository.GetFilteredAsync(detailsFilter, token);

            return detailsEntities;
        }

        private async Task<List<VacancyInteractionEntity>> GetInteractionsForVacanciesAsync(
            IEnumerable<Guid> vacanciesIds,
            Resume resume,
            CancellationToken token)
        {
            var interactions =
                await _readInteractionsRepository.GetAllByUserAndVacanciesAsync(resume.UserId, vacanciesIds.ToList(), token);

            return interactions;
        }
    }
}

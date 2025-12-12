using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Enums;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacanciesByInteraction
{
    public class GetVacanciesByInteractionQueryHandler : IRequestHandler<GetVacanciesByInteractionQuery, List<Vacancy>>
    {
        private readonly ILogger<GetVacanciesByInteractionQueryHandler> _logger;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IMapper _mapper;

        public GetVacanciesByInteractionQueryHandler(
            ILogger<GetVacanciesByInteractionQueryHandler> logger,
            IReadInteractionsRepository readInteractionsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readInteractionsRepository = readInteractionsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _mapper = mapper;
        }

        public async Task<List<Vacancy>> Handle(GetVacanciesByInteractionQuery request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for user {UserId} with interaction type {InteractionType}",
                request.GetType().Name,
                request.UserId,
                request.InteractionType);

            var vacancyIds = await _readInteractionsRepository.GetVacancyIdsByUserAndTypeAsync(
                request.UserId,
                request.InteractionType,
                request.PageNumber,
                request.PageSize,
                token);

            if (vacancyIds.Count == 0)
            {
                _logger.LogInformation(
                    "No vacancies found for user {UserId} with interaction type {InteractionType}",
                    request.UserId,
                    request.InteractionType);
                return new List<Vacancy>();
            }

            var vacanciesEntities = await _readVacanciesRepository.GetAllIn(vacancyIds, token);
            var vacancies = _mapper.Map<List<Vacancy>>(vacanciesEntities);

            _logger.LogInformation(
                "Successfully handled {QueryName} for user {UserId} with interaction type {InteractionType}. Found {Count} vacancies",
                request.GetType().Name,
                request.UserId,
                request.InteractionType,
                vacancies.Count);

            return vacancies;
        }
    }
}


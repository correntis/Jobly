using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetVacancyInteractions
{
    public class GetVacancyInteractionsQueryHandler : IRequestHandler<GetVacancyInteractionsQuery, List<VacancyInteraction>>
    {
        private readonly ILogger<GetVacancyInteractionsQueryHandler> _logger;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IMapper _mapper;

        public GetVacancyInteractionsQueryHandler(
            ILogger<GetVacancyInteractionsQueryHandler> logger,
            IReadInteractionsRepository readInteractionsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readInteractionsRepository = readInteractionsRepository;
            _mapper = mapper;
        }

        public async Task<List<VacancyInteraction>> Handle(GetVacancyInteractionsQuery request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for user with ID {VacancyId}",
                request.GetType().Name,
                request.VacancyId);

            var interactionsEntities = await _readInteractionsRepository.GetAllByVacancyAsync(request.VacancyId, token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for vacancy with ID {VacancyId}",
                request.GetType().Name,
                request.VacancyId);

            return _mapper.Map<List<VacancyInteraction>>(interactionsEntities);
        }
    }
}

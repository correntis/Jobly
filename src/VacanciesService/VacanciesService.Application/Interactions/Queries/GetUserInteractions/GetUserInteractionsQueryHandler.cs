using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetUserInteractions
{
    public class GetUserInteractionsQueryHandler : IRequestHandler<GetUserInteractionsQuery, List<VacancyInteraction>>
    {
        private readonly ILogger<GetUserInteractionsQueryHandler> _logger;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IMapper _mapper;

        public GetUserInteractionsQueryHandler(
            ILogger<GetUserInteractionsQueryHandler> logger,
            IReadInteractionsRepository readInteractionsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readInteractionsRepository = readInteractionsRepository;
            _mapper = mapper;
        }

        public async Task<List<VacancyInteraction>> Handle(GetUserInteractionsQuery request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            var interactionsEntities = await _readInteractionsRepository.GetAllByUserAsync(request.UserId, token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for vacancy with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            return _mapper.Map<List<VacancyInteraction>>(interactionsEntities);
        }
    }
}

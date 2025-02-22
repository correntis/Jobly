using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Interactions.Queries.GetUserForVacancyInteractions
{
    public class GetUserForVacancyInteractionsQueryHandler : IRequestHandler<GetUserForVacancyInteractionQuery, VacancyInteraction>
    {
        private readonly ILogger<GetUserForVacancyInteractionsQueryHandler> _logger;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IMapper _mapper;

        public GetUserForVacancyInteractionsQueryHandler(
            ILogger<GetUserForVacancyInteractionsQueryHandler> logger,
            IReadInteractionsRepository readInteractionsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readInteractionsRepository = readInteractionsRepository;
            _mapper = mapper;
        }

        public async Task<VacancyInteraction> Handle(GetUserForVacancyInteractionQuery request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start {QueryName} user ID {UserId}",
                request.GetType().Name,
                request.UserId);

            var interactionEntity = await _readInteractionsRepository.GetByUserAndVacancy(request.UserId, request.VacancyId, token);

            if(interactionEntity is null)
            {
                throw new EntityNotFoundException($"Interaction for user {request.UserId} and vacancy {request.VacancyId} not found");
            }

            _logger.LogInformation(
                "Success {QueryName} user ID {UserId}",
                request.GetType().Name,
                request.UserId);

            return _mapper.Map<VacancyInteraction>(interactionEntity);
        }
    }
}

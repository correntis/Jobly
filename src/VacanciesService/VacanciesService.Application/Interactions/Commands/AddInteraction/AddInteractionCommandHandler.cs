using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Interactions.Commands.AddInteraction
{
    public class AddInteractionCommandHandler : IRequestHandler<AddInteractionCommand, Guid>
    {
        private readonly ILogger<AddInteractionCommandHandler> _logger;
        private readonly IWriteInteractionsRepository _writeInteractionsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public AddInteractionCommandHandler(
            ILogger<AddInteractionCommandHandler> logger,
            IWriteInteractionsRepository writeInteractionsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IUsersService usersService,
            IMapper mapper)
        {
            _logger = logger;
            _writeInteractionsRepository = writeInteractionsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _usersService = usersService;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(AddInteractionCommand request, CancellationToken token = default)
        {
            _logger.LogInformation(
               "Start handling {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
               request.GetType().Name,
               request.VacancyId,
               request.UserId);

            var vacancyEntity = await _readVacanciesRepository.GetAsync(request.VacancyId, token);
            if(vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with id {request.VacancyId} not found");
            }

            var isUserExist = await _usersService.IsUserExistsAsync(request.UserId, token);
            if(!isUserExist)
            {
                throw new EntityNotFoundException($"User with id {request.UserId} not found");
            }

            var interactionEntity = _mapper.Map<VacancyInteractionEntity>(request);

            interactionEntity.CreatedAt = DateTime.UtcNow;

            await _writeInteractionsRepository.AddAsync(interactionEntity, token);

            await _writeInteractionsRepository.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            return interactionEntity.Id;
        }
    }
}

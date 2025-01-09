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
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public AddInteractionCommandHandler(
            ILogger<AddInteractionCommandHandler> logger,
            IWriteInteractionsRepository writeInteractionsRepository,
            IReadInteractionsRepository readInteractionsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IUsersService usersService,
            IMapper mapper)
        {
            _logger = logger;
            _writeInteractionsRepository = writeInteractionsRepository;
            _readInteractionsRepository = readInteractionsRepository;
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

            await CheckVacancyExistenceAsync(request.VacancyId, token);

            await CheckUserExistenceAsync(request.UserId, token);

            var applicationId = Guid.Empty;

            var existingInteraction = await _readInteractionsRepository.GetByUserAndVacancy(request.UserId, request.VacancyId, token);

            if(existingInteraction is not null)
            {
                applicationId = await HandleUpdateAsync(existingInteraction, request, token);
            }
            else
            {
                applicationId = await HandleAddAsync(request, token);
            }

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            return applicationId;
        }

        private async Task CheckVacancyExistenceAsync(Guid vacancyId, CancellationToken token)
        {
            var vacancyEntity = await _readVacanciesRepository.GetAsync(vacancyId, token);

            if(vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with id {vacancyId} not found");
            }
        }

        private async Task CheckUserExistenceAsync(Guid userId, CancellationToken token)
        {
            var isUserExist = await _usersService.IsUserExistsAsync(userId, token);

            if(!isUserExist)
            {
                throw new EntityNotFoundException($"User with id {userId} not found");
            }
        }

        private async Task<Guid> HandleUpdateAsync(
            VacancyInteractionEntity existingInteraction,
            AddInteractionCommand request,
            CancellationToken token)
        {
            _writeInteractionsRepository.Attach(existingInteraction);

            _mapper.Map(request, existingInteraction);

            _writeInteractionsRepository.Update(existingInteraction);

            await _writeInteractionsRepository.SaveChangesAsync(token);

            return existingInteraction.Id;
        }

        private async Task<Guid> HandleAddAsync(
            AddInteractionCommand request,
            CancellationToken token)
        {
            var interactionEntity = _mapper.Map<VacancyInteractionEntity>(request);
            interactionEntity.CreatedAt = DateTime.UtcNow;

            await _writeInteractionsRepository.AddAsync(interactionEntity, token);

            await _writeInteractionsRepository.SaveChangesAsync(token);

            return interactionEntity.Id;
        }
    }
}

using AutoMapper;
using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.AddApplication
{
    public class AddApplicationCommandHandler : IRequestHandler<AddApplicationCommand, Guid>
    {
        private readonly ILogger<AddApplicationCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IWriteApplicationsRepository _writeApplicationsRepository;
        private readonly IUsersService _usersService;
        private readonly IBrokerProcuder _brokerProcuder;

        public AddApplicationCommandHandler(
            ILogger<AddApplicationCommandHandler> logger,
            IMapper mapper,
            IReadVacanciesRepository readVacanciesRepository,
            IWriteApplicationsRepository writeApplicationsRepository,
            IUsersService usersService,
            IBrokerProcuder brokerProcuder)
        {
            _logger = logger;
            _mapper = mapper;
            _readVacanciesRepository = readVacanciesRepository;
            _writeApplicationsRepository = writeApplicationsRepository;
            _usersService = usersService;
            _brokerProcuder = brokerProcuder;
        }

        public async Task<Guid> Handle(AddApplicationCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            await CheckUserExistence(request.UserId, token);

            var vacancyEntity = await GetVacancyEntity(request.VacancyId, token);

            _writeApplicationsRepository.AttachVacancy(vacancyEntity);

            var applicationEntity = _mapper.Map<ApplicationEntity>(request);

            applicationEntity.Status = BusinessRules.Application.DefaultStatus;
            applicationEntity.CreatedAt = DateTime.UtcNow;
            applicationEntity.Vacancy = vacancyEntity;

            await _writeApplicationsRepository.AddAsync(applicationEntity, token);

            await _writeApplicationsRepository.SaveChangesAsync(token);

            await ProduceApplicationEventAsync(applicationEntity, token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            return applicationEntity.Id;
        }

        private async Task CheckUserExistence(Guid userId, CancellationToken token)
        {
            var isUserExists = await _usersService.IsUserExistsAsync(userId, token);

            if (!isUserExists)
            {
                throw new EntityNotFoundException($"User with ID {userId} not found");
            }
        }

        private async Task<VacancyEntity> GetVacancyEntity(Guid vacancyId, CancellationToken token)
        {
            var vacancyEntity = await _readVacanciesRepository.GetAsync(vacancyId, token);

            if (vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {vacancyId} not found");
            }

            await _readVacanciesRepository.LoadApplications(vacancyEntity, token);

            return vacancyEntity;
        }

        private async Task ProduceApplicationEventAsync(ApplicationEntity applicationEntity, CancellationToken token)
        {
            var applicationEvent = new VacancyApplicationEvent()
            {
                ApplicationId = applicationEntity.Id,
                VacacnyId = applicationEntity.Vacancy.Id,
                UserId = applicationEntity.UserId,
                CompanyId = applicationEntity.Vacancy.CompanyId,
            };

            await _brokerProcuder.SendAsync(applicationEvent, token);
        }
    }
}

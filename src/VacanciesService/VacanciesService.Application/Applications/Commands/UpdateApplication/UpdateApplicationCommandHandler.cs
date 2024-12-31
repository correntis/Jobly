using AutoMapper;
using Jobly.Brokers.Abstractions;
using Jobly.Brokers.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.UpdateApplication
{
    public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand, Guid>
    {
        private readonly ILogger<UpdateApplicationCommandHandler> _logger;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IWriteApplicationsRepository _writeApplicationsRepository;
        private readonly IMapper _mapper;
        private readonly IBrokerProcuder _brokerProducer;
        private readonly IUsersService _usersService;

        public UpdateApplicationCommandHandler(
            ILogger<UpdateApplicationCommandHandler> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IReadApplicationsRepository readApplicationsRepository,
            IWriteApplicationsRepository writeApplicationsRepository,
            IMapper mapper,
            IBrokerProcuder brokerProducer,
            IUsersService usersService)
        {
            _logger = logger;
            _readVacanciesRepository = readVacanciesRepository;
            _readApplicationsRepository = readApplicationsRepository;
            _writeApplicationsRepository = writeApplicationsRepository;
            _mapper = mapper;
            _brokerProducer = brokerProducer;
            _usersService = usersService;
        }

        public async Task<Guid> Handle(UpdateApplicationCommand request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            var applicationEntity = await _readApplicationsRepository.GetAsync(request.Id, token);

            if(applicationEntity is null)
            {
                throw new EntityNotFoundException($"Application with ID {request.Id} not found");
            }

            _mapper.Map(request, applicationEntity);

            applicationEntity.AppliedAt = DateTime.UtcNow;

            _writeApplicationsRepository.Update(applicationEntity);

            await _writeApplicationsRepository.SaveChangesAsync(token);

            await ProduceResponseEventAsync(applicationEntity, token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            return applicationEntity.Id;
        }

        private async Task ProduceResponseEventAsync(ApplicationEntity applicationEntity, CancellationToken token)
        {
            var username = await _usersService.GetUserNameAsync(applicationEntity.UserId, token);

            var responseEvent = new ApplicationResponseEvent()
            {
                ApplicationId = applicationEntity.Id,
                ApplicationStatus = applicationEntity.Status,
                UserId = applicationEntity.UserId,
                UserName = username,
                VacancyId = applicationEntity.Id,
                VacancyTitle = applicationEntity.Vacancy.Title,
            };

            await _brokerProducer.SendAsync(responseEvent, token);
        }
    }
}

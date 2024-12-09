using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Applications;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.UpdateApplicationCommand
{
    public class UpdateApplicationCommandHandler : IRequestHandler<UpdateApplicationCommand, Guid>
    {
        private readonly ILogger<UpdateApplicationCommandHandler> _logger;
        private readonly IReadApplicationsRepository _readApplicationsRepository;
        private readonly IWriteApplicationsRepository _writeApplicationsRepository;
        private readonly IMapper _mapper;

        public UpdateApplicationCommandHandler(
            ILogger<UpdateApplicationCommandHandler> logger,
            IReadVacanciesRepository readVacanciesRepository,
            IReadApplicationsRepository readApplicationsRepository,
            IWriteApplicationsRepository writeApplicationsRepository,
            IMapper mapper)
        {
            _logger = logger;
            _readApplicationsRepository = readApplicationsRepository;
            _writeApplicationsRepository = writeApplicationsRepository;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateApplicationCommand request, CancellationToken token = default)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            var applicationEntity = await _readApplicationsRepository.GetAsync(request.Id, token);

            if (applicationEntity is null)
            {
                throw new EntityNotFoundException($"Application with ID {request.Id} not found");
            }

            _mapper.Map(request, applicationEntity);

            applicationEntity.AppliedAt = DateTime.Now;

            _writeApplicationsRepository.Update(applicationEntity);

            await _writeApplicationsRepository.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for application with ID {ApplicationId}",
                request.GetType().Name,
                request.Id);

            return applicationEntity.Id;
        }
    }
}

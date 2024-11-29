using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Applications.Commands.AddApplicationCommand
{
    public class AddApplicationCommandHandler : IRequestHandler<AddApplicationCommand, Guid>
    {
        private readonly ILogger<AddApplicationCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesWriteContext _vacanciesContext;

        public AddApplicationCommandHandler(
            ILogger<AddApplicationCommandHandler> logger,
            IMapper mapper,
            IVacanciesWriteContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<Guid> Handle(AddApplicationCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            // TODO Check if user with request.UserId exists with gRPC request
            // for users service

            var vacancyEntity = await _vacanciesContext.Vacancies
                .FirstOrDefaultAsync(v => v.Id == request.VacancyId, token);

            if (vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {request.VacancyId} not found");
            }

            await _vacanciesContext.Vacancies
                .Entry(vacancyEntity)
                .Collection(v => v.Applications)
                .LoadAsync(token);

            var applicationEntity = _mapper.Map<ApplicationEntity>(request);

            applicationEntity.Status = BusinessRules.Application.DefaultStatus;
            applicationEntity.CreatedAt = DateTime.Now;

            await _vacanciesContext.Applications.AddAsync(applicationEntity, token);

            vacancyEntity.Applications.Add(applicationEntity);

            await _vacanciesContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and user with ID {UserId}",
                request.GetType().Name,
                request.VacancyId,
                request.UserId);

            return applicationEntity.Id;
        }
    }
}

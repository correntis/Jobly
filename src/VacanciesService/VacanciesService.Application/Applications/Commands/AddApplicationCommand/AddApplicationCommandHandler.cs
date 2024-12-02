using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Services;
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
        private readonly IUsersService _usersService;

        public AddApplicationCommandHandler(
            ILogger<AddApplicationCommandHandler> logger,
            IMapper mapper,
            IVacanciesWriteContext vacanciesContext,
            IUsersService usersService)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
            _usersService = usersService;
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

        public async Task CheckUserExistence(Guid userId, CancellationToken token)
        {
            var isUserExists = await _usersService.IsUserExistsAsync(userId, token);

            if (!isUserExists)
            {
                throw new EntityNotFoundException($"User with ID {userId} not found");
            }
        }

        public async Task<VacancyEntity> GetVacancyEntity(Guid vacancyId, CancellationToken token)
        {
            var vacancyEntity = await _vacanciesContext.Vacancies.FirstOrDefaultAsync(v => v.Id == vacancyId, token);

            if (vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {vacancyId} not found");
            }

            await _vacanciesContext.Vacancies
                .Entry(vacancyEntity)
                .Collection(v => v.Applications)
                .LoadAsync(token);

            return vacancyEntity;
        }
    }
}

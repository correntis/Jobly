using AutoMapper;
using DnsClient.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.Vacancies.Commands.AddVacancyCommand
{
    public class AddVacancyCommandHandler : IRequestHandler<AddVacancyCommand, Guid>
    {
        private readonly ILogger<AddVacancyCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IWriteVacanciesRepository _writeVacanciesRepository;
        private readonly IUsersService _usersService;

        public AddVacancyCommandHandler(
            ILogger<AddVacancyCommandHandler> logger,
            IMapper mapper,
            IWriteVacanciesRepository writeVacanciesRepository,
            IUsersService usersService)
        {
            _logger = logger;
            _mapper = mapper;
            _writeVacanciesRepository = writeVacanciesRepository;
            _usersService = usersService;
        }

        public async Task<Guid> Handle(AddVacancyCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with Title {VacancyTitle}",
                request.GetType().Name,
                request.Title);

            await CheckCompanyExistence(request.CompanyId, token);

            var vacancyEntity = _mapper.Map<VacancyEntity>(request);

            vacancyEntity.Archived = false;
            vacancyEntity.CreatedAt = DateTime.UtcNow;

            await _writeVacanciesRepository.AddAsync(vacancyEntity, token);

            await _writeVacanciesRepository.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with Title {VacancyTitle} and ID {VacancyId}",
                request.GetType().Name,
                vacancyEntity.Title,
                vacancyEntity.Id);

            return vacancyEntity.Id;
        }

        public async Task CheckCompanyExistence(Guid companyId, CancellationToken token)
        {
            var isCompanyExists = await _usersService.IsCompanyExistsAsync(companyId, token);

            if (!isCompanyExists)
            {
                throw new EntityNotFoundException($"Company with ID {companyId} not found");
            }
        }
    }
}

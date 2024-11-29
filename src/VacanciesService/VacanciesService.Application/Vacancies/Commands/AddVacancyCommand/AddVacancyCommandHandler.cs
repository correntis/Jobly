using AutoMapper;
using DnsClient.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;

namespace VacanciesService.Application.Vacancies.Commands.AddVacancyCommand
{
    public class AddVacancyCommandHandler : IRequestHandler<AddVacancyCommand, Guid>
    {
        private readonly ILogger<AddVacancyCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesWriteContext _vacanciesContext;

        public AddVacancyCommandHandler(
            ILogger<AddVacancyCommandHandler> logger,
            IMapper mapper,
            IVacanciesWriteContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<Guid> Handle(AddVacancyCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with Title {VacancyTitle}",
                request.GetType().Name,
                request.Title);

            // TODO Check if company with request.CompanyId exists with gRPC request
            // for users service

            var vacancyEntity = _mapper.Map<VacancyEntity>(request);

            vacancyEntity.Archived = false;
            vacancyEntity.CreatedAt = DateTime.Now;

            await _vacanciesContext.Vacancies.AddAsync(vacancyEntity, token);

            await _vacanciesContext.SaveChangesAsync(token);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with Title {VacancyTitle} and ID {VacancyId}",
                request.GetType().Name,
                vacancyEntity.Title,
                vacancyEntity.Id);

            return vacancyEntity.Id;
        }
    }
}

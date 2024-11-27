using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyQuery
{
    public class GetVacancyQueryHandler : IRequestHandler<GetVacancyQuery, Vacancy>
    {
        private readonly ILogger<GetVacancyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetVacancyQueryHandler(
            ILogger<GetVacancyQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
            _detailsRepository = detailsRepository;
        }

        public async Task<Vacancy> Handle(GetVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            var vacancyEntity = await _vacanciesContext.Vacancies.FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (vacancyEntity == null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {vacancyEntity.Id} not found");
            }

            var vacancyDetailsEntity = await _detailsRepository.GetByAsync(v => v.VacancyId, vacancyEntity.Id, token);

            var vacancy = _mapper.Map<Vacancy>(vacancyEntity);

            vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(vacancyDetailsEntity);

            _logger.LogInformation("Successfully handled {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            return vacancy;
        }
    }
}

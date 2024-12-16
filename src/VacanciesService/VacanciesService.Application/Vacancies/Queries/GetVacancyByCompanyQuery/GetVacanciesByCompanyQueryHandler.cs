using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyByCompanyQuery
{
    public class GetVacanciesByCompanyQueryHandler : IRequestHandler<GetVacanciesByCompanyQuery, List<Vacancy>>
    {
        private readonly ILogger<GetVacanciesByCompanyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetVacanciesByCompanyQueryHandler(
            ILogger<GetVacanciesByCompanyQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<Vacancy>> Handle(GetVacanciesByCompanyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            var vacanciesEntities = await _vacanciesContext.Vacancies
                .Where(v => v.CompanyId == request.CompanyId)
                .ToListAsync(token);

            var detailsEntities = await _detailsRepository.GetManyByAsync(
                vd => vd.VacancyId,
                vacanciesEntities.Select(v => v.Id),
                token);

            var detailsMap = detailsEntities.ToDictionary(d => d.VacancyId);
            var vacancies = _mapper.Map<List<Vacancy>>(vacanciesEntities);

            foreach (var vacancy in vacancies)
            {
                if (detailsMap.TryGetValue(vacancy.Id, out var detailsEntity))
                {
                    vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(detailsEntity);
                }
            }

            _logger.LogInformation("Successfully handled {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            return vacancies;
        }
    }
}

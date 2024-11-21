using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyByCompanyQuery
{
    public class GetVacanciesByCompanyQueryHandler : IRequestHandler<GetVacanciesByCompanyQuery, List<Vacancy>>
    {
        private readonly ILogger<GetVacanciesByCompanyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;

        public GetVacanciesByCompanyQueryHandler(
            ILogger<GetVacanciesByCompanyQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<Vacancy>> Handle(GetVacanciesByCompanyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            var vacanciesEntities = await _vacanciesContext.Vacancies
                .Where(v => v.CompanyId == request.CompanyId)
                .ToListAsync(token);

            _logger.LogInformation("Successfully handled {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            return _mapper.Map<List<Vacancy>>(vacanciesEntities);
        }
    }
}

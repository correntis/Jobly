using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacanciesByCompanyQuery
{
    public class GetVacanciesByCompanyQueryHandler : IRequestHandler<GetVacanciesByCompanyQuery, List<Vacancy>>
    {
        private readonly ILogger<GetVacanciesByCompanyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;

        public GetVacanciesByCompanyQueryHandler(
            ILogger<GetVacanciesByCompanyQueryHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository,
            IReadVacanciesRepository readVacanciesRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
            _readVacanciesRepository = readVacanciesRepository;
        }

        public async Task<List<Vacancy>> Handle(GetVacanciesByCompanyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            var vacanciesEntities = await _readVacanciesRepository.GetAllByCompanyAsync(request.CompanyId);

            var detailsEntities = await _detailsRepository.GetManyByAsync(
                vd => vd.VacancyId,
                vacanciesEntities.Select(v => v.Id),
                token);

            var detailsMap = detailsEntities.ToDictionary(d => d.VacancyId);
            var vacancies = _mapper.Map<List<Vacancy>>(vacanciesEntities);

            foreach(var vacancy in vacancies)
            {
                if(detailsMap.TryGetValue(vacancy.Id, out var detailsEntity))
                {
                    vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(detailsEntity);
                }
            }

            _logger.LogInformation("Successfully handled {QueryName} for company with ID {CompanyId}", request.GetType().Name, request.CompanyId);

            return vacancies;
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancy
{
    public class GetVacancyQueryHandler : IRequestHandler<GetVacancyQuery, Vacancy>
    {
        private readonly ILogger<GetVacancyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;

        public GetVacancyQueryHandler(
            ILogger<GetVacancyQueryHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository,
            IReadVacanciesRepository readVacanciesRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
            _readVacanciesRepository = readVacanciesRepository;
        }

        public async Task<Vacancy> Handle(GetVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            var vacancyEntity = await _readVacanciesRepository.GetAsync(request.Id, token);

            if(vacancyEntity == null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {request.Id} not found");
            }

            var vacancyDetailsEntity = await _detailsRepository.GetByAsync(v => v.VacancyId, vacancyEntity.Id, token);

            var vacancy = _mapper.Map<Vacancy>(vacancyEntity);

            vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(vacancyDetailsEntity);

            _logger.LogInformation("Successfully handled {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            return vacancy;
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Entities.SQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetVacancyQuery
{
    public class GetVacancyQueryHandler : IRequestHandler<GetVacancyQuery, Vacancy>
    {
        private readonly ILogger<GetVacancyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;

        public GetVacancyQueryHandler(
            ILogger<GetVacancyQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<Vacancy> Handle(GetVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            var vacancyEntity = await QueryWithIncludes(request).FirstOrDefaultAsync(x => x.Id == request.Id, token);

            if (vacancyEntity == null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {vacancyEntity.Id} not found");
            }

            _logger.LogInformation("Successfully handled {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.Id);

            return _mapper.Map<Vacancy>(vacancyEntity);
        }

        private IQueryable<VacancyEntity> QueryWithIncludes(GetVacancyQuery request)
        {
            var query = _vacanciesContext.Vacancies.AsQueryable();

            if (request.IncludeApplications)
            {
                query = query.Include(v => v.Applications);
            }

            return query;
        }
    }
}

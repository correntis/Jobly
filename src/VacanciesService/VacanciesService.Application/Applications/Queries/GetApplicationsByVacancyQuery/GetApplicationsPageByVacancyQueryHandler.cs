using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByVacancyQuery
{
    public class GetApplicationsPageByVacancyQueryHandler
        : IRequestHandler<GetApplicationsPageByVacancyQuery, List<Domain.Models.Application>>
    {
        private readonly ILogger<GetApplicationsPageByVacancyQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;

        public GetApplicationsPageByVacancyQueryHandler(
            ILogger<GetApplicationsPageByVacancyQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<Domain.Models.Application>> Handle(GetApplicationsPageByVacancyQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for vacancy with ID {UserId}",
                request.GetType().Name,
                request.VacancyId);

            var applicationsEntities = await _vacanciesContext.Applications
                .Where(a => a.Id == request.VacancyId)
                .OrderBy(a => a.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageNumber)
                .Take(request.PageNumber)
                .ToListAsync(token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for vacancy with ID {UserId}",
                request.GetType().Name,
                request.VacancyId);

            return _mapper.Map<List<Domain.Models.Application>>(applicationsEntities);
        }
    }
}

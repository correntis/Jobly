using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Contexts;

namespace VacanciesService.Application.Applications.Queries.GetApplicationsByUserQuery
{
    public class GetApplicationsByUserQueryHandler : IRequestHandler<GetApplicationsByUserQuery, List<Domain.Models.Application>>
    {
        private readonly ILogger<GetApplicationsByUserQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesReadContext _vacanciesContext;

        public GetApplicationsByUserQueryHandler(
            ILogger<GetApplicationsByUserQueryHandler> logger,
            IMapper mapper,
            IVacanciesReadContext vacanciesContext)
        {
            _logger = logger;
            _mapper = mapper;
            _vacanciesContext = vacanciesContext;
        }

        public async Task<List<Domain.Models.Application>> Handle(GetApplicationsByUserQuery request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {QueryName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            var applicationsEntities = await _vacanciesContext.Applications
                .Where(a => a.UserId == request.UserId)
                .OrderBy(a => a.CreatedAt)
                .Include(a => a.Vacancy)
                .ToListAsync(token);

            _logger.LogInformation(
                "Successfully handled {QueryName} for user with ID {UserId}",
                request.GetType().Name,
                request.UserId);

            return _mapper.Map<List<Domain.Models.Application>>(applicationsEntities);
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Queries.GetFilteredVacanciesDetailsQuery
{
    public class GetFilteredVacanciesDetailsQueryHandler
        : IRequestHandler<GetFilteredVacanciesDetailsQuery, List<VacancyDetails>>
    {
        private readonly ILogger<GetFilteredVacanciesDetailsQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetFilteredVacanciesDetailsQueryHandler(
            ILogger<GetFilteredVacanciesDetailsQueryHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
        }

        public async Task<List<VacancyDetails>> Handle(GetFilteredVacanciesDetailsQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} with filter {@Filter}", request.GetType().Name, request.Filter);

            var detailsEntities = await _detailsRepository.GetFilteredPageAsync(
                request.Filter,
                request.Filter.PageNumber,
                request.Filter.PageSize,
                token);

            _logger.LogInformation("Successfully handled {QueryName}",request.GetType().Name);

            return _mapper.Map<List<VacancyDetails>>(detailsEntities);
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.VacanciesDetails.Queries.GetVacancyDetailsQuery
{
    public class GetVacancyDetailsQueryHandler : IRequestHandler<GetVacancyDetailsQuery, VacancyDetails>
    {
        private readonly ILogger<GetVacancyDetailsQuery> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;

        public GetVacancyDetailsQueryHandler(
            ILogger<GetVacancyDetailsQuery> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
        }
        public async Task<VacancyDetails> Handle(GetVacancyDetailsQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} for vacancy with ID {VacancyId}", request.GetType().Name, request.VacancyId);

            var vacancyDetailsEntity = await _detailsRepository.GetByAsync(vd => vd.VacancyId, request.VacancyId, token);

            if (vacancyDetailsEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy_details with ID {request.VacancyId} not found");
            }

            _logger.LogInformation("Successfully handled {QueryName}", request.GetType().Name);

            return _mapper.Map<VacancyDetails>(vacancyDetailsEntity);
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Abstractions;
using VacanciesService.Domain.Abstractions.Contexts;
using VacanciesService.Domain.Abstractions.Repositories;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetFilteredVacanciesQuery
{
    public class GetFilteredVacanciesQueryHandler : IRequestHandler<GetFilteredVacanciesQuery, List<Vacancy>>
    {
        private readonly ILogger<GetFilteredVacanciesQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IVacanciesReadContext _vacanciesContext;
        private readonly ICurrencyApiService _currencyApi;
        private readonly ICurrencyConverter _currencyConverter;

        public GetFilteredVacanciesQueryHandler(
            ILogger<GetFilteredVacanciesQueryHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository,
            IVacanciesReadContext vacanciesContext,
            ICurrencyApiService currencyApi,
            ICurrencyConverter currencyConverter)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
            _vacanciesContext = vacanciesContext;
            _currencyApi = currencyApi;
            _currencyConverter = currencyConverter;
        }

        public async Task<List<Vacancy>> Handle(GetFilteredVacanciesQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} with filter {@Filter}", request.GetType().Name, request.Filter);

            if (request.Filter.Salary is not null)
            {
                request.Filter.Salary = await CalculateSalaryAsync(request.Filter.Salary);
            }

            var detailsEntities = await _detailsRepository.GetFilteredPageAsync(
                request.Filter,
                request.Filter.PageNumber,
                request.Filter.PageSize,
                token);

            var vacanciesIds = detailsEntities.Select(x => x.VacancyId).ToList();
            var detailsMap = detailsEntities.ToDictionary(d => d.VacancyId);

            var vacanciesEntities = await _vacanciesContext.Vacancies
                .Where(v => vacanciesIds.Contains(v.Id))
                .ToListAsync(token);

            var vacancies = _mapper.Map<List<Vacancy>>(vacanciesEntities);

            foreach (var vacancy in vacancies)
            {
                if (detailsMap.TryGetValue(vacancy.Id, out VacancyDetailsEntity detailsEntity))
                {
                    vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(detailsEntity);
                }
            }

            _logger.LogInformation("Successfully handled {QueryName}", request.GetType().Name);

            return vacancies;
        }

        public async Task<SalaryFilter> CalculateSalaryAsync(SalaryFilter sourceFilter)
        {
            if (sourceFilter.Currency == BusinessRules.Salary.DefaultCurrency)
            {
                return sourceFilter;
            }

            var exchangeRate = await _currencyApi.GetExchangeRateAsync(sourceFilter.Currency);

            var targetFilter = new SalaryFilter()
            {
                Currency = BusinessRules.Salary.DefaultCurrency,
                Min = _currencyConverter.Convert(sourceFilter.Min, exchangeRate.Value),
                Max = _currencyConverter.Convert(sourceFilter.Max, exchangeRate.Value),
            };

            return targetFilter;
        }
    }
}

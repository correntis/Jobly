using System;
using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Abstractions;
using VacanciesService.Domain.Abstractions.Repositories.Interactions;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Exceptions;
using VacanciesService.Domain.Filters.VacancyDetails;
using VacanciesService.Domain.Models;

namespace VacanciesService.Application.Vacancies.Queries.GetFilteredVacancies
{
    public class GetFilteredVacanciesQueryHandler : IRequestHandler<GetFilteredVacanciesQuery, List<Vacancy>>
    {
        private readonly ILogger<GetFilteredVacanciesQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly IReadInteractionsRepository _readInteractionsRepository;
        private readonly ICurrencyApiService _currencyApi;
        private readonly ICurrencyConverter _currencyConverter;

        public GetFilteredVacanciesQueryHandler(
            ILogger<GetFilteredVacanciesQueryHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            IReadInteractionsRepository readInteractionsRepository,
            ICurrencyApiService currencyApi,
            ICurrencyConverter currencyConverter)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _readInteractionsRepository = readInteractionsRepository;
            _currencyApi = currencyApi;
            _currencyConverter = currencyConverter;
        }

        public async Task<List<Vacancy>> Handle(GetFilteredVacanciesQuery request, CancellationToken token)
        {
            _logger.LogInformation("Start handling {QueryName} with filter {@Filter}", request.GetType().Name, request.Filter);

            if(request.Filter.Salary is not null)
            {
                request.Filter.Salary = await CalculateSalaryAsync(request.Filter.Salary);
            }

            var fetchSize = request.Filter.PageSize * 5;
            var requiredCount = request.Filter.PageSize * request.Filter.PageNumber;
            var allFilteredVacancies = new List<Vacancy>();
            var currentPage = 1;
            const int maxPagesToFetch = 50;
            var allDislikedVacancyIds = new HashSet<Guid>();

            while (allFilteredVacancies.Count < requiredCount && currentPage <= maxPagesToFetch)
            {
                var detailsEntities = await _detailsRepository.GetFilteredPageAsync(
                    request.Filter,
                    currentPage,
                    fetchSize,
                    token);

                if (detailsEntities.Count == 0)
                {
                    break;
                }

                var vacanciesIds = detailsEntities.Select(x => x.VacancyId).ToList();
                var detailsMap = detailsEntities.ToDictionary(d => d.VacancyId);

                var vacanciesEntities = await _readVacanciesRepository.GetAllIn(vacanciesIds, token);

                var filteredVacancies = vacanciesEntities.AsEnumerable();
                
                if (!string.IsNullOrWhiteSpace(request.Filter.Title))
                {
                    filteredVacancies = filteredVacancies
                        .Where(v => v.Title.Contains(request.Filter.Title, StringComparison.OrdinalIgnoreCase));
                }

                if (request.UserId.HasValue && filteredVacancies.Any())
                {
                    var currentBatchVacancyIds = filteredVacancies.Select(v => v.Id).ToList();
                    var dislikedIds = await _readInteractionsRepository.GetDislikedVacancyIdsByUserAsync(
                        request.UserId.Value, currentBatchVacancyIds, token);
                    
                    foreach (var dislikedId in dislikedIds)
                    {
                        allDislikedVacancyIds.Add(dislikedId);
                    }

                    filteredVacancies = filteredVacancies
                        .Where(v => !allDislikedVacancyIds.Contains(v.Id));
                }

                var filteredList = filteredVacancies.ToList();
                var vacancies = _mapper.Map<List<Vacancy>>(filteredList);

                foreach(var vacancy in vacancies)
                {
                    if(detailsMap.TryGetValue(vacancy.Id, out VacancyDetailsEntity detailsEntity))
                    {
                        vacancy.VacancyDetails = _mapper.Map<VacancyDetails>(detailsEntity);
                    }
                }

                allFilteredVacancies.AddRange(vacancies);

                if (detailsEntities.Count < fetchSize)
                {
                    break;
                }

                currentPage++;
            }

            var skip = (request.Filter.PageNumber - 1) * request.Filter.PageSize;
            var result = allFilteredVacancies.Skip(skip).Take(request.Filter.PageSize).ToList();

            _logger.LogInformation(
                "Successfully handled {QueryName}. Fetched {Pages} pages, collected {CollectedCount} after filters, returned {ResultCount}", 
                request.GetType().Name, 
                currentPage - 1, 
                allFilteredVacancies.Count, 
                result.Count);

            return result;
        }

        public async Task<SalaryFilter> CalculateSalaryAsync(SalaryFilter sourceFilter)
        {
            if(sourceFilter.Currency == BusinessRules.Salary.DefaultCurrency)
            {
                return sourceFilter;
            }

            var exchangeRate = await _currencyApi.GetExchangeRateAsync(sourceFilter.Currency);

            if(exchangeRate is null)
            {
                throw new EntityNotFoundException($"Currenct with code {sourceFilter.Currency} not found");
            }

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

using AutoMapper;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using VacanciesService.Application.Abstractions;
using VacanciesService.Application.Vacancies.Jobs;
using VacanciesService.Domain.Abstractions.Repositories.Vacancies;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Entities.NoSQL;
using VacanciesService.Domain.Exceptions;

namespace VacanciesService.Application.VacanciesDetails.Commands.AddVacancyDetails
{
    public class AddVacancyDetailsCommandHandler : IRequestHandler<AddVacancyDetailsCommand, string>
    {
        private readonly ILogger<AddVacancyDetailsCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IVacanciesDetailsRepository _detailsRepository;
        private readonly IReadVacanciesRepository _readVacanciesRepository;
        private readonly ICurrencyApiService _currencyApi;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public AddVacancyDetailsCommandHandler(
            ILogger<AddVacancyDetailsCommandHandler> logger,
            IMapper mapper,
            IVacanciesDetailsRepository detailsRepository,
            IReadVacanciesRepository readVacanciesRepository,
            ICurrencyApiService currencyApi,
            ICurrencyConverter currencyConverter,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _mapper = mapper;
            _detailsRepository = detailsRepository;
            _readVacanciesRepository = readVacanciesRepository;
            _currencyApi = currencyApi;
            _currencyConverter = currencyConverter;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<string> Handle(AddVacancyDetailsCommand request, CancellationToken token)
        {
            _logger.LogInformation(
                "Start handling {CommandName} for vacancy with ID {VacancyId}",
                request.GetType().Name,
                request.VacancyId);

            var vacancyEntity = await _readVacanciesRepository.GetAsync(request.VacancyId, token);
            if (vacancyEntity is null)
            {
                throw new EntityNotFoundException($"Vacancy with ID {request.VacancyId} not found");
            }

            var existingDetails = await _detailsRepository.GetByAsync(vd => vd.VacancyId, request.VacancyId, token);
            if (existingDetails is not null)
            {
                throw new EntityAlreadyExistException($"Vacancy_details for vacancy with ID {request.VacancyId} already exist");
            }

            var vacancyDetailsEntity = _mapper.Map<VacancyDetailsEntity>(request);

            vacancyDetailsEntity.Salary = await CalculateCurrencyAsync(vacancyDetailsEntity.Salary);

            await _detailsRepository.AddAsync(vacancyDetailsEntity, token);

            CreateNotifyJob(vacancyDetailsEntity.VacancyId);

            _logger.LogInformation(
                "Successfully handled {CommandName} for vacancy with ID {VacancyId} and vacancy_details with ID {VacancyDetailsId}",
                request.GetType().Name,
                request.VacancyId,
                vacancyDetailsEntity.Id);

            return vacancyDetailsEntity.Id;
        }

        private async Task<SalaryEntity> CalculateCurrencyAsync(SalaryEntity sourceEntity)
        {
            if(sourceEntity is null)
            {
                return null;
            }

            var exchangeRate = await _currencyApi.GetExchangeRateAsync(sourceEntity.Currency);

            var targetEntity = new SalaryEntity
            {
                Currency = BusinessRules.Salary.DefaultCurrency,
                Min = _currencyConverter.Convert(sourceEntity.Min, exchangeRate.Value),
                Max = _currencyConverter.Convert(sourceEntity.Max, exchangeRate.Value),
                Original = new OriginalSalaryEntity
                {
                    Currency = sourceEntity.Currency,
                    Min = sourceEntity.Min,
                    Max = sourceEntity.Max,
                    ExchangeRate = exchangeRate.Value,
                },
            };

            return targetEntity;
        }

        private void CreateNotifyJob(Guid vacancyId)
        {
            _backgroundJobClient.Enqueue<NotifyAboutVacancyBestMatchesJob>(j => j.ExecuteAsync(vacancyId));
        }
    }
}

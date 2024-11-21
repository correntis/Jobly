using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Configuration;
using VacanciesService.Domain.Constants;
using VacanciesService.Domain.Models;
using VacanciesService.Infrastructure.API.JsonPolicies;
using VacanciesService.Infrastructure.API.Responses;

namespace VacanciesService.Infrastructure.API.Services
{
    public class CurrencyApiService : ICurrencyApiService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;

        private readonly JsonSerializerOptions _serializerOptions;

        public CurrencyApiService(IOptions<CurrencyApiOptions> options)
        {
            _baseUrl = options.Value.BaseUrl;
            _apiKey = options.Value.ApiKey;

            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            };
        }

        public async Task<IEnumerable<Currency>> GetCurrencies()
        {
            var client = GetClient($"currencies?type={BusinessRules.Salary.DefaultCurrencyType}");
            var request = GetRequestWithGetMethod();

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            var currenciesDictionary = JsonSerializer.Deserialize<CurrencyApiResponse>(
                response.Content,
                _serializerOptions);

            return currenciesDictionary.Data.Values.ToList();
        }

        public async Task<ExchangeRate> GetExchangeRate(string currencyCode)
        {
            var client = GetClient($"latest?base_currency={BusinessRules.Salary.DefaultCurrency}&currencies={currencyCode}");
            var request = GetRequestWithGetMethod();

            var response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                throw response.ErrorException;
            }

            var currenciesDictionary = JsonSerializer.Deserialize<ExchangeRateApiResponse>(
                response.Content,
                _serializerOptions);

            return currenciesDictionary.Data.Values.First();
        }

        private RestClient GetClient(string parameters)
        {
            return new RestClient(_baseUrl + parameters);
        }

        private RestRequest GetRequestWithGetMethod()
        {
            return new RestRequest().AddHeader("apikey", _apiKey);
        }
    }
}

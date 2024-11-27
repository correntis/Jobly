using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Json;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Configuration;
using VacanciesService.Domain.Models;
using VacanciesService.Infrastructure.API.JsonPolicies;
using VacanciesService.Infrastructure.API.Responses;

namespace VacanciesService.Infrastructure.API.Services
{
    public class CurrencyApiServiceDevelopmentMock : ICurrencyApiService
    {
        private readonly string _testCurrencyResponse =
          """
           {
                "data": {
                    "AED": {
                        "symbol": "AED",
                        "name": "United Arab Emirates Dirham",
                        "symbol_native": "د.إ",
                        "decimal_digits": 2,
                        "rounding": 0,
                        "code": "AED",
                        "name_plural": "UAE dirhams",
                        "type": "fiat",
                        "countries": [
                            "AE"
                        ]
                    },
                    "AFN": {
                        "symbol": "Af",
                        "name": "Afghan Afghani",
                        "symbol_native": "؋",
                        "decimal_digits": 0,
                        "rounding": 0,
                        "code": "AFN",
                        "name_plural": "Afghan Afghanis",
                        "type": "fiat",
                        "countries": [
                            "AF"
                        ]
                    },
                    "USD": {
                         "symbol": "$",
                         "name": "US Dollar",
                         "symbol_native": "$",
                         "decimal_digits": 2,
                         "rounding": 0,
                         "code": "USD",
                         "name_plural": "US dollars",
                         "type": "fiat",
                         "countries": [
                           "AC",
                           "AS",
                           "BQ",
                           "DG"
                         ]
                    },
                    "EUR": {
                         "symbol": "€",
                         "name": "Euro",
                         "symbol_native": "€",
                         "decimal_digits": 2,
                         "rounding": 0,
                         "code": "EUR",
                         "name_plural": "Euros",
                         "type": "fiat",
                         "countries": [
                           "AD",
                           "AT",
                           "AX",
                           "BE",
                           "BL"
                         ]
                    },
                    "BYN": {
                         "symbol": "Br",
                         "name": "Belarusian ruble",
                         "symbol_native": "Br",
                         "decimal_digits": 2,
                         "rounding": 0,
                         "code": "BYN",
                         "name_plural": "Belarusian rubles",
                         "type": "fiat",
                         "countries": []
                    },
                    "RUB": {
                         "symbol": "RUB",
                         "name": "Russian Ruble",
                         "symbol_native": "руб.",
                         "decimal_digits": 2,
                         "rounding": 0,
                         "code": "RUB",
                         "name_plural": "Russian rubles",
                         "type": "fiat",
                         "countries": [
                           "RU",
                           "SU"
                         ]
                    }
                }
           }
           """;

        private readonly string _testExchangeRateResponse =
            """
             {
                "meta": {
                    "last_updated_at": "2023-06-23T10:15:59Z"
                },
                "data": {
                    "AED": {
                        "code": "AED",
                        "value": 3.67306
                    },
                    "AFN": {
                        "code": "AFN",
                        "value": 91.80254
                    },
                    "ALL": {
                        "code": "ALL",
                        "value": 108.22904
                    },
                    "AMD": {
                        "code": "AMD",
                        "value": 480.41659
                    },
                    "USD": {
                          "code": "USD",
                          "value": 1
                    },
                    "EUR": {
                      "code": "EUR",
                      "value": 0.9427401273
                    },
                    "BYN": {
                      "code": "BYN",
                      "value": 3.2700648341
                    }
                }
            }
            """;

        private readonly JsonSerializerOptions _serializerOptions;

        public CurrencyApiServiceDevelopmentMock(IOptions<CurrencyApiOptions> options)
        {
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            };
        }

        public async Task<IEnumerable<Currency>> GetCurrenciesAsync(CancellationToken token = default)
        {
            var currenciesDictionary = JsonSerializer.Deserialize<CurrencyApiResponse>(
                _testCurrencyResponse,
                _serializerOptions);

            return await Task.FromResult(currenciesDictionary.Data.Values.ToList());
        }

        public async Task<ExchangeRate> GetExchangeRateAsync(string currencyCode, CancellationToken token = default)
        {
            var currenciesDictionary = JsonSerializer.Deserialize<ExchangeRateApiResponse>(
                _testExchangeRateResponse,
                _serializerOptions);

            return await Task.FromResult(currenciesDictionary.Data.Values.FirstOrDefault(er => er.Code == currencyCode));
        }
    }
}

using VacanciesService.Domain.Models;

namespace VacanciesService.Domain.Abstractions.Services
{
    public interface ICurrencyApiService
    {
        Task<IEnumerable<Currency>> GetCurrenciesAsync(CancellationToken token = default);
        Task<ExchangeRate> GetExchangeRateAsync(string currencyCode, CancellationToken token = default);
    }
}
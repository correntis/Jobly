using VacanciesService.Domain.Models;

namespace VacanciesService.Domain.Abstractions.Services
{
    public interface ICurrencyApiService
    {
        Task<IEnumerable<Currency>> GetCurrencies();
        Task<ExchangeRate> GetExchangeRate(string currencyCode);
    }
}
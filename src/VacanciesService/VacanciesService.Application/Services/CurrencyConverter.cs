using VacanciesService.Application.Abstractions;

namespace VacanciesService.Application.Services
{
    public class CurrencyConverter : ICurrencyConverter
    {
        public decimal? Convert(decimal? source, decimal? exchangeRate)
        {
            if (source is null || exchangeRate is null)
            {
                return null;
            }

            return source / exchangeRate;
        }
    }
}

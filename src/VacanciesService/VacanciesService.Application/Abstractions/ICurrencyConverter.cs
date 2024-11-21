namespace VacanciesService.Application.Abstractions
{
    public interface ICurrencyConverter
    {
        decimal? Convert(decimal? source, decimal? exchangeRate);
    }
}
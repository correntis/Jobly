using VacanciesService.Domain.Models;

namespace VacanciesService.Infrastructure.API.Responses
{
    public class CurrencyApiResponse
    {
        public Dictionary<string, Currency> Data { get; set; }
    }
}

using VacanciesService.Domain.Models;

namespace VacanciesService.Infrastructure.API.Responses
{
    public class ExchangeRateApiResponse
    {
        public Dictionary<string, string> Meta { get; set; }
        public Dictionary<string, ExchangeRate> Data { get; set; }
    }
}

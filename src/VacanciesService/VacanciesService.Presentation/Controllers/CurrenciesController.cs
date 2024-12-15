using Microsoft.AspNetCore.Mvc;
using VacanciesService.Domain.Abstractions.Services;

namespace VacanciesService.Presentation.Controllers
{
    [ApiController]
    [Route("/currencies")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyApiService _currencyApi;

        public CurrenciesController(ICurrencyApiService currencyApi)
        {
            _currencyApi = currencyApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies(CancellationToken token)
        {
            return Ok(await _currencyApi.GetCurrenciesAsync(token));
        }
    }
}

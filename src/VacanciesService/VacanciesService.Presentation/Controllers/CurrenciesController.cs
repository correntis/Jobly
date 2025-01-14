using Microsoft.AspNetCore.Mvc;
using VacanciesService.Domain.Abstractions.Services;
using VacanciesService.Domain.Models;

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
        public async Task<ActionResult<List<Currency>>> GetCurrencies(CancellationToken token)
        {
            return Ok(await _currencyApi.GetCurrenciesAsync(token));
        }
    }
}
